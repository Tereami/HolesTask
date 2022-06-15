using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

namespace HolesTask
{
    public static class StaticXMLsettings
    {
        public static bool CheckAndActivateSettings(string settingsPath)
        {
            string settingsFilePath = System.IO.Path.Combine(settingsPath, "settings.xml");

            bool checkSettingsFileExists = System.IO.File.Exists(settingsFilePath);
            if (checkSettingsFileExists == true)
            {
                Settings.nameHoleFloor = StaticXMLsettings.GetValue(settingsFilePath, "nameHoleFloor");
                Settings.nameHoleWall = StaticXMLsettings.GetValue(settingsFilePath, "nameHoleWall");
                Settings.nameTaskWallFamily = StaticXMLsettings.GetValue(settingsFilePath, "nameTaskWallFamily");

                string o = StaticXMLsettings.GetValue(settingsFilePath, "holeOffset");
                double offset = 100 / 304.8;
                bool check = double.TryParse(o, out offset);

                if (check == true)
                {
                    Settings.holeOffset = double.Parse(o);
                }
            }
            else
            {
                CheckExistsAndCreateXml(settingsFilePath);
                SaveSettings(settingsPath);
            }

            return true;
        }

        public static bool SaveSettings(string settingsPath)
        {
            string settingsFilePath = System.IO.Path.Combine(settingsPath, "settings.xml");
            SetValue(settingsFilePath, "nameHoleFloor", Settings.nameHoleFloor);
            SetValue(settingsFilePath, "nameHoleWall", Settings.nameHoleWall);
            SetValue(settingsFilePath, "nameTaskWallFamily", Settings.nameTaskWallFamily);
            SetValue(settingsFilePath, "holeOffset", Settings.holeOffset.ToString());

            return true;
        }


        /// <summary>
        /// Получает текстово значение с указанным именем из XML-файла
        /// </summary>
        /// <param name="xmlFilePath">Полный путь к XML-файлу</param>
        /// <param name="fieldName">Имя параметра для получения</param>
        /// <returns>Значение требуемого параметра. При отсутствии параметра в файле возвращается пустая строка.</returns>
        public static string GetValue(string xmlFilePath, string fieldName)
        {
            string value = "";
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(xmlFilePath);
            XmlElement xRoot = xDoc.DocumentElement;

            XmlNode xn = xRoot.SelectSingleNode(fieldName);
            if (xn != null) value = xn.InnerText;

            return value;
        }


        /// <summary>
        /// Записывает значение параметра в XML-файл.
        /// </summary>
        /// <param name="xmlFilePath">Полный путь к XML-файлу.</param>
        /// <param name="fieldName">Имя параметра, в который будет выполнена запись. При отсутствии параметра он будет создан.</param>
        /// <param name="value">Значение параметра для записи.</param>
        /// <returns>True в случае, если запись успешно выполнена, иначе false.</returns>
        public static bool SetValue(string xmlFilePath, string fieldName, string value)
        {
            CheckExistsAndCreateXml(xmlFilePath);

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(xmlFilePath);
            XmlElement xRoot = xDoc.DocumentElement;

            XmlNode xn = xRoot.SelectSingleNode(fieldName);

            XmlText text = xDoc.CreateTextNode(value);
            if (xn == null)
            {
                XmlElement fieldElem = xDoc.CreateElement(fieldName);
                fieldElem.AppendChild(text);
                xRoot.AppendChild(fieldElem);
            }
            else
            {
                xn.InnerText = value;
            }
            xDoc.Save(xmlFilePath);
            return true;
        }


        public static XmlDocument CheckExistsAndCreateXml(string xmlPath)
        {
            string directoryPath = System.IO.Path.GetDirectoryName(xmlPath);
            CheckExistsAndCreateDirectory(directoryPath);

            string startElement = "info";
            XmlDocument xDoc = new XmlDocument();

            bool check = File.Exists(xmlPath);

            if (check == false)
            {
                try
                {
                    XmlWriter xw = XmlWriter.Create(xmlPath);
                    xw.WriteStartDocument();
                    xw.WriteStartElement(startElement);
                    xw.WriteEndElement();
                    xw.Close();
                }
                catch { return null; }
            }

            xDoc.Load(xmlPath);
            return xDoc;

        }

        public static string CheckExistsAndCreateDirectory(string directoryPath)
        {
            bool check = System.IO.Directory.Exists(directoryPath);
            string resPath = "";
            if (check == false)
            {
                DirectoryInfo di = System.IO.Directory.CreateDirectory(directoryPath);
                resPath = di.FullName;
                return resPath;
            }
            return directoryPath;
        }


    }
}
