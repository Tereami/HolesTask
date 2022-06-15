using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace HolesTask
{
    public static class StaticXML
    {
        /// <summary>
        /// Запись в XML-файл информации о размещенном элементе-задании
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="mainFileName"></param>
        /// <param name="hostInfo"></param>
        /// <param name="locInfo"></param>
        /// <returns></returns>
        public static bool AddTaskInfo(string filePath, string mainFileName, HostInfo hostInfo, LocationInfo locInfo)
        {
            CheckExistsAndCreateXml(filePath, "tasks");

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(filePath);
            XmlElement xRoot = xDoc.DocumentElement;

            XmlElement taskElem = xDoc.CreateElement("task");

            AddFieldToXmlElement(taskElem, HostInfo.fieldMepFileName, mainFileName);
            AddFieldToXmlElement(taskElem, HostInfo.fieldLinkedFileName, hostInfo.ConstrFileName);
            AddFieldToXmlElement(taskElem, HostInfo.fieldMepElemId, hostInfo.MepElemId.ToString());
            AddFieldToXmlElement(taskElem, HostInfo.fieldConstrElemId, hostInfo.ConstrElemId.ToString());
            AddFieldToXmlElement(taskElem, HostInfo.fieldHostType, hostInfo.HostType.ToString());

            AddFieldToXmlElement(taskElem, "X", locInfo.X.ToString());
            AddFieldToXmlElement(taskElem, "Y", locInfo.Y.ToString());
            AddFieldToXmlElement(taskElem, "Z", locInfo.Z.ToString());

            AddFieldToXmlElement(taskElem, "B", locInfo.B.ToString());
            AddFieldToXmlElement(taskElem, "H", locInfo.H.ToString());
            AddFieldToXmlElement(taskElem, "L", locInfo.L.ToString());

            AddFieldToXmlElement(taskElem, "Angle", locInfo.Angle.ToString());
            AddFieldToXmlElement(taskElem, "Level", locInfo.BaseLevel.Elevation.ToString());

            xRoot.AppendChild(taskElem);

            xDoc.Save(filePath);

            return true;
        }

        /// <summary>
        /// Чтение из XML-файла информации о всех элементах-заданиях
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<XMLinfo> ReadTaskFile(string path)
        {
            List<XMLinfo> infos = new List<HolesTask.XMLinfo>();

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(path);
            XmlElement xRoot = xDoc.DocumentElement;

            XmlNodeList childnodes = xRoot.SelectNodes("task");

            foreach (XmlNode xn in childnodes)
                {
                    string mepFileName = xn.SelectSingleNode(HostInfo.fieldMepFileName).InnerText;
                    string constrFileName = xn.SelectSingleNode(HostInfo.fieldLinkedFileName).InnerText;
                    int mepElemId = Int32.Parse(xn.SelectSingleNode(HostInfo.fieldMepElemId).InnerText);
                    int constrElemId = Int32.Parse(xn.SelectSingleNode(HostInfo.fieldConstrElemId).InnerText);
                    string hostType = xn.SelectSingleNode(HostInfo.fieldHostType).InnerText;
                    HostInfo hi = new HolesTask.HostInfo(mepFileName, mepElemId, constrFileName, constrElemId, HostInfo.FromString(hostType));

                    double x = Double.Parse(xn.SelectSingleNode("X").InnerText);
                    double y = Double.Parse(xn.SelectSingleNode("Y").InnerText);
                    double z = Double.Parse(xn.SelectSingleNode("Z").InnerText);
                    double b = Double.Parse(xn.SelectSingleNode("B").InnerText);
                    double h = Double.Parse(xn.SelectSingleNode("H").InnerText);
                    double l = Double.Parse(xn.SelectSingleNode("L").InnerText);
                    double angle = Double.Parse(xn.SelectSingleNode("Angle").InnerText);
                    double level = Double.Parse(xn.SelectSingleNode("Level").InnerText);

                    LocationInfo li = new HolesTask.LocationInfo(x, y, z, b, h, l, angle, level);

                    XMLinfo xi = new HolesTask.XMLinfo(mepFileName, constrFileName, hi, li);
                    infos.Add(xi);
                }
            

            return infos;
        }


        private static XmlElement AddFieldToXmlElement(XmlElement elem, string fieldName, string value)
        {
            XmlDocument xDoc = elem.OwnerDocument;

            XmlElement fieldElem = xDoc.CreateElement(fieldName);
            XmlText text = xDoc.CreateTextNode(value);
            fieldElem.AppendChild(text);

            elem.AppendChild(fieldElem);

            return elem;
        }



        private static bool CheckExistsAndCreateXml(string path, string startElement)
        {
            bool check = File.Exists(path);

            if (check == false)
            {
                try
                {
                    XmlWriter xw = XmlWriter.Create(path);
                    xw.WriteStartDocument();
                    xw.WriteStartElement(startElement);
                    xw.WriteEndElement();
                    xw.Close();
                    return true;
                }
                catch { return false; }
            }
            return true;

        }
    }
}
