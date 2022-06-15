using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.IO;
using System.Windows.Forms;

namespace HolesTask
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]

    class CommandSaveHolesTask : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            List<FamilyInstance> fis = StaticFamily.GetAllInstancesOfFamily(doc, "Задание");

            string path = "";

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.AddExtension = true;
            sfd.DefaultExt = "xml";
            sfd.FileName = "tasks_" + doc.Title.Split('.')[0] + ".xml";
            sfd.RestoreDirectory = false;
            DialogResult dr = sfd.ShowDialog();
            if (dr == DialogResult.OK)
            {
                 path = sfd.FileName;
            }


            foreach (FamilyInstance fi in fis)
            {
                HostInfo hi = new HostInfo(fi);
                if (hi == null) continue;

                LocationInfo li = new LocationInfo(fi);
                if (li == null) continue;

                StaticXML.AddTaskInfo(path, doc.Title, hi, li);
            }


            return Result.Succeeded;
        }
    }
}
