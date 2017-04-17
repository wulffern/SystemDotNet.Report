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

#endregion

namespace SystemDotNet.PostProcessing
{
    public class PlotValueVsTime:IPlotSingle,IPlotMultiple      
    {
        ChartData cd;

        SignalBase[] sigs;

        bool visible = true;
        public bool Visible { get { return visible; } set { visible = value; } }

        public PlotValueVsTime()
        {
            cd = ChartData.GetInstance();
        }

        public PlotValueVsTime(params SignalBase[] sigs)
        {
            this.sigs = sigs;

        }

        #region IPlotSingle Members

        public List<NextGenLab.Chart.ChartData> Plot(List<double> d)
        {
            if (!visible) return new List<ChartData>();

            double[] x = new double[d.Count];
            for (int i = 0; i < x.Length; i++)
            {
                x[i] = i;
            }

            cd.AutoScale = true;
            cd.X = x;
            cd.Y = new double[][] { d.ToArray() };
            cd.ShowFullNameLegend = false;
            List<ChartData> cds = new List<ChartData>();
            cds.Add(cd);
            return cds;
        }

        public string Title
        {
            get
            {
                return cd.Title;
            }

            set
            {
                cd.Title = value;
            }
        }


        #endregion

        #region IPlotMultiple Members

        public List<ChartData> Plot(Dictionary<string, List<double>> vals)
        {
            if (!visible) return new List<ChartData>();

            if (sigs == null)
                return new List<ChartData>();



            
            cd.AutoScale = true;
            cd.ChartType = ChartType.Curve;


            List<double[]> dvals = new List<double[]>();
            int maxlength = 0;
            List<string> names = new List<string>();
            foreach (SignalBase sb in sigs)
            {
                if (sb != null)
                {
                    if (vals.ContainsKey(sb.FullName))
                    {
                        names.Add(sb.FullName);
                        List<double> ls = vals[sb.FullName];
                        if(ls.Count > maxlength)
                            maxlength = ls.Count;
                        dvals.Add(ls.ToArray());
                    }
                }
            }

            double[] x = new double[maxlength];
            for (int i = 0; i < x.Length; i++)
            {
                x[i] = i;
            }
            cd.X = x;
            cd.Y = dvals.ToArray();
            cd.TitleX = "Samples";
            cd.TitlesY = names.ToArray();
            cd.AxisLabelX = "[n]";
            cd.ShowFullNameLegend = false;
            List<ChartData> cds = new List<ChartData>();
            cds.Add(cd);
            return cds;
        }

#endregion
    }
}
