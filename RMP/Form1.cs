using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;

namespace RMP
{
    public partial class Form1 : System.Windows.Forms.Form
    {

        public Dictionary<string, List<string>> allCoudsParamsAndValues { get; set; }

        public Form1(Dictionary<string, List<string>> results)
        {
            allCoudsParamsAndValues = results;

            InitializeComponent();

            cBoxParameters.DataSource = results.Keys.ToList();

        }

        private void cBoxParameters_SelectedIndexChanged(object sender, System.EventArgs e)
        {

            List<string> selected = allCoudsParamsAndValues[cBoxParameters.SelectedItem as string];
            cBoxValues.Items.Clear();

            foreach (string stringValues in selected)
            {
                cBoxValues.Items.Add(stringValues);
            }


        }
    }
}
