using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace HolesTask
{
    public static class StaticPlaceTask
    {
        public static string widthParamGuid = "8f2e4f93-9472-4941-a65d-0ac468fd6a5d"; //Рзм.Ширина
        public static string heigthParamGuid = "da753fe3-ecfa-465b-9a2c-02f55d0c2ff1"; //Рзм.Высота
        public static string thicknessParamGuid = "293f055d-6939-4611-87b7-9a50d0c1f50e"; // Рзм.Толщина
        public static string baseLevelElevParamGuid = "9f5f7e49-616e-436f-9acc-5305f34b6933"; // Рзм.ВысотаБазовогоУровня
        public static string offsetFromLevelParamGuid = "515dc061-93ce-40e4-859a-e29224d80a10"; //Рзм.СмещениеОтУровня


        public static FamilyInstance CreateNewTask(GenericMepElement gme, XYZ placePoint)
        {
            Document doc = gme.elem.Document;

            Family taskFam = StaticFamily.GetFamilyByName(doc, Settings.nameTaskWallFamily);
            FamilySymbol fs = doc.GetElement(taskFam.GetFamilySymbolIds().First()) as FamilySymbol;

            FamilyInstance task = doc.Create.NewFamilyInstance(placePoint, fs, gme.baseLevel, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
            return task;
        }


        /// <summary>
        /// Размещает элемент-задание по информации и размещении, полученной из XML-файла
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="fs"></param>
        /// <param name="locInfo"></param>
        /// <returns></returns>
        public static FamilyInstance PlaceTaskByInfo(Document doc, FamilySymbol fs, LocationInfo locInfo)
        {
            Level lev = locInfo.FindLevel(doc);
            XYZ placePoint = new XYZ(locInfo.X, locInfo.Y, locInfo.OffsetFromLevel);

            FamilyInstance task = doc.Create.NewFamilyInstance(placePoint, fs, lev, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
            return task;
        }

        /// <summary>
        /// Устанавливает размеры и поворот элемента-задания при создании нового задания
        /// </summary>
        /// <param name="gme"></param>
        /// <param name="task"></param>
        /// <param name="intersectPoint"></param>
        /// <param name="gse"></param>
        /// <returns></returns>
        public static FamilyInstance ActivateNewTask(GenericMepElement gme, FamilyInstance task, XYZ intersectPoint, GenericStructureElement gse)
        {
            if (gse.Elem is Wall)
            {
                Wall linkedWall = gse.Elem as Wall;
                double locAngle = StaticAngle.CalculateWallMepAngle(gme, linkedWall).HorizontalAngle;
                double width2 = linkedWall.Width / (Math.Tan(locAngle));
                if (width2 < 0) width2 = width2 * -1;
                width2 += gme.width;

                //SetTaskDimensions(task, gme.Heigth, width2, gse.Thickness, Settings.holeOffset, HostTypes.Wall);
                SetTaskDimensions(task, gme, gse);

                AngleCalculationResult angle = StaticAngle.CalculateWallTaskRotateAngle(linkedWall);
                RotateElementByVerticalAxis(task, angle.HorizontalAngle);

                StaticSaveDataToFamilyInstance.SaveHostInfoToTask(task, gme.elem.Document.Title, gme.elem.Id.IntegerValue, gse.Elem.Document.Title, gse.Elem.Id.IntegerValue, "Wall");

            }

            if (gse.Elem is Floor)
            {
                SetTaskDimensions(task, gse.Thickness, gme.Heigth, gme.width, Settings.holeOffset, HostTypes.Floor);

                Floor linkedFloor = gse.Elem as Floor;
                double angle = StaticPlaceTask.CalculateVerticalMepElemRotationAngle(gme);
                RotateElementByVerticalAxis(task, angle);

                StaticSaveDataToFamilyInstance.SaveHostInfoToTask(task, gme.elem.Document.Title, gme.elem.Id.IntegerValue, gse.Elem.Document.Title, gse.Elem.Id.IntegerValue, "Floor");

            }

            SetTaskElevation(task, gme, intersectPoint);
            return task;
        }

        /// <summary>
        /// Устанавливает размеры и поворот элемента-задания при получении задания из XML-файла
        /// </summary>
        /// <param name="locInfo"></param>
        /// <param name="hi"></param>
        /// <param name="fi"></param>
        /// <returns></returns>
        public static FamilyInstance ActivateNewTask(LocationInfo locInfo, HostInfo hi, FamilyInstance fi)
        {
            SetTaskDimensions(fi, locInfo.H, locInfo.B, locInfo.L, 0, hi.HostType);

            if (hi.HostType == HostTypes.Wall)
            {
                RotateElementByVerticalAxis(fi, locInfo.Angle);
                SetTaskElevationParamValues(fi, locInfo.BaseLevel, locInfo.OffsetFromLevel);
                StaticSaveDataToFamilyInstance.SaveHostInfoToTask(fi, hi.MepFileName, hi.MepElemId, hi.ConstrFileName, hi.ConstrElemId, "Wall");
            }

            if (hi.HostType == HostTypes.Floor)
            {

            }

            return fi;
        }




        public static string RefreshTask(FamilyInstance task)
        {
            HostInfo hi = new HostInfo(task);
            if (hi == null) return "Не удалось получить информацию из элемента-задания";

            Document doc = task.Document;


            Element mepElem = doc.GetElement(new ElementId(hi.MepElemId));
            if (mepElem == null) return "Элемент MEP удален из модели, обновление невозможно, id" + hi.MepElemId;
            GenericMepElement gme = new GenericMepElement(mepElem);

            RevitLinkInstance rli = StaticLinkedDoc.GetRevitLinkInstanceByTitle(doc, hi.ConstrFileName);
            if (rli == null) return "Не найден связанный файл " + hi.ConstrFileName;

            Document linkedDoc = StaticLinkedDoc.GetLinkDocumentByInstance(rli);
            if (linkedDoc == null) return "Не загружен связанный файл " + hi.ConstrFileName;

            
            Element linkedElem = linkedDoc.GetElement(new ElementId(hi.ConstrElemId));
            GenericStructureElement gse = new HolesTask.GenericStructureElement(linkedElem);
            if (gse.Correct != true)
                return "В связанном файле не найден элемент id" + hi.ConstrElemId;


            XYZ newPoint = Intersection.GetIntersectionPoint(gme.locationCurve, gse.Elem);
            if (newPoint == null) return "Элементы изменены так, что более не пересекаются. Обновление задания невозможно.";

            LocationPoint lp = task.Location as LocationPoint;
            XYZ oldPoint = lp.Point;
            XYZ moveVector = new XYZ(newPoint.X - oldPoint.X, newPoint.Y - oldPoint.Y, newPoint.Z - oldPoint.Z);

            ElementTransformUtils.MoveElement(doc, task.Id, moveVector);

            if (gse.Elem is Wall)
            {
                Wall linkedWall = gse.Elem as Wall;
                double oldAngle = lp.Rotation;
                RotateElementByVerticalAxis(task, -oldAngle);
                AngleCalculationResult angle = StaticAngle.CalculateWallTaskRotateAngle(gse.Elem as Wall);

                RotateElementByVerticalAxis(task, angle.HorizontalAngle);

                SetTaskDimensions(task, gme, gse);
                SetTaskElevation(task, gme, newPoint);
            }
            if (gse.Elem is Floor)
            {

            }
            return "1";
        }





        private static bool SetTaskDimensions(FamilyInstance task, GenericMepElement gme, GenericStructureElement gse)
        {
            if (gse.Elem is Wall)
            {
                Wall wall = gse.Elem as Wall;
                double locAngle = StaticAngle.CalculateWallMepAngle(gme, wall).HorizontalAngle;
                double widthAdd = wall.Width / (Math.Tan(locAngle));
                if (widthAdd < 0) widthAdd = widthAdd * -1;
                double finishWidth = gme.width + 2 * widthAdd;
                SetTaskDimensions(task, gme.Heigth, finishWidth, gse.Thickness, Settings.holeOffset, HostTypes.Wall);
            }
            else if(gse.Elem is Floor)
            {
                Floor floor = gse.Elem as Floor;
                
            }

            return true;
        }


        private static bool SetTaskDimensions(FamilyInstance task, double h, double b, double t, double offset, HostTypes hostType)
        {
            try
            {
                if (hostType == HostTypes.Wall)
                {
                    task.get_Parameter(new Guid(widthParamGuid)).Set(b + offset);
                    task.get_Parameter(new Guid(heigthParamGuid)).Set(h + offset);
                    task.get_Parameter(new Guid(thicknessParamGuid)).Set(t);
                    return true;
                }
                if (hostType == HostTypes.Floor)
                {
                    task.get_Parameter(new Guid(widthParamGuid)).Set(b);
                    task.get_Parameter(new Guid(heigthParamGuid)).Set(h + offset);
                    task.get_Parameter(new Guid(thicknessParamGuid)).Set(t + offset);
                    return true;
                }
                else return false;
            }
            catch { return false; }
        }


        private static bool SetTaskElevation(FamilyInstance task, GenericMepElement gme, XYZ intersectionPoint)
        {
            try
            {
                double taskElev = intersectionPoint.Z - gme.baseLevel.Elevation;
                task.get_Parameter(BuiltInParameter.INSTANCE_FREE_HOST_OFFSET_PARAM).Set(taskElev);

                SetTaskElevationParamValues(task, gme.baseLevel, taskElev);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool SetTaskElevationParamValues(FamilyInstance task, Level level, double elevation)
        {
            task.get_Parameter(new Guid(baseLevelElevParamGuid)).Set(level.Elevation); //Рзм.ВысотаБазовогоУровня
            task.get_Parameter(new Guid(offsetFromLevelParamGuid)).Set(elevation); //Рзм.СмещениеОтУровня
            return true;
        }

        private static bool RotateElementByVerticalAxis(FamilyInstance task, double angle)
        {
            LocationPoint lp = task.Location as LocationPoint;
            XYZ basePoint = lp.Point;

            XYZ pointToRotate = new XYZ(basePoint.X, basePoint.Y, 1 + basePoint.Z);
            Line axis = Line.CreateBound(basePoint, pointToRotate);

            ElementTransformUtils.RotateElement(task.Document, task.Id, axis, angle);

            return true;
        }


        

        public static double CalculateVerticalMepElemRotationAngle(GenericMepElement gme)
        {
            double angle = 0;


            return angle;
        }

    }
}
