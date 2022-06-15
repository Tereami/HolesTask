using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
using System.Windows.Media.Imaging;

using System.Reflection;


namespace HolesTask
{
    public enum HostTypes { Wall, Floor, Roof };

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]

    public class App : IExternalApplication
    {
        public static string assemblyPath = "";
        public static string settingsPath = "";

        public Result OnStartup(UIControlledApplication application)
        {
            string tabName = "Weandrevit";
            try { application.CreateRibbonTab(tabName); } catch { }

            assemblyPath = Assembly.GetExecutingAssembly().Location;
            settingsPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(assemblyPath), "Settings");
            StaticXMLsettings.CheckAndActivateSettings(settingsPath);

            RibbonPanel panel1 = application.CreateRibbonPanel(tabName, "Передача задания");

            PushButton btnCreate = panel1.AddItem(new PushButtonData(
                "CreateHoleTask",
                "Создать",
                assemblyPath,
                "HolesTask.CommandCreateHoleTask")
                ) as PushButton;
            btnCreate.LargeImage = PngImageSource("HolesTask.Resources.PlaceHoleTask.png");
            btnCreate.ToolTip = "Размещение элемента-задания на пересечении системы и конструкции";

            PushButton btnRefresh = panel1.AddItem(new PushButtonData(
                "RefreshHoleTask",
                "Обновить",
                assemblyPath,
                "HolesTask.CommandRefreshHoleTask")
                ) as PushButton;
            btnRefresh.LargeImage = PngImageSource("HolesTask.Resources.HolesTaskRefresh.png");
            btnRefresh.ToolTip = "Обновление положения и размеров элемента-задания";


            PushButton btnSave = panel1.AddItem(new PushButtonData(
                "SaveHolesTask",
                "Передать",
                assemblyPath,
                "HolesTask.CommandSaveHolesTask")
                ) as PushButton;
            btnSave.LargeImage = PngImageSource("HolesTask.Resources.SaveHolesTask.png");
            btnSave.ToolTip = "Сохранение информации о размещенных заданиях в файл для передачи";


            PushButton btnSettings = panel1.AddItem(new PushButtonData(
                "Settings",
                "Настройки",
                assemblyPath,
                "HolesTask.CommandSettings")
                ) as PushButton;
            btnSettings.LargeImage = PngImageSource("HolesTask.Resources.Settings.png");
            btnSettings.ToolTip = "Настройки модуля передачи и получения заданий";



            RibbonPanel panel2 = application.CreateRibbonPanel(tabName, "Получение задания");

            PushButton btnGive = panel2.AddItem(new PushButtonData(
                "GiveHolesTask",
                "Получить",
                assemblyPath,
                "HolesTask.CommandGiveHolesTask")
                ) as PushButton;
            btnGive.LargeImage = PngImageSource("HolesTask.Resources.GiveHolesTask.png");
            btnGive.ToolTip = "Получение задания на отверстия";

            PushButton btnConfirm = panel2.AddItem(new PushButtonData(
                "ConfirmHoleTask",
                "Утвердить",
                assemblyPath,
                "HolesTask.CommandConfirmHole")
                ) as PushButton;
            btnConfirm.LargeImage = PngImageSource("HolesTask.Resources.ConfirmHoleTask.png");
            btnConfirm.ToolTip = "Утвердить отверстие";


            PushButton btnDisallow = panel2.AddItem(new PushButtonData(
                "DisallowHoleTask",
                "Отклонить",
                assemblyPath,
                "HolesTask.CommandDisallowHole")
                ) as PushButton;
            btnDisallow.LargeImage = PngImageSource("HolesTask.Resources.DisallowHoleTask.png");
            btnDisallow.ToolTip = "Утвердить отверстие";



            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            StaticXMLsettings.SaveSettings(settingsPath);
            return Result.Succeeded;
        }

        private System.Windows.Media.ImageSource PngImageSource(string embeddedPathname)
        {
            System.IO.Stream st = this.GetType().Assembly.GetManifestResourceStream(embeddedPathname);
            var decoder = new System.Windows.Media.Imaging.PngBitmapDecoder(st, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            return decoder.Frames[0];
        }

    }
}
