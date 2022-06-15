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

    class CommandDisallowHole : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            FamilyInstance fi = new FilteredElementCollector(doc)
                .OfClass(typeof(FamilyInstance))
                .Cast < FamilyInstance>()
                .First();

            FamilyInstance fi2 = new FilteredElementCollector(doc)
                .OfClass(typeof(FamilyInstance))
                .Cast<FamilyInstance>()
                .First();


            List<ElementId> ids = new List<ElementId> { fi.Id, fi2.Id };
            ids.Add(fi.Id);
            ids.Add(fi2.Id);

            doc.Create.NewGroup(ids);



            return Result.Succeeded;
        }
    }
}
