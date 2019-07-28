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
                            v.SetElementOverrides(rev.Id, og);
                            v.SetCategoryOverrides(revCloudsCategory.Id, og);
                            v.SetCategoryOverrides(revCloudTagsCategory.Id, og);
                        }
                        else
                        {
                            sb.AppendLine(v.Name + "has hidden clouds");
                        }

                        if (!p.AsValueString().Contains("Sheet"))
                        {
                            viewToCheck.Add(v);
                            sb.AppendLine(v.Name);
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
