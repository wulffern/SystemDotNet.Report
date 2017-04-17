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
    public class SinglePlotXYDifferentiate : SinglePlotBase
    {
        string xaxis = "";
        string yaxis = "";

        public SinglePlotXYDifferentiate(ReportNode rp)
            : base(rp)
        {
            AddMenuItem("Calculus", "Differentiate", delegate
            {
                XYSelectForm xy = new XYSelectForm("X-Axis:","Y-Axis:",rp.PlotKeys);
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
            PlotType = xaxis + " VS DERIV(" + yaxis + ")";

            List<ChartData> cds = new List<ChartData>();

            ChartData cd = ChartData.GetInstance();

            cd.TitlesY = new string[] { "d(" + yaxis + ")/dx:" };

            List<double> chxaxis = rp.GetPlot(xaxis);
            List<double> chyaxis = rp.GetPlot(yaxis);

            double[] deriv = new double[chxaxis.Count - 1];
            for (int i = 0; i < deriv.Length; i++)
            {
                deriv[i] = (chyaxis[i + 1] - chyaxis[i]) / (chxaxis[i + 1] - chxaxis[i]);
            }
            double[] xvals = new double[deriv.Length];
            chxaxis.CopyTo(0, xvals, 0, deriv.Length);

            cd.TitleX = xaxis;

            cd.X = xvals;

            cd.Y = new double[][] { deriv };
            cds.Add(cd);
            return cds;
        }

    }
}
