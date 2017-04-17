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
using System.Text;
using System.Windows.Forms;
using SystemDotNet.PostProcessing;
using NextGenLab.Chart;

#endregion

namespace SystemDotNet.Reporter
{
    public class ColumnNode:TreeNode
    {
        ReportNode parent;

        public ColumnNode(ReportNode parent, string name)
        {
            this.parent = parent;
            this.Text = name;
            this.ImageIndex = 0;
            this.SelectedImageIndex = 0;
            this.StateImageIndex = 0;

           
        }

        void ContextMenu_Popup(object sender, EventArgs e)
        {
            
        }
    }
}
