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
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

#endregion

namespace SystemDotNet.Reporter
{
    partial class MultiSelect : Form
    {


        List<string> selectedkeys = new List<string>();
        public List<string> SelectedKeys { get { return selectedkeys; } }
      //  public bool MatchNames { get { return this.cbMatchNames.Checked; } }

        public MultiSelect(List<string> keys)
        {
            InitializeComponent();
            foreach (string s in keys)
            {
                lNodes.Items.Add(s);
            }
        }

        private void bOk_Click(object sender, EventArgs e)
        {
            foreach (object item in this.lNodes.SelectedItems)
            {
                if(item != null)
                    selectedkeys.Add((string)item);
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void cbMatchNames_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void bCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}