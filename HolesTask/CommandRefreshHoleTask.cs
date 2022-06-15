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

    class CommandRefreshHoleTask : IExternalCommand
    {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
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

            using (Transaction ttt = new Transaction(doc))
            {
                ttt.Start("Обновление задания");

                string result = StaticPlaceTask.RefreshTask(task);
                if(result != "1")
                {
                    message = "Не удалось обновить задание: " + result;
                    return Result.Failed;
                }

                ttt.Commit();
            }
            return Result.Succeeded;
        }
    }
}
