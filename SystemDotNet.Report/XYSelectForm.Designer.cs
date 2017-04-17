namespace SystemDotNet.Reporter
{
    partial class XYSelectForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
            this.lXlabel = new System.Windows.Forms.Label();
            this.cbXaxis = new System.Windows.Forms.ComboBox();
            this.cbYaxis = new System.Windows.Forms.ComboBox();
            this.lYLabel = new System.Windows.Forms.Label();
            this.bOk = new System.Windows.Forms.Button();
            this.bCancel = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // lXlabel
            // 
            this.lXlabel.AutoSize = true;
            this.lXlabel.Location = new System.Drawing.Point(12, 24);
            this.lXlabel.Name = "lXlabel";
            this.lXlabel.Size = new System.Drawing.Size(39, 13);
            this.lXlabel.TabIndex = 0;
            this.lXlabel.Text = "X-Axis:";
            // 
            // cbXaxis
            // 
            this.cbXaxis.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbXaxis.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbXaxis.FormattingEnabled = true;
            this.cbXaxis.Location = new System.Drawing.Point(77, 21);
            this.cbXaxis.Name = "cbXaxis";
            this.cbXaxis.Size = new System.Drawing.Size(300, 21);
            this.cbXaxis.TabIndex = 1;
            // 
            // cbYaxis
            // 
            this.cbYaxis.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbYaxis.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbYaxis.FormattingEnabled = true;
            this.cbYaxis.Location = new System.Drawing.Point(77, 62);
            this.cbYaxis.Name = "cbYaxis";
            this.cbYaxis.Size = new System.Drawing.Size(300, 21);
            this.cbYaxis.TabIndex = 3;
            // 
            // lYLabel
            // 
            this.lYLabel.AutoSize = true;
            this.lYLabel.Location = new System.Drawing.Point(12, 65);
            this.lYLabel.Name = "lYLabel";
            this.lYLabel.Size = new System.Drawing.Size(39, 13);
            this.lYLabel.TabIndex = 2;
            this.lYLabel.Text = "Y-Axis:";
            // 
            // bOk
            // 
            this.bOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bOk.Location = new System.Drawing.Point(302, 99);
            this.bOk.Name = "bOk";
            this.bOk.Size = new System.Drawing.Size(75, 23);
            this.bOk.TabIndex = 4;
            this.bOk.Text = "OK";
            this.bOk.Click += new System.EventHandler(this.bOk_Click);
            // 
            // bCancel
            // 
            this.bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bCancel.Location = new System.Drawing.Point(211, 99);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(75, 23);
            this.bCancel.TabIndex = 5;
            this.bCancel.Text = "Cancel";
            this.bCancel.Click += new System.EventHandler(this.bCancel_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(31, 99);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(103, 17);
            this.checkBox1.TabIndex = 6;
            this.checkBox1.Text = "Show Full Name";
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // XYSelectForm
            // 
            this.ClientSize = new System.Drawing.Size(401, 130);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.bCancel);
            this.Controls.Add(this.bOk);
            this.Controls.Add(this.cbYaxis);
            this.Controls.Add(this.lYLabel);
            this.Controls.Add(this.cbXaxis);
            this.Controls.Add(this.lXlabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "XYSelectForm";
            this.Text = "Select X and Y Axis";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lXlabel;
        private System.Windows.Forms.ComboBox cbXaxis;
        private System.Windows.Forms.ComboBox cbYaxis;
        private System.Windows.Forms.Label lYLabel;
        private System.Windows.Forms.Button bOk;
        private System.Windows.Forms.Button bCancel;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}