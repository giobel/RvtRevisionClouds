#region Namespaces
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
#endregion

namespace RMP
{
    [Transaction(TransactionMode.Manual)]
    public class ExportCloudsInfo : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            

            string outputFile = @"C:\Temp\report.csv";

            StringBuilder sb = new StringBuilder();

            StringBuilder result = new StringBuilder();

            try
            {
                File.WriteAllText(outputFile,
                  "Sheet Number," +
                  "Sheet Name," +
                  "Sheet Revision," +
                  "Cloud Id," +
                  "Is Hidden," +
                  "Is Element Color Override,"+
                  "Is Category Color Override," +
                  "Revision," +
                  "Mark" + Environment.NewLine
                 );
            }
            catch
            {
                TaskDialog.Show("Error", "Opertion cancelled. Please close the log file.");
                return Result.Failed;
            }


            ICollection<Element> allClouds = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_RevisionClouds).WhereElementIsNotElementType().ToElements();

            int cloudsOnSheets = 0;

            foreach (Element rev in allClouds)
            {

                Parameter p = rev.get_Parameter(BuiltInParameter.ELEM_PARTITION_PARAM);
                View currentView = doc.GetElement(rev.OwnerViewId) as View;

                Parameter revision = rev.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS);
                Parameter version = rev.get_Parameter(BuiltInParameter.DOOR_NUMBER);

                try
                {
                    if (currentView is ViewSheet)
                    {

                        ViewSheet sheetView = doc.GetElement(rev.OwnerViewId) as ViewSheet;

                        Parameter sheetRevision = sheetView.get_Parameter("TB_Revision");

                        OverrideGraphicSettings og = sheetView.GetElementOverrides(rev.Id);
                        
                        Category revCloudsCategory = doc.Settings.Categories.get_Item(BuiltInCategory.OST_RevisionClouds);

                        OverrideGraphicSettings ogc = sheetView.GetCategoryOverrides(revCloudsCategory.Id);

                        string isElementOverriden = "";
                        string isCategoryOverriden = "";

                        if (null != og)
                        {
                            if (og.IsValidObject)
                            {
                                if (og.ProjectionLineColor.IsValid)
                                    isElementOverriden = $"{og.ProjectionLineColor.Red.ToString()} {og.ProjectionLineColor.Green.ToString()} {og.ProjectionLineColor.Blue.ToString()}";
                                else
                                    isElementOverriden = "N/A";
                            }
                            else
                                isElementOverriden = "N/A";
                        }

                        if (null != ogc)
                        {
                            if (ogc.IsValidObject)
                            {
                                if (ogc.ProjectionLineColor.IsValid)
                                    isCategoryOverriden = $"{ogc.ProjectionLineColor.Red.ToString()} {ogc.ProjectionLineColor.Green.ToString()} {ogc.ProjectionLineColor.Blue.ToString()}";
                                else
                                    isCategoryOverriden = "N/A";
                            }
                        }
                        else
                            isCategoryOverriden = "N/A";


                        string fixedName = sheetView.Name.Replace(',', '-');

                        sb.AppendLine(String.Format("{0},{1},{2},{3},{4},{5},{6}",
                                                  sheetView.SheetNumber,
                                                  fixedName,
                                                  sheetRevision.AsString(),
                                                  rev.Id,
                                                  rev.IsHidden(sheetView),
                                                  isElementOverriden,
                                                  isCategoryOverriden,
                                                  revision.AsString(),
                                                  version.AsString()));
                        cloudsOnSheets += 1;
                    }
                    else
                    {
                        string fixedName = currentView.Name.Replace(',', '-');

                        sb.AppendLine(String.Format("{0},{1},{2},{3},{4},{5},{6}",
                                                  "N/A",
                                                  fixedName,
                                                  "N/A",
                                                  rev.Id,
                                                  rev.IsHidden(currentView),
                                                  revision.AsString(),
                                                  version.AsString()));

                    }

                }
                catch (Exception ex)
                {
                    result.AppendLine(String.Format("Element {0} \nError Message: {1}", rev.Id, ex.Message));
                }

            }
            File.AppendAllText(outputFile, sb.ToString());



            TaskDialog myDialog = new TaskDialog("Summary");
            myDialog.MainIcon = TaskDialogIcon.TaskDialogIconNone;
            myDialog.MainContent = String.Format("Total Rev Clouds: {0} \nClouds not on Sheets: {1} \nErrors: {2}",

                                                 allClouds.Count, allClouds.Count - cloudsOnSheets, result.ToString());

            myDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink4, $"Open Log File {outputFile}", "");

            TaskDialogResult res = myDialog.Show();

            if (TaskDialogResult.CommandLink4 == res)

            {

                System.Diagnostics.Process process =

                  new System.Diagnostics.Process();

                process.StartInfo.FileName = outputFile;

                process.Start();

            }

            return Result.Succeeded;
        }
    }
}
