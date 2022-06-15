using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace HolesTask
{
    public class VariableField
    {
        public string Name { get; }

        public Type FieldType { get; }
        private object Value;

        //public VariableField(string name, object value)
        //{
        //    Name = name;
        //    FieldType = value.GetType();
        //    Value = value;
        //}

        public VariableField(object value)
        {
            FieldType = value.GetType();
            Value = value;
        }


        public string AsString()
        {
            if (FieldType == typeof(ElementId))
            {
                ElementId id = Value as ElementId;
                return id.IntegerValue.ToString();
            }

            if (FieldType == typeof(string))
            {
                string s = Value as string;
                return s;
            }

            if (FieldType == typeof(int))
            {
                return Value.ToString();
            }

            if (FieldType == typeof(double))
            {
                return Value.ToString();
            }

            return "INVALID";
        }

        public int AsInteger()
        {
            if (FieldType != typeof(int)) throw new Exception("Type mismatch");
            string s = Value as string;
            int i = int.Parse(s);
            return i;
        }

        public double AsDouble()
        {
            if (FieldType != typeof(double)) throw new Exception("Type mismatch");
            string s = Value as string;
            double d = int.Parse(s);
            return d;
        }

        public ElementId AsElementid()
        {
            if (FieldType != typeof(ElementId)) throw new Exception("Type mismatch");
            string s = Value as string;
            int i = int.Parse(s);
            ElementId id = new ElementId(i);
            return id;
        }

    }
}
