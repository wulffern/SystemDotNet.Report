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
using System.IO;
using NextGenLab.Chart;

#endregion

namespace SystemDotNet.Reporter
{
    public class ReportNode:TreeNode
    {
        
        string defaultXAxis = "Time";

        public string DefaultXAxis { get { return defaultXAxis; } set { defaultXAxis = value; } }

       

        Report report;
        public Report Report { get { return report; } }

        Dictionary<string, List<double>> plots;
        Form mdiContainer;
        bool singlewindow = false;

        List<string> plotKeys = new List<string>();
        public List<string> PlotKeys { get { return plotKeys; } set { plotKeys = value; } }

        public bool SingleWindow { get { return singlewindow; } set { singlewindow = value; } }

        string currentname;
        public string CurrentName { get { return currentname; } set { currentname = value; } }

        public Form MDIContainer { get { return mdiContainer; } }

        public event ProgressHandler UpdatedProgess;

        public ReportNode(Form mdiContainer, string Title, OutputReaderBase reader, bool showFullName)
        {
            this.mdiContainer = mdiContainer;

            LoadReader(reader,Title,showFullName);
        }


        void LoadReader(OutputReaderBase reader,string filename, bool showFullName)
        {
            report = new Report(null, reader);
            //report.NewChartData +=new Report.ChartDataHandler(report_NewChartData);
            report.OutputReader.UpdatedProgress += new ProgressHandler(OutputReader_UpdatedProgress);

            plots = new Dictionary<string, List<double>>();

            List<string> keys = report.OutputReader.ReadHeader();

            if (keys.Count > 0 && reader.GetType() == typeof(ORBChartDataReader))
                defaultXAxis = keys[0];

            foreach (string k in keys)
            {

                if (!plots.ContainsKey(k))
                {
                    plots.Add(k, null);

                    plotKeys.Add(k);
                }
            }


            NodeContextMenu ncm = new NodeContextMenu(this);

            ColumnNode cn;
            string sn = "";
            MenuItem midefault;
            foreach (string s in plots.Keys)
            {
                if (!showFullName)
                    sn = s.Remove(0, s.LastIndexOf('.') + 1);
                else
                    sn = s;

                cn = new ColumnNode(this, sn);


                midefault = new MenuItem("Default X-Axis",new EventHandler(OnSelectXaxis));

                if (s == defaultXAxis)
                {
                    cn.Checked = true;
                    midefault.Checked = true;
                }

                cn.ContextMenu = new ContextMenu();
                
                cn.ContextMenu.Tag = s;
                cn.ContextMenu.MenuItems.AddRange(ncm.GetMenuItems());
                cn.ContextMenu.MenuItems.Add(midefault);
                cn.ContextMenu.Popup += new EventHandler(ContextMenu_Popup);
                this.Nodes.Add(cn);
            }

            string filen = Path.GetFileNameWithoutExtension(filename);
            int index;
            long ticks;
            if ((index = filen.IndexOf('_')) > -1)
            {
                if (long.TryParse(filen.Substring(0, index), out ticks))
                {
                    DateTime dt = new DateTime(ticks);
                    filen = filen.Substring(index + 1) + " (" + dt.ToShortDateString() + " " + dt.ToShortTimeString() + ")";
                }
            }

            this.Text = filen;
            this.ToolTipText = DateTime.Now.ToLongTimeString();
           
            this.Expand();
            this.SelectedImageIndex = 2;
            this.StateImageIndex = 2;
            this.ImageIndex = 2;

            ContextMenu cm = new ContextMenu();
            cm.MenuItems.Add(new MenuItem("Close", delegate { this.Remove(); }));
            this.ContextMenu = cm;

        }

        void OutputReader_UpdatedProgress(int progress)
        {
            mdiContainer.Invoke(new ProgressHandler(UpdateProgessEvent), progress);
           // throw new Exception("The method or operation is not implemented.");
        }

        void UpdateProgessEvent(int progress)
        {
            if (UpdatedProgess != null)
                UpdatedProgess(progress);
        }

        public List<double> GetPlot(string name)
        {
            if (name == null)
                return new List<double>();

            if (plots.ContainsKey(name))
            {
                if (plots[name] == null)
                {
                    plots[name] = report.OutputReader.Read(name);
                    return plots[name];
                }
                else

                   return plots[name];
                
            }
            else
                return new List<double>();
        }

        public void ClearPlots()
        {
            List<string> keys = new List<string>();
            keys.AddRange(plots.Keys);
            for (int i = 0; i < keys.Count; i++)
            {
                plots[keys[i]] = null;
            }
        }

        public void Plot(List<NextGenLab.Chart.ChartData> cds)
        {
            if (singlewindow)
            {
                Chart ch = new Chart();
                foreach (ChartData cd in cds)
                {
                    ch.Open(cd, true);
                 ch.MdiParent = mdiContainer;
                 ch.Show();
                }

            }
            else
            {
                Chart ch = null;
                foreach (ChartData cd in cds)
                {
                    ch = new Chart();
                    ch.Open(cd, false);
                    ch.MdiParent = mdiContainer;
                    ch.Show();

                }
            }

          

           // mdiContainer.
        }



        

        void OnSelectXaxis(object sender, EventArgs e)
        {

             }

        void ContextMenu_Popup(object sender, EventArgs e)
        {
           currentname = (string)((ContextMenu)sender).Tag;
        }
    }
}
