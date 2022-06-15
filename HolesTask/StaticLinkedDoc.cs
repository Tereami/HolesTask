using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace HolesTask
{
    public static class StaticLinkedDoc
    {
        public static RevitLinkInstance GetRevitLinkInstanceByTitle(Document mainDoc, string title)
        {
            FilteredElementCollector col = new FilteredElementCollector(mainDoc)
                .OfClass(typeof(RevitLinkInstance));
            if (col.GetElementCount() == 0) return null;

            List<RevitLinkInstance> rlis = col
                .Cast<RevitLinkInstance>()
                .ToList();
            RevitLinkInstance rli = null;
            foreach (RevitLinkInstance r in rlis)
            {
                string name2 = r.Name.Split('.')[0];
                string name3 = name2 + ".rvt";
                if (name3 == title)
                {
                    rli = r;
                    break;
                }
            }
            return rli;
        }

        public static Document GetLinkDocumentByInstance(RevitLinkInstance rli)
        {
            Document linkDoc = rli.GetLinkDocument();
            if (linkDoc == null) return null;

            return linkDoc;
        }
    }
}
