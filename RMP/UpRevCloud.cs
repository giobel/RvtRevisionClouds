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
            builtInFilter.Add(BuiltInCategory.OST_RevisionCloudTags);


            ElementMulticategoryFilter filterRevision = new ElementMulticategoryFilter(builtInFilter);

            ICollection<Element> fec = new FilteredElementCollector(doc).WherePasses(filterRevision).WhereElementIsNotElementType().ToElements();
            
            StringBuilder sb = new StringBuilder();
            
            using (Transaction t = new Transaction(doc, "Reset revision cloud color"))
            {
                t.Start();

                foreach (Element rev in fec)
                {
                    try
                    {
                        Parameter p = rev.get_Parameter(BuiltInParameter.ELEM_PARTITION_PARAM);
                        View v = doc.GetElement(rev.OwnerViewId) as View;

                        if (!rev.IsHidden(v))
                        {
                            Parameter revision = rev.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS);

                            Parameter version = rev.get_Parameter(BuiltInParameter.DOOR_NUMBER);

                            string currentRevision = null;

                            if (revision != null)
                            {
                                currentRevision = revision.AsString();
                            }


                            if (currentRevision != null)
                            {
                                int last = Int32.Parse(currentRevision) + 1;
                                revision.Set("00" + last.ToString());
                                version.Set("01");
                            }

                        }
                        else
                        {
                            sb.AppendLine(v.Name + " has hidden clouds");
                        }

                    }
                    catch (Exception ex)
                    {

                        sb.AppendLine("Error: ElementId " + rev.Id.ToString() + Environment.NewLine + ex.Message);
                    }
                }

                t.Commit();
            }

            TaskDialog.Show("result", sb.ToString());

            return Result.Succeeded;
        }
    }
}