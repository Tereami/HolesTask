using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace HolesTask
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]

    class CommandConfirmHole : IExternalCommand
    {
        Result IExternalCommand.Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            Selection sel = commandData.Application.ActiveUIDocument.Selection;
            FamilyInstance task = null;

            if (sel.GetElementIds().Count == 1)
            {
                ElementId selId = sel.GetElementIds().First();
                Element elem = doc.GetElement(selId);
                task = elem as FamilyInstance;
            }
            if (task == null)
            {
                ISelectionFilter isf1 = new taskFilter();
                Reference rf1 = sel.PickObject(ObjectType.Element, isf1, "Выберите установленный элемент задания");
                task = doc.GetElement(rf1.ElementId) as FamilyInstance;
            }

            LocationInfo li = new HolesTask.LocationInfo(task);
            Level baseLevel = doc.GetElement(task.LevelId) as Level;
            double offsetFromLevel = task.get_Parameter(BuiltInParameter.INSTANCE_FREE_HOST_OFFSET_PARAM).AsDouble();

            HostInfo hi = new HolesTask.HostInfo(task);
            ElementId hostid = new ElementId(hi.ConstrElemId);
            Element hostElem = doc.GetElement(hostid);

            Family fam = StaticFamily.GetFamilyByName(doc, "231_Отверстие прямоуг (Окно_Стена)");
            FamilySymbol fs = doc.GetElement(fam.GetFamilySymbolIds().First()) as FamilySymbol;
            XYZ p = new XYZ(li.X, li.Y, li.Z - li.H/2);

            using (Transaction t = new Transaction(doc))
            {
                t.Start("Подтверждение задания");
                doc.Delete(task.Id);
                FamilyInstance hole = doc.Create.NewFamilyInstance(p, fs, hostElem, baseLevel, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                t.Commit();

                t.Start("Подтверждение задания2");
                hole.get_Parameter(new Guid(StaticPlaceTask.widthParamGuid)).Set(li.B);
                hole.get_Parameter(new Guid(StaticPlaceTask.heigthParamGuid)).Set(li.H);

                StaticPlaceTask.SetTaskElevationParamValues(hole, baseLevel, offsetFromLevel - li.H / 2);


                t.Commit();
            }



                return Result.Succeeded;
        }
    }
}
