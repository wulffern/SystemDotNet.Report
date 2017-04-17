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
    public class SingleHistogram : SinglePlotBase
    {

        enum HistogramType { Normal, WithNormalDist };

        HistogramType histType = HistogramType.Normal;

        public SingleHistogram(ReportNode rp)
            : base(rp)
        {
            
            AddMenuItem("Statistics", "Histogram w/normal dist.", delegate
            {
                histType = HistogramType.WithNormalDist;
                Plot();
            });
            AddMenuItem("Statistics", "Histogram", delegate
            {
                histType = HistogramType.Normal;
                Plot();
            });

        }


        int histCount = 300;

        public override List<ChartData> OnPlot(ReportNode rp)
        {
            PlotType = this.Name;

            List<ChartData> cds = new List<ChartData>();
            ChartData cd = ChartData.GetInstance();
           

            List<double> values = rp.GetPlot(this.Name);

            //Set bin count
            histCount = values.Count / 100;
            if (histCount < 10)
            {
                histCount = 10;
            }


            //Find Min and Max
            double max = double.MinValue;
            double min = double.MaxValue;
            foreach (double d in values)
            {
                if (d > max)
                {
                    max = d;
                }
                if (d < min)
                {
                    min = d;
                }
            }
            if (min == max) { return cds; };

            //Place values into bins
            double[] vals = new double[histCount + 1];
            double step = (max - min) / ((double)histCount);
            double vmax = double.MinValue;
            foreach (double d in values)
            {
                int v = ((int)Math.Round((d - min) / step));
                vals[v] += 1;
                if (vals[v] > vmax)
                {
                    vmax = vals[v];
                }
            }

            
            List<double> xval = new List<double>(histCount + 1);
            for (int i = 0; i < (histCount + 1); i++)
            {
                xval.Add(min + step * i);
            }


            cd.TitleX = this.Name;
            cd.X = xval.ToArray();

            if (histType == HistogramType.WithNormalDist)
            {
                double um, sigma;
                MathStatistics.MeanStandardDeviation(values, out um, out sigma);
                double[] norm_vals = new double[histCount + 1];

                double vl_max = double.MinValue;
                cd.TitlesY = new string[] { "Count (u=" + um.ToString("e2") + ",std=" + sigma.ToString("e2") + ")", "Normal distribution" };
                for (int i = 0; i < (histCount + 1); i++)
                {
                    double vl = 1 / (sigma * Math.Sqrt(2 * Math.PI)) * Math.Exp(-Math.Pow(xval[i] - um, 2) / (2 * Math.Pow(sigma, 2)));
                    norm_vals[i] = vl;
                    if (vl > vl_max)
                    {
                        vl_max = vl;
                    }
                }

                for (int i = 0; i < (histCount + 1); i++)
                {
                    norm_vals[i] = norm_vals[i] * vmax / vl_max;
                }
                cd.Y = new double[][] { vals, norm_vals };
            }
            else
            {

                double um, sigma;
                MathStatistics.MeanStandardDeviation(values, out um, out sigma);
                cd.ChartType = ChartType.Stem;
          
                cd.TitlesY = new string[] { this.Name + " ( u=" + um.ToString("e2") + ",std=" + sigma.ToString("e2") + ") " };
                cd.Y = new double[][] { vals };
            }
            


            

            cds.Add(cd);
            return cds;
        }
    }
}
