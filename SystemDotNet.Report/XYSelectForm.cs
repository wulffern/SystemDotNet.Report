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
#region Using directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

#endregion

namespace SystemDotNet.Reporter
{
    partial class XYSelectForm : Form
    {
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


        List<string> keys;
        string xlabel = "X-Axis:";
        string ylabel = "Y-Axis:";

        public XYSelectForm(string xlabel,string ylabel,List<string> keys):this()
        {
            this.xlabel = xlabel;
            this.ylabel = ylabel;
            this.lXlabel.Text = xlabel; this.lYLabel.Text = ylabel;
            this.keys = keys;
            LoadData();
        }

        public XYSelectForm() { InitializeComponent(); ; }

        void LoadData()
        {
            string[] trkeys = RemoveFullName(keys.ToArray());
            cbXaxis.Items.AddRange(trkeys);
            cbYaxis.Items.AddRange(trkeys);

            if (cbXaxis.Items.Count > 0)
                cbXaxis.SelectedIndex = 0;
            if (cbYaxis.Items.Count > 0)
                cbYaxis.SelectedIndex = 0;
        }

        private void bCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void bOk_Click(object sender, EventArgs e)
        {
            if (cbXaxis.SelectedIndex > -1 && cbXaxis.SelectedIndex < keys.Count
                && cbYaxis.SelectedIndex > -1 && cbYaxis.SelectedIndex < keys.Count)
            {
                xaxis = keys[cbXaxis.SelectedIndex];
                yaxis = keys[cbYaxis.SelectedIndex];
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
            LoadData();
        }

    }
}