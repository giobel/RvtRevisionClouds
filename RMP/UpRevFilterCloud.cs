using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Linq;
using System.Collections;

namespace RMP
{
    [Transaction(TransactionMode.Manual)]
    public class UpRevFilterCloud : IExternalCommand
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


            ICollection<Element> fec = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Sheets).WhereElementIsNotElementType().ToElements();

            List<Parameter> cloudParams = new List<Parameter>();

            IEnumerator cloudsParamEnum = fec.First().ParametersMap.GetEnumerator();

            while (cloudsParamEnum.MoveNext())
            {
                cloudParams.Add(cloudsParamEnum.Current as Parameter);
            }

            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();

            

            foreach (Element item in fec)
            {
                foreach (Parameter p in cloudParams)
                {
                    try
                    {
                        Parameter currentParam = item.get_Parameter(p.Definition);

                        if (!result.Keys.Contains(currentParam.Definition.Name))
                        {
                            result.Add(currentParam.Definition.Name, new List<string>() { currentParam.AsString() });
                        }
                        else
                        {
                            if (!result[currentParam.Definition.Name].Contains(currentParam.AsString()))
                                result[currentParam.Definition.Name].Add(currentParam.AsString());
                        }
                    }
                    catch { }
                }
                
            }


            //foreach (Element item in fec)
            //{
            //    Parameter p = item.get_Parameter("TB_Revision");

            //    if (!result.Keys.Contains(p.Definition.Name))
            //    {
            //        result.Add(p.Definition.Name, new List<string>() { p.AsString() });
            //    }
            //    else
            //    {
            //        if (!result[p.Definition.Name].Contains(p.AsString()))
            //            result[p.Definition.Name].Add(p.AsString());
            //    }

            //}


            using (var form = new Form1(result))
            {
                form.ShowDialog();
            }

            return Result.Succeeded;
        }
    }
}