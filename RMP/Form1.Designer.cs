namespace RMP
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cBoxParameters = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cBoxValues = new System.Windows.Forms.ComboBox();
            this.buttonUpdate = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cBoxParameters
            // 
            this.cBoxParameters.FormattingEnabled = true;
            this.cBoxParameters.Location = new System.Drawing.Point(12, 35);
            this.cBoxParameters.Name = "cBoxParameters";
            this.cBoxParameters.Size = new System.Drawing.Size(314, 21);
            this.cBoxParameters.Sorted = true;
            this.cBoxParameters.TabIndex = 0;
            this.cBoxParameters.SelectedIndexChanged += new System.EventHandler(this.cBoxParameters_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(139, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Revision Clouds Parameters";
            // 
            // cBoxValues
            // 
            this.cBoxValues.FormattingEnabled = true;
            this.cBoxValues.Location = new System.Drawing.Point(12, 86);
            this.cBoxValues.Name = "cBoxValues";
            this.cBoxValues.Size = new System.Drawing.Size(314, 21);
            this.cBoxValues.TabIndex = 4;
            // 
            // buttonUpdate
            // 
            this.buttonUpdate.Location = new System.Drawing.Point(80, 128);
            this.buttonUpdate.Name = "buttonUpdate";
            this.buttonUpdate.Size = new System.Drawing.Size(177, 28);
            this.buttonUpdate.TabIndex = 5;
            this.buttonUpdate.Text = "Up-rev Revision Clouds";
            this.buttonUpdate.UseVisualStyleBackColor = true;
            this.buttonUpdate.Click += new System.EventHandler(this.buttonUpdate_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Values";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(349, 176);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonUpdate);
            this.Controls.Add(this.cBoxValues);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cBoxParameters);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Up-rev Revision Clouds";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cBoxParameters;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cBoxValues;
        private System.Windows.Forms.Button buttonUpdate;
        private System.Windows.Forms.Label label2;
    }
}