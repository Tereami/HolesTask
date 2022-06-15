using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace HolesTask
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]

    class CommandSettings : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            FormSettingsCreateTask form = new HolesTask.FormSettingsCreateTask();
            form.ShowDialog();

            if (form.DialogResult != System.Windows.Forms.DialogResult.OK) return Result.Cancelled;

            return Result.Succeeded;
        }
    }
}
