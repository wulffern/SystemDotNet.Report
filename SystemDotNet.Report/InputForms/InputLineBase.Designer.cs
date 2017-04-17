namespace SystemDotNet.Reporter.InputForms
{
    partial class InputLineBase
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tValue = new System.Windows.Forms.TextBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.lText = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // tValue
            // 
            this.tValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.tValue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tValue.Location = new System.Drawing.Point(222, 3);
            this.tValue.Name = "tValue";
            this.tValue.Size = new System.Drawing.Size(100, 20);
            this.tValue.TabIndex = 3;
            this.tValue.Leave += new System.EventHandler(this.CheckValue);
            // 
            // comboBox1
            // 
            this.comboBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "None",
            "K",
            "Meg",
            "G",
            "T"});
            this.comboBox1.Location = new System.Drawing.Point(329, 3);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(58, 21);
            this.comboBox1.TabIndex = 4;
            // 
            // lText
            // 
            this.lText.AutoSize = true;
            this.lText.Location = new System.Drawing.Point(12, 6);
            this.lText.Name = "lText";
            this.lText.Size = new System.Drawing.Size(126, 13);
            this.lText.TabIndex = 5;
            this.lText.Text = "Sampling Frequency (Fs):";
            // 
            // InputLineBase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.tValue);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.lText);
            this.Name = "InputLineBase";
            this.Size = new System.Drawing.Size(400, 26);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tValue;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label lText;
    }
}
