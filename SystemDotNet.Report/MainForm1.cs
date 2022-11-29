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
using System.IO;
using System.Globalization;
using SystemDotNet.PostProcessing;
using System.Text.RegularExpressions;
using NextGenLab.Chart;

#endregion

namespace SystemDotNet.Reporter
{
    public partial class MainForm1 : Form
    {
        RegExReaderData rerd = null;
        bool guessFileFormat = true;
        public MainForm1()
        {
            InitializeComponent();

            foreach (string s in RegExReaderFactory.GetNames())
            {
                textFileFormatToolStripMenuItem.DropDownItems.Add(s, null, new EventHandler(UpdateReRDer));

            }

            string version;
            try
            {
                version = this.GetType().Assembly.FullName.Split(',')[1].Split('=')[1];
                this.Text += "(" + version + ")";
            }
            catch { }

            if (textFileFormatToolStripMenuItem.DropDownItems.Count > 0)
                ((ToolStripMenuItem)textFileFormatToolStripMenuItem.DropDownItems[0]).Checked = true;

            NumberFormatInfo nfi = NumberFormatInfo.CurrentInfo;

            if (nfi.NumberDecimalSeparator == ",")
                MessageBox.Show("Your decimal separator is set to \",\" this may cause problems when reading files", "Region Settings Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        public MainForm1(string[] files)
            : this()
        {

            foreach (string f in files)
            {
                Open(f);
            }



        }

        [STAThread]
        static void Main(string[] args)
        {
            //Application.EnableVisualStyles();


            if (args.Length > 0)
            {
                Application.Run(new MainForm1(args));
            }
            else
            {
                Application.Run(new MainForm1());
            }
        }

        private void PlotNode(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node is ColumnNode)
            {
                if (e.Node.Parent is ReportNode)
                {
                    ((ReportNode)e.Node.Parent).CurrentName = (string)e.Node.ContextMenu.Tag;
                    e.Node.ContextMenu.MenuItems["General"].MenuItems["Just Plot"].PerformClick();
                }
            }
        }

        private void UpdateReRDer(object sender, EventArgs e)
        {
            ToolStripMenuItem ob = (ToolStripMenuItem)sender;

            if (ob.Text == "Guess")
            {
                guessFileFormat = true;
            }
            else if (ob.Text == "Fast CSV (;)")
            {
                rerd = null;
                guessFileFormat = false;
            }
            else
            {
                guessFileFormat = false;
                rerd = RegExReaderFactory.GetReaderData(ob.Text);
            }

            foreach (ToolStripMenuItem obb in textFileFormatToolStripMenuItem.DropDownItems)
            {
                obb.Checked = false;
            }
            ob.Checked = true;

        }

        private void treeView1_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                object[] files = (object[])e.Data.GetData(DataFormats.FileDrop);
                Open(files);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception while dropping file", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void treeView1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        void Open(object[] files)
        {
            foreach (string f in files)
            {
                Open(f);
            }
        }
        void Open(string file)
        {
            try
            {
                OutputReaderBase reader;
                switch (Path.GetExtension(file))
                {
                    case ".xnc":
                    case ".xnmc":
                    case ".tsv":
                        reader = new TsvReader(file);
                        OpenReportNode(file, reader);
                        break;
                    case ".bnc":
                        NextGenLab.Chart.Chart c = new NextGenLab.Chart.Chart();
                        c.MdiParent = this;
                        c.Open(file, false);
                        c.Show();
                        break;
                    case ".bsdr":
                        reader = new ORBBinaryReader(file, true);
                        OpenReportNode(file,reader);
                        break;
                    default:

                        if (guessFileFormat)
                            rerd = RegExReaderData.GuessFormat(file);

                        if (rerd != null)
                        {
                            
                            reader = new ORBRegExReader(file, true, rerd);
                        }
                        else
                        {
                            reader = new ORBCsvReader(file, true);
                        }
                        OpenReportNode(file,reader);

                        
                        break;

                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception while opening file " + Path.GetFileName(file), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void OpenReportNode(string filename, OutputReaderBase reader)
        {
            ReportNode rp = new ReportNode(this, filename, reader, fullNameToolStripMenuItem.Checked);
            rp.UpdatedProgess += new ProgressHandler(rp_UpdatedProgess);
            this.treeView1.Nodes.Add(rp);
        }

        public void Open(string Title,List<ChartData> cds)
        {
            ORBChartDataReader cdr = new ORBChartDataReader(cds);

            ReportNode rp = new ReportNode(this, Title,cdr,true);
            rp.UpdatedProgess += new ProgressHandler(rp_UpdatedProgess);
            this.treeView1.Nodes.Add(rp);
        }

        delegate void OpenDelegate(string Title, List<ChartData> cds);

        public void OpenFromThread(string Title, List<ChartData> cds)
        {
            this.Invoke(new OpenDelegate(Open), Title, cds);
        }

        void rp_UpdatedProgess(int progress)
        {
            this.toolStripProgressBar1.Value = progress;
        }
        private void fullNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fullNameToolStripMenuItem.Checked = !fullNameToolStripMenuItem.Checked;
        }

        private void EhRefresh(object sender, EventArgs e)
        {
            foreach (TreeNode tn in treeView1.Nodes)
            {
                if (tn is ReportNode)
                {
                    ((ReportNode)tn).ClearPlots();

                }
            }
        }
        private void EhNewChartWindow(object sender, EventArgs e)
        {
            NextGenLab.Chart.Chart c = new NextGenLab.Chart.Chart();
            c.MdiParent = this;
            c.Show();
        }
        private void EhAlignHorizontal(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void EhAlignVertical(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.TileVertical);
        }

        private void EhAlignCascade(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.Cascade);
        }

        private void EhCloseAll(object sender, EventArgs e)
        {
            treeView1.Nodes.Clear();
        }
        private void EhOpen(object sender, EventArgs e)
        {

            if (DialogResult.OK == this.openFileDialog1.ShowDialog())
            {
                foreach (string filename in this.openFileDialog1.FileNames)
                {
                    Open(filename);
                }
            }
        }

        private void EhCreateFileFormat_Click(object sender, EventArgs e)
        {
            FileFormatForm ff = new FileFormatForm();
            ff.Show();
        }

    }
}