using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;

namespace HolesTask
{
    public class HostInfo
    {
        public int MepElemId;
        public string ConstrFileName;
        public int ConstrElemId;
        public string MepFileName;
        
        public HostTypes HostType { get; }

        public static Dictionary<string, VariableField> fields = new Dictionary<string, VariableField>();
        
        public static void Initialize()
        {
            fields.Add("MepFileName", new HolesTask.VariableField(""));
            fields.Add("MepElemId", new HolesTask.VariableField(0));
            fields.Add("ConstrFileName", new HolesTask.VariableField(""));
            fields.Add("ConstrElemId", new HolesTask.VariableField(0));
            fields.Add("HostType", new HolesTask.VariableField(""));
        }


        public static string fieldMepElemId = "MepElemId";
        public static string fieldLinkedFileName = "ConstrFileName";
        public static string fieldConstrElemId = "ConstrElemId";
        public static string fieldMepFileName = "MepFileName";
        public static string fieldHostType = "HostType";

        /// <summary>
        /// Создание информации о связанных элементах семейства-задания
        /// </summary>
        /// <param name="mepFileName"></param>
        /// <param name="mepElemId"></param>
        /// <param name="constrFileName"></param>
        /// <param name="constrElemId"></param>
        /// <param name="hostType"></param>
        public HostInfo(string mepFileName, int mepElemId, string constrFileName, int constrElemId, HostTypes hostType)
        {
            MepFileName = mepFileName;
            MepElemId = mepElemId;
            ConstrFileName = constrFileName;
            ConstrElemId = constrElemId;
            HostType = hostType;
        }

        /// <summary>
        /// Получение информации о связанных элементов из информации, ранее записанной в ExtensibleStorage семейства-задание
        /// </summary>
        /// <param name="fi"></param>
        public HostInfo(FamilyInstance fi)
        {
            Schema sch = Schema.Lookup(new Guid(StaticSaveDataToFamilyInstance.schemaGuid));
            if (sch == null) return;

            try
            {
                Entity ent = fi.GetEntity(sch);
                MepElemId = ent.Get<int>(sch.GetField(HostInfo.fieldMepElemId));
                ConstrFileName = ent.Get<string>(sch.GetField(HostInfo.fieldLinkedFileName));
                ConstrElemId = ent.Get<int>(sch.GetField(HostInfo.fieldConstrElemId));
                MepFileName = ent.Get<string>(sch.GetField(HostInfo.fieldMepFileName));

                string hostTypeString = ent.Get<string>(sch.GetField(HostInfo.fieldHostType));
                HostType = FromString(hostTypeString);
            }
            catch
            {
                MepElemId = -1;
                ConstrFileName = "INVALID";
                ConstrElemId = -1;
            }
        }

        public static HostTypes FromString(string hostTypeString)
        {
            if (hostTypeString == "Wall") return HostTypes.Wall;
            if (hostTypeString == "Floor") return HostTypes.Floor;
            if (hostTypeString == "Roof") return HostTypes.Roof;

            throw new Exception("Недопустимое значение HostType");
        }


    }
}
