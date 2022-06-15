using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace HolesTask
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]

    class CommandCreateHoleTask : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            try
            {
                Selection sel = commandData.Application.ActiveUIDocument.Selection;

                ISelectionFilter isf1 = new mepFilter();
                Reference rf1 = sel.PickObject(ObjectType.Element, isf1, "Выберите трубу или воздуховод");
                Element el1 = doc.GetElement(rf1.ElementId);

                GenericMepElement gme = new GenericMepElement(el1);

                //ISelectionFilter isf = new constrFilter();
                Reference rf2 = sel.PickObject(ObjectType.LinkedElement, "Выберите стены в связанном файле");
                ElementId linkedElemId = rf2.LinkedElementId;
                RevitLinkInstance linkInstance = doc.GetElement(rf2.ElementId) as RevitLinkInstance;
                Document linkedDoc = linkInstance.GetLinkDocument();
                Element linkedElem = linkedDoc.GetElement(linkedElemId);

                GenericStructureElement gse = new GenericStructureElement(linkedElem);
                if (gse.Correct == false)
                {
                    message = "Выберите стену или перекрытие в связанном файле";
                    return Result.Failed;
                }
                
                XYZ intersectPoint = Intersection.GetIntersectionPoint(gme.locationCurve, linkedElem);
                if (intersectPoint == null)
                {
                    message = "Нет пересечения";
                    return Result.Failed;
                }


                using (Transaction ttt = new Transaction(doc))
                {
                    ttt.Start("Создание задания");
                    FamilyInstance task = StaticPlaceTask.CreateNewTask(gme, intersectPoint);
                    ttt.Commit();

                    ttt.Start("Активация задания");
                    StaticPlaceTask.ActivateNewTask(gme, task, intersectPoint, gse);
                    ttt.Commit();
                }


                return Result.Succeeded;
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException) { return Result.Cancelled; }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }

        }



    }
}
