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
    public class PlotXY : IPlotMultiple
    {
        SignalBase xsignal;
        List<SignalBase> ysignals = new List<SignalBase>();
        ChartData cd = ChartData.GetInstance();
        public string Title { get {return cd.Title; } set { cd.Title = value; } }
        public string AxisLabelX { get { return cd.AxisLabelX; } set { cd.AxisLabelX = value; } }
        public string AxisLabelY { get { return cd.AxisLabelY; } set { cd.AxisLabelY = value; } }
        public ChartType ChartType { get { return cd.ChartType; } set { cd.ChartType = value; } }

        bool visible = true;
        public bool Visible { get { return visible; } set { visible = value; } }


        public PlotXY(SignalBase xsignal, params SignalBase[] ysignals)
        {
            this.xsignal = xsignal;
            this.ysignals.AddRange(ysignals);
        }

        #region IPlotMultiple Members

        public List<NextGenLab.Chart.ChartData> Plot(Dictionary<string, List<double>> vals)
        {

            if (!visible) return new List<ChartData>();

            if (!vals.ContainsKey(xsignal.FullName))
                return new List<NextGenLab.Chart.ChartData>();

            double[] x = vals[xsignal.FullName].ToArray();
            List<double[]> ys = new List<double[]>();
            List<string> names = new List<string>();
            foreach (SignalBase ysig in ysignals)
            {
                names.Add(ysig.FullName);
                ys.Add(vals[ysig.FullName].ToArray());
            }

            cd.AutoScale = true;
            cd.X = x;
            cd.Y = ys.ToArray();
            cd.TitleX = xsignal.FullName;
            cd.TitlesY = names.ToArray();
            cd.ShowFullNameLegend = false;

            List<ChartData> cds = new List<ChartData>();
            cds.Add(cd);
            return cds;
        }

        #endregion
    }
}
