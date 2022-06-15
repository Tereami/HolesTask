using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;

namespace HolesTask
{
    public static class StaticFamily
    {
        public static Family GetFamilyByName(Document doc, string familyName)
        {
            Family fam = SearchFamilyByname(doc, familyName);

            if (fam != null) return fam;

            TaskDialog.Show("Предупреждение", "Первый запуск приложения: попытка загрузки семейств мониторинга");
            string familyPath = System.IO.Path.Combine(App.settingsPath, (familyName + ".rfa"));
            bool checkFileExists = System.IO.File.Exists(familyPath);

            if (checkFileExists == false)
            {
                TaskDialog.Show("Ошибка", "Семейство BOX не найдено");
                return null;
            }

            try
            {
                using (Transaction t = new Transaction(doc))
                {
                    t.Start("Load family");
                    doc.LoadFamily(familyPath);
                    t.Commit();
                }
            }
            catch { }

            fam = SearchFamilyByname(doc, familyName);
            if (fam != null) return fam;

            throw new Exception("Не удалось загрузить семейство!. Скопируйте семейство BOX в папку с программой");
        }

        public static Family SearchFamilyByname(Document doc, string familyName)
        {
            FilteredElementCollector col2 = new FilteredElementCollector(doc);

            List<Family> fams = col2
                .OfClass(typeof(Family))
                .Cast<Family>()
                .Where(i => i.Name.Equals(familyName))
                .ToList();

            if (fams.Count != 0)
            {
                Family fam = fams[0];
                return fam;
            }

            return null;
        }

        public static FamilySymbol GetSymbolByName(Document doc, Family fam, string SymbolName)
        {
            ISet<ElementId> symbIds = fam.GetFamilySymbolIds();

            FamilySymbol symbol = null;
            foreach (ElementId id in symbIds)
            {
                symbol = doc.GetElement(id) as FamilySymbol;
                string name = symbol.Name;
                if (name.Equals(SymbolName))
                    break;
            }

            return symbol;
        }

        public static List<FamilyInstance> GetAllInstancesOfFamily(Document doc, string familyName)
        {
            FilteredElementCollector col = new FilteredElementCollector(doc);

            List<FamilyInstance> famIns = col
                .OfClass(typeof(FamilyInstance))
                .Cast<FamilyInstance>()
                .Where(i => i.Symbol.FamilyName.Equals(familyName))
                .ToList();
            return famIns;
        }

        public static void DeleteAllInstances(Document doc, string familyName)
        {
            List<FamilyInstance> famIns = GetAllInstancesOfFamily(doc, familyName);

            using (Transaction t = new Transaction(doc))
            {
                try { t.Start("Удаление экземпляров семейства"); }
                catch { }

                foreach (FamilyInstance fam in famIns)
                {
                    ElementId id = fam.Id;
                    doc.Delete(id);
                }

                try { t.Commit(); }
                catch { }
            }
        }
    }
}
