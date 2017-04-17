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
    public class SinglePlotXYZ : SinglePlotBase
    {
        string xaxis = "";
        string yaxis = "";
        string zaxis = "";

        public SinglePlotXYZ(ReportNode rp)
            : base(rp)
        {
            AddMenuItem("General", "Plot XYZ", delegate
            {
                XYZSelectForm xy = new XYZSelectForm(rp.PlotKeys);
                if (DialogResult.OK == xy.ShowDialog())
                {
                    xaxis = xy.XAxis;
                    yaxis = xy.YAxis;
                    zaxis = xy.ZAxis;
                    Plot();
                }
            }
            );
        }

        public override List<ChartData> OnPlot(ReportNode rp)
        {
            List<ChartData> cds = new List<ChartData>();
            ChartData cd = ChartData.GetInstance();

            List<double> chxaxis = rp.GetPlot(xaxis);
            List<double> chyaxis = rp.GetPlot(yaxis);
            List<double> chzaxis = rp.GetPlot(zaxis);

            SortedDictionary<string, List<double>> ys = new SortedDictionary<string, List<double>>();

            double[] tmp = new double[chxaxis.Count];
            List<double> dtmp;
            string k;

            for (int i = 0; i < chzaxis.Count; i++)
            {
                if (chzaxis[i] == 0)
                    continue;

                k = (chzaxis[i] / 1e-6).ToString();

                dtmp = new List<double>();
                dtmp.AddRange(tmp);

                if (!ys.ContainsKey(k))
                    ys.Add(k, dtmp);
                ys[k][i] = chyaxis[i];
            }

            string[] keys = new string[ys.Keys.Count];
            double[][] yvals = new double[ys.Keys.Count][];
            int z = 0;
            foreach (string key in ys.Keys)
            {
                keys[z] = key;
                yvals[z] = ys[key].ToArray();
                z++;
            }

            cd.TitlesY = keys;

            cd.TitleX = xaxis;

            cd.X = chxaxis.ToArray();

            cd.Y = yvals;

            cds.Add(cd);

            return cds;
        }


    }
}
