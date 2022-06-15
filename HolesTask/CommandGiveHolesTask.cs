using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows.Forms;

namespace HolesTask
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]

    class CommandGiveHolesTask : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            string path = "";

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Файл задания (*.xml)|*.xml";
            DialogResult dr = ofd.ShowDialog();
            if (dr == DialogResult.OK)
            {
                path = ofd.FileName;
            }


            List<XMLinfo> infos = StaticXML.ReadTaskFile(path);

            Family taskFam = StaticFamily.GetFamilyByName(doc, "Задание");
            FamilySymbol taskfamSymbol = StaticFamily.GetSymbolByName(doc, taskFam, "Тип 1");



            using (Transaction t = new Transaction(doc))
            {
                t.Start("Получение задания на отверстия");
                foreach (XMLinfo info in infos)
                {
                    FamilyInstance task = StaticPlaceTask.PlaceTaskByInfo(doc, taskfamSymbol, info.LocInfo);
                    info.task = task;
                }
                t.Commit();

                t.Start("Активация заданий на отверстия");

                foreach (XMLinfo info in infos)
                {
                    FamilyInstance task = info.task;
                    StaticPlaceTask.ActivateNewTask(info.LocInfo, info.HInfo, task);
                }


                t.Commit();

            }
            return Result.Succeeded;
        }
    }
}
