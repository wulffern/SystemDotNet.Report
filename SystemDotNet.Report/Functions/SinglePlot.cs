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
using SystemDotNet.PostProcessing;
using NextGenLab.Chart;
using System.Windows.Forms;

#endregion

namespace SystemDotNet.Reporter
{
    public class SinglePlot:SinglePlotBase
    {

        public SinglePlot(ReportNode rp):base(rp)
        {
            AddMenuItem("General", "Just Plot", delegate { Plot(); });
        }

        public override List<ChartData> OnPlot(ReportNode rp)
        {
            PlotType = this.Name;
            List<ChartData> cds = new List<ChartData>();
            ChartData cd = ChartData.GetInstance();

            List<double> time = rp.GetPlot(rp.DefaultXAxis);
            List<double> values = rp.GetPlot(this.Name);

            cd.TitlesY = new string[] { this.Name };

            if (time != null && time.Count == values.Count)
            {
                cd.TitleX = rp.DefaultXAxis;
                cd.AxisLabelX = "[s]";
                cd.X = time.ToArray();
            }
            else
            {
                cd.TitleX = "Sample";
                double[] x = new double[values.Count];
                for (int i = 0; i < x.Length; i++)
                {
                    x[i] = i;
                }
                cd.AxisLabelX = "[n]";
                cd.X = x;
            }

            cd.Y = new double[][] { values.ToArray() };

            cds.Add(cd);
            return cds;
        }
    }
}
