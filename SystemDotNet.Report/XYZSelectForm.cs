/*
Copyright (C) 2005 Carsten Wulff

This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the

GNU General Public License for more details.
You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SystemDotNet.Reporter
{
    internal class XYZSelectForm : System.Windows.Forms.Form
    {
        private Button bCancel;
        private Button bOk;
        private ComboBox cbYaxis;
        private Label label2;
        private ComboBox cbXaxis;
        private Label label1;
        private ComboBox cbZaxis;
        private Label label3;
        private CheckBox checkBox1;
        private System.ComponentModel.IContainer components = null;

        private string xaxis;

        public string XAxis
        {
            get { return xaxis; }
            set { xaxis = value; }
        }

        private string yaxis;

        public string YAxis
        {
            get { return yaxis; }
            set { yaxis = value; }
        }

        private string zaxis;

        public string ZAxis
        {
            get { return zaxis; }
            set { zaxis = value; }
        }


        List<string> keys;

        public XYZSelectForm(List<string> keys)
            : this()
        {

            this.keys = keys;
            LoadData();
        }

        public XYZSelectForm() { InitializeComponent(); }

        void LoadData()
        {
            string[] trkeys = RemoveFullName(keys.ToArray());
            cbXaxis.Items.AddRange(trkeys);
            cbYaxis.Items.AddRange(trkeys);
            cbZaxis.Items.AddRange(trkeys);

            if (cbXaxis.Items.Count > 0)
                cbXaxis.SelectedIndex = 0;
            if (cbYaxis.Items.Count > 0)
                cbYaxis.SelectedIndex = 0;
            if (cbZaxis.Items.Count > 0)
                cbZaxis.SelectedIndex = 0;
        }

        private void bCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void bOk_Click(object sender, EventArgs e)
        {
            if (cbXaxis.SelectedIndex > -1 && cbXaxis.SelectedIndex < keys.Count
                && cbYaxis.SelectedIndex > -1 && cbYaxis.SelectedIndex < keys.Count 
                && cbZaxis.SelectedIndex > -1 && cbZaxis.SelectedIndex < keys.Count)
            {
                xaxis = keys[cbXaxis.SelectedIndex];
                yaxis = keys[cbYaxis.SelectedIndex];
                zaxis = keys[cbZaxis.SelectedIndex];
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        string[] RemoveFullName(string[] strs)
        {
            if (checkBox1.Checked)
                for (int i = 0; i < strs.Length; i++)
                    strs[i] = strs[i].Remove(0, strs[i].LastIndexOf('.') + 1);


            return strs;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            cbXaxis.Items.Clear();
            cbYaxis.Items.Clear();
            cbZaxis.Items.Clear();
            LoadData();
        }

        #region Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.bCancel = new System.Windows.Forms.Button();
            this.bOk = new System.Windows.Forms.Button();
            this.cbYaxis = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbXaxis = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbZaxis = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // bCancel
            // 
            this.bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bCancel.Location = new System.Drawing.Point(207, 136);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(75, 23);
            this.bCancel.TabIndex = 12;
            this.bCancel.Text = "Cancel";
            this.bCancel.Click += new System.EventHandler(this.bCancel_Click);
            // 
            // bOk
            // 
            this.bOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bOk.Location = new System.Drawing.Point(298, 136);
            this.bOk.Name = "bOk";
            this.bOk.Size = new System.Drawing.Size(75, 23);
            this.bOk.TabIndex = 11;
            this.bOk.Text = "Ok";
            this.bOk.Click += new System.EventHandler(this.bOk_Click);
            // 
            // cbYaxis
            // 
            this.cbYaxis.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbYaxis.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbYaxis.FormattingEnabled = true;
            this.cbYaxis.Location = new System.Drawing.Point(74, 53);
            this.cbYaxis.Name = "cbYaxis";
            this.cbYaxis.Size = new System.Drawing.Size(300, 21);
            this.cbYaxis.TabIndex = 10;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Y-Axis:";
            // 
            // cbXaxis
            // 
            this.cbXaxis.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbXaxis.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbXaxis.FormattingEnabled = true;
            this.cbXaxis.Location = new System.Drawing.Point(74, 12);
            this.cbXaxis.Name = "cbXaxis";
            this.cbXaxis.Size = new System.Drawing.Size(300, 21);
            this.cbXaxis.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "X-Axis:";
            // 
            // cbZaxis
            // 
            this.cbZaxis.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbZaxis.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbZaxis.FormattingEnabled = true;
            this.cbZaxis.Location = new System.Drawing.Point(74, 93);
            this.cbZaxis.Name = "cbZaxis";
            this.cbZaxis.Size = new System.Drawing.Size(299, 21);
            this.cbZaxis.TabIndex = 15;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(27, 96);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Z-Axis:";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(27, 136);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(99, 17);
            this.checkBox1.TabIndex = 13;
            this.checkBox1.Text = "Show Full Name";
            this.checkBox1.Click += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // XYZSelectForm
            // 
            this.ClientSize = new System.Drawing.Size(403, 167);
            this.Controls.Add(this.cbZaxis);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.bCancel);
            this.Controls.Add(this.bOk);
            this.Controls.Add(this.cbYaxis);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbXaxis);
            this.Controls.Add(this.label1);
            this.Name = "XYZSelectForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion
    }
}

