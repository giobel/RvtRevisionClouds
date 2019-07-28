using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RMP
{
    [Transaction(TransactionMode.Manual)]
    public class ResetOverrides : IExternalCommand
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




            string outputFile = @"C:\Temp\reportOverrides.csv";

            try
            {
                File.WriteAllText(outputFile,
                    "Revision Cloud Id," +
                    "Type,"+
                    "View Name,"+
                  "Hidden," +
                  "Error"+ Environment.NewLine
                 );
            }
            catch
            {
                TaskDialog.Show("Error", "Opertion cancelled. Please close the log file.");
                return Result.Failed;
            }

            List<BuiltInCategory> builtInFilter = new List<BuiltInCategory>();

            builtInFilter.Add(BuiltInCategory.OST_RevisionClouds);
            builtInFilter.Add(BuiltInCategory.OST_RevisionCloudTags);


            ElementMulticategoryFilter filterRevision = new ElementMulticategoryFilter(builtInFilter);

            ICollection<Element> fec = new FilteredElementCollector(doc).WherePasses(filterRevision).WhereElementIsNotElementType().ToElements();
            
            OverrideGraphicSettings og = new OverrideGraphicSettings();

            //Category revCloudsCategory = doc.Settings.Categories.get_Item("Revision Clouds");

            Category revCloudsCategory = doc.Settings.Categories.get_Item(BuiltInCategory.OST_RevisionClouds);

            Category revCloudTagsCategory = doc.Settings.Categories.get_Item(BuiltInCategory.OST_RevisionCloudTags);

            StringBuilder sb = new StringBuilder();

            ICollection<Element> allViews = new FilteredElementCollector(doc).OfClass(typeof(View)).
                WhereElementIsNotElementType().ToElements();

            List<View> viewToCheck = new List<View>();

            int totalRevClouds = fec.Count;

            int revisionCloudChanged = 0;
            int cloudsCategoryChanged = 0;
            int cloudsTagsCategoryChanged = 0;

            using (Transaction t = new Transaction(doc, "Reset revision cloud color"))
            {
                t.Start();

                foreach (Element rev in fec) //clouds + tags
                {
                    View v = doc.GetElement(rev.OwnerViewId) as View;
                    try
                    {
                        Parameter p = rev.get_Parameter(BuiltInParameter.ELEM_PARTITION_PARAM);
                        //View v = doc.GetElement(rev.OwnerViewId) as View;

                        if (!rev.IsHidden(v))
                        {
                            v.SetElementOverrides(rev.Id, og);
                            revisionCloudChanged += 1;
                            
                            if (rev.Category.Name == "Revision Clouds")
                            {
                                v.SetCategoryOverrides(revCloudsCategory.Id, og);
                                cloudsCategoryChanged += 1;
                            }

                            if (rev.Category.Name == "Revision Clouds Tags")
                            {
                                v.SetCategoryOverrides(revCloudTagsCategory.Id, og);
                                cloudsTagsCategoryChanged += 1;
                            }
                        }                       
                        //sb.AppendLine($"{rev.Id}," +
                        //                $"{rev.Category.Name}," +
                        //                $"{v.Name}," +
                        //                $"{rev.IsHidden(v)}," +
                        //                $"Success");
                        
                        //if (!p.AsValueString().Contains("Sheet"))
                        //{
                        //    viewToCheck.Add(v);
                        //    sb.AppendLine($"{v.Name}");
                        //}

                    }
                    catch (Exception ex)
                    {

                        string str = ex.Message;
                        str = str.Replace("\n", String.Empty);
                        str = str.Replace("\r", String.Empty);
                        str = str.Replace("\t", String.Empty);

                        string viewName = v.Name.Replace(',','-');

                        sb.AppendLine($"{rev.Id}," +
                                        $"{rev.Category.Name}," +
                                       $"{viewName}," +
                                       $"N/A," +
                                       $"{str}");
                    }
                }

                t.Commit();
            }

            File.AppendAllText(outputFile, sb.ToString());

            TaskDialog myDialog = new TaskDialog("Summary");
            myDialog.MainIcon = TaskDialogIcon.TaskDialogIconWarning;
            
            myDialog.MainContent = $"Total Revision Cloud {totalRevClouds}\n" +
                                    $"Rev Clouds changed: {revisionCloudChanged}\n" +
                                    $"Rev Clouds Category changed: {cloudsCategoryChanged}\n" +
                                    $"Rev Clouds Tags Category changed: {cloudsTagsCategoryChanged}";

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
