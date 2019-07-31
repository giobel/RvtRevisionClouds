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

        private void buttonUpdate_Click(object sender, System.EventArgs e)
        {
            Autodesk.Revit.UI.TaskDialog.Show("Warning", "Command not implemented yet");
        }
    }
}
