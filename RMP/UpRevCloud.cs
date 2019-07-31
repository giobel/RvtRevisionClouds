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
    public class UpRevCloud: IExternalCommand
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

            List<BuiltInCategory> builtInFilter = new List<BuiltInCategory>();

            builtInFilter.Add(BuiltInCategory.OST_RevisionClouds);
            //builtInFilter.Add(BuiltInCategory.OST_RevisionCloudTags);


            ElementMulticategoryFilter filterRevision = new ElementMulticategoryFilter(builtInFilter);

            //ICollection<Element> fec = new FilteredElementCollector(doc).WherePasses(filterRevision).WhereElementIsNotElementType().ToElements();

            ICollection<ElementId> fec = uidoc.Selection.GetElementIds();

            string outputFile = @"C:\Temp\reportUprev.csv";

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
                  "Current Revision," +
                  "Error" + Environment.NewLine
                 );
            }
            catch
            {
                TaskDialog.Show("Error", "Opertion cancelled. Please close the log file.");
                return Result.Failed;
            }

            int hiddenClouds = 0;
            int changedClouds = 0;
            int cloudsNotOnSheet = 0;
            int errors = 0;

            using (Transaction t = new Transaction(doc, "Reset revision cloud color"))
            {
                t.Start();

                foreach (ElementId revId in fec)
                {
                    Element rev = doc.GetElement(revId);
                    View v = doc.GetElement(rev.OwnerViewId) as View;
                    string fixedName = v.Name.Replace(',', '-');
                    string currentRevision = "N/A";
                    string sheetNumber = "N/A";

                    if (rev.IsHidden(v))
                    {
                        hiddenClouds += 1;
                    }

                    try
                    {
                        if (v is ViewSheet)
                        {
                            ViewSheet sheetView = doc.GetElement(rev.OwnerViewId) as ViewSheet;
                            sheetNumber = sheetView.SheetNumber;
                            

                            Parameter sheetRevision = sheetView.get_Parameter("TB_Revision");
                            Parameter p = rev.get_Parameter(BuiltInParameter.ELEM_PARTITION_PARAM);

                            if (!rev.IsHidden(v))
                            {
                                Parameter revision = rev.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS);

                                Parameter version = rev.get_Parameter(BuiltInParameter.DOOR_NUMBER);

                                if (revision != null)
                                {
                                    currentRevision = revision.AsString();
                                }

                                if (currentRevision != null)
                                {
                                    int last = Int32.Parse(sheetRevision.AsString()); //sheet revision, not cloud revision
                                    revision.Set("00" + last.ToString());
                                    version.Set("01");
                                }
                                changedClouds += 1;
                                sb.AppendLine(String.Format("{0},{1},{2},{3},{4},{5},{6}",
                                                      sheetNumber,
                                                      fixedName,
                                                      sheetRevision.AsString(),
                                                      rev.Id,
                                                      rev.IsHidden(v),
                                                      rev.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).AsString(),
                                                      "No"
                                                      ));
                            }
                        }
                        else
                        {
                            cloudsNotOnSheet += 1;
                            sb.AppendLine(String.Format("{0},{1},{2},{3},{4},{5},{6}",
                                                      sheetNumber,
                                                      fixedName,
                                                      "N/A",
                                                      rev.Id,
                                                      rev.IsHidden(v),
                                                      currentRevision,
                                                      "N/A"
                                                      ));
                        }
                    }
                    catch (Exception ex)
                    {
                        errors += 1;
                        string str = ex.Message;
                        str = str.Replace("\n", String.Empty);
                        str = str.Replace("\r", String.Empty);
                        str = str.Replace("\t", String.Empty);

                        sb.AppendLine(String.Format("{0},{1},{2},{3},{4},{5},{6}",
                                                  sheetNumber,
                                                  fixedName,
                                                  "N/A",
                                                  rev.Id,
                                                  rev.IsHidden(v),
                                                  currentRevision,
                                                  str
                                                  ));
                    }
                }

                t.Commit();
            }

            File.AppendAllText(outputFile, sb.ToString());



            TaskDialog myDialog = new TaskDialog("Summary");
            myDialog.MainIcon = TaskDialogIcon.TaskDialogIconNone;
            //myDialog.MainContent = String.Format("Rev Clouds: {0} \nClouds not on sheets: {1}\nClouds hidden (not uprev): {2} \nErrors: {3}", fec.Count, cloudsNotOnSheet, hiddenClouds, errors);
            myDialog.MainContent = String.Format("Rev Clouds updated: {0}", fec.Count);

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