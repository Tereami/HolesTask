using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.ExtensibleStorage;

namespace HolesTask
{
    /// <summary>
    /// Предназначен для работы с Extensible Storage семейства
    /// </summary>
    public static class StaticSaveDataToFamilyInstance
    {
        public static string schemaGuid = "80595888-d642-4705-be04-85f5d30fa92c";

        /// <summary>
        /// Проверка наличия хранилища информации в семействе
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        private static bool StorageCheckIn(FamilyInstance task)
        {
            Schema sch = Schema.Lookup(new Guid(schemaGuid));
            if (sch == null) return false;

            Entity ent = task.GetEntity(sch);
            if (ent == null) return false;

            try
            {
                string test = ent.Get<string>(sch.GetField(HostInfo.fieldLinkedFileName));
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Создание хранилища информации в семействе
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        private static bool CreateStorage(FamilyInstance task)
        {
            Document doc = task.Document;

            try
            {

                SchemaBuilder sb = new SchemaBuilder(new Guid(schemaGuid));
                sb.SetReadAccessLevel(AccessLevel.Public);

                FieldBuilder fbmepElemId = sb.AddSimpleField(HostInfo.fieldMepElemId, typeof(int));
                FieldBuilder fbConstrFileName = sb.AddSimpleField(HostInfo.fieldLinkedFileName, typeof(string));
                FieldBuilder fbconstrElemId = sb.AddSimpleField(HostInfo.fieldConstrElemId, typeof(int));
                FieldBuilder fbMepFileName = sb.AddSimpleField(HostInfo.fieldMepFileName, typeof(string));
                FieldBuilder fbHostType = sb.AddSimpleField(HostInfo.fieldHostType, typeof(string));

                sb.SetSchemaName("Settings");
                Schema sch = sb.Finish();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool SaveHostInfoToTask(FamilyInstance fi, string mepFileName, int mepElemId, string linkedFileName, int constElemId, string hostType)
        {
            bool check = StorageCheckIn(fi);
            if (check == false) CreateStorage(fi);

            Document doc = fi.Document;
            try
            {
                Schema sch = Schema.Lookup(new Guid(schemaGuid));

                Field f1 = sch.GetField(HostInfo.fieldMepElemId);
                Field f2 = sch.GetField(HostInfo.fieldLinkedFileName);
                Field f3 = sch.GetField(HostInfo.fieldConstrElemId);
                Field f4 = sch.GetField(HostInfo.fieldMepFileName);
                Field f5 = sch.GetField(HostInfo.fieldHostType);

                Entity ent = new Entity(sch);
                ent.Set<int>(f1, mepElemId);
                ent.Set<string>(f2, linkedFileName);
                ent.Set<int>(f3, constElemId);
                ent.Set<string>(f4, mepFileName);
                ent.Set<string>(f5, hostType);

                fi.SetEntity(ent);
                return true;
            }
            catch
            {
                return false;
            }
        }


    }
}
