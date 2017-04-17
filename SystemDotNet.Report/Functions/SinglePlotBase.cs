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
using NextGenLab.Chart;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using SystemDotNet.PostProcessing;

#endregion

namespace SystemDotNet.Reporter
{
    public abstract class SinglePlotBase
    {
        protected string Name = "";
        protected string PlotType = "";
        bool matchNames;

        public bool MatchNames
        {
            get { return matchNames; }
            set { matchNames = value; }
        }
        private ReportNode reportnode;
        SortedDictionary<string, SortedDictionary<string, EventHandler>> eventhandlers = new SortedDictionary<string, SortedDictionary<string, EventHandler>>();
        public SortedDictionary<string, SortedDictionary<string, EventHandler>> EventHandlers { get { return eventhandlers; } }
        public SinglePlotBase(ReportNode rp)
        {
            reportnode = rp;
            reportnode.SingleWindow = false;
        }

        public void AddMenuItem(string Category, string Title, EventHandler ev)
        {
            if (!eventhandlers.ContainsKey(Category))
                eventhandlers.Add(Category, new SortedDictionary<string, EventHandler>());

            eventhandlers[Category].Add(Title, delegate { matchNames = false; ev(null, null); });
            eventhandlers[Category].Add(Title + " (All)", delegate { matchNames = true; ev(null, null); });
        }

        public void Plot()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(DoPlot));
        }

        void DoPlot(object state)
        {
            Name = reportnode.CurrentName;
            List<ChartData> cdsa = new List<ChartData>();
            List<ChartData> cds = null;
            ChartData cd;
            int index = 1;
            if (MatchNames)
            {
                reportnode.SingleWindow = true;
                foreach (TreeNode tn in reportnode.TreeView.Nodes)
                {
                    if (tn is ReportNode)
                    {
                        ReportNode rp = (ReportNode)tn;
                        cds = OnPlot(rp);
                        if (cds != null)
                        {
                            for (int i = 0; i < cds.Count; i++)
                            {
                                cd = cds[i];
                                for (int z = 0; z < cd.TitlesY.Length; z++)
                                {
                                    cd.TitlesY[z] = cd.TitlesY[z] + ":" + index;
                                }
                                cds[i] = cd;
                            }


                            cdsa.AddRange(cds);
                        }
                        index++;
                    }

                }
            }
            else
            {
                cds = OnPlot(reportnode);
                if (cds != null)
                    cdsa.AddRange(cds);
            }

            for (int i = 0; i < cdsa.Count; i++)
            {
                cd = cdsa[i];
                if (PlotType == "")
                    cd.Title = reportnode.Text;
                else
                    cd.Title = PlotType;

               
                cd.AutoScale = true;
                cdsa[i] = cd;

            }

            if (reportnode.MDIContainer != null)
            {
                reportnode.MDIContainer.Invoke(new ChartDataHandler(reportnode.Plot), cdsa);
            }


        }

        public abstract List<ChartData> OnPlot(ReportNode rp);

        string Filter(string s)
        {
            int index;
            if ((index = s.LastIndexOf('_')) > -1)
                s = s.Substring(index + 1);
            return s;

        }
    }
}
