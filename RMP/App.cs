#region Namespaces
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Media.Imaging;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
#endregion

namespace RMP
{
    class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication a)
        {

            try
            {
                a.CreateRibbonTab("RMP");

                RibbonPanel tools = a.CreateRibbonPanel("RMP", "Tools");
                
                AddPushButton(tools, "btnExportClouds", "Export All Clouds \nto Excel", "", "pack://application:,,,/RMP;component/Images/Export.png", "RMP.ExportCloudsInfo", "Export Revision Clouds info to Excel.");

                AddPushButton(tools, "btnSelectFromExcel", "Select Clouds\nfrom Excel", "", "pack://application:,,,/RMP;component/Images/Select.png", "RMP.SelectFromExcel", "Copy (Ctrl+c) a number of ElementIds in Excel first, then click this button. The revision clouds matching the Ids will be selected.");
                
                AddPushButton(tools, "btnResetOverrides", "Remove All Clouds \nColor Overrides", "", "pack://application:,,,/RMP;component/Images/Reset.png", "RMP.ResetOverrides", "Remove the color overrides from revision clouds and their tags.");

                AddPushButton(tools, "btnUpRevCloud", "Up-rev All \nRevision Clouds", "", "pack://application:,,,/RMP;component/Images/Uprev.png", "RMP.UpRevCloud", "Update the Revision Cloud to the next Revision");
            }
            catch
            {

            }

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }

        private RibbonPanel GetSetRibbonPanel(UIControlledApplication application, string tabName, string panelName)
        {
            List<RibbonPanel> tabList = new List<RibbonPanel>();

            tabList = application.GetRibbonPanels(tabName);

            RibbonPanel tab = null;

            foreach (RibbonPanel r in tabList)
            {
                if (r.Name.ToUpper() == panelName.ToUpper())
                {
                    tab = r;
                }
            }

            if (tab is null)
                tab = application.CreateRibbonPanel(tabName, panelName);

            return tab;
        }

        private Boolean AddPushButton(RibbonPanel Panel, string ButtonName, string ButtonText, string ImagePath16, string ImagePath32, string dllClass, string Tooltip)
        {

            string thisAssemblyPath = Assembly.GetExecutingAssembly().Location;

            try
            {
                PushButtonData m_pbData = new PushButtonData(ButtonName, ButtonText, thisAssemblyPath, dllClass);

                if (ImagePath16 != "")
                {
                    try
                    {
                        m_pbData.Image = new BitmapImage(new Uri(ImagePath16));
                    }
                    catch
                    {
                        //Could not find the image
                    }
                }
                if (ImagePath32 != "")
                {
                    try
                    {
                        m_pbData.LargeImage = new BitmapImage(new Uri(ImagePath32));
                    }
                    catch
                    {
                        //Could not find the image
                    }
                }

                m_pbData.ToolTip = Tooltip;


                PushButton m_pb = Panel.AddItem(m_pbData) as PushButton;

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
