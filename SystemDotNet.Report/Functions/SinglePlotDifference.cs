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
    public class SinglePlotDifference : SinglePlotBase
    {
        List<string> keys;
        Dictionary<string, List<double>> plots;
        string xaxis = "";
        string yaxis = "";

        public SinglePlotDifference(ReportNode rp)
            : base(rp)
        {
            AddMenuItem("General", "Difference between two dataset", delegate
            {
                XYSelectForm xy = new XYSelectForm("Plot 1:", "Plot 2:", rp.PlotKeys);
                if (DialogResult.OK == xy.ShowDialog())
                {
                    xaxis = xy.XAxis;
                    yaxis = xy.YAxis;
                    Plot();
                }
            }
            );

        }

        public override List<ChartData> OnPlot(ReportNode rp)
        {
            PlotType = "Difference (" + xaxis + " - " + yaxis + ")";

            List<ChartData> cds = new List<ChartData>();

            ChartData cd = ChartData.GetInstance();
            cd.TitlesY = new string[] { xaxis + " - " + yaxis };

            List<double> chxaxis = rp.GetPlot(xaxis);
            List<double> chyaxis = rp.GetPlot(yaxis);

            List<double> time = rp.GetPlot("Time");

            if (time != null && time.Count == chxaxis.Count)
            {
                cd.TitleX = "Time";
                cd.AxisLabelX = "[s]";
                cd.X = time.ToArray();
            }
            else
            {
                cd.TitleX = "Sample";
                double[] x = new double[chxaxis.Count];
                for (int i = 0; i < x.Length; i++)
                {
                    x[i] = i;
                }
                cd.AxisLabelX = "[n]";
                cd.X = x;
            }

            double[] y = new double[chxaxis.Count];
            for (int i = 0; i < chxaxis.Count; i++)
            {
                if (chyaxis.Count > i)
                {
                    y[i] = chxaxis[i] - chyaxis[i];
                }

            }

            cd.Y = new double[][] { y };
            cds.Add(cd);

            return cds;
        }

    }
}
