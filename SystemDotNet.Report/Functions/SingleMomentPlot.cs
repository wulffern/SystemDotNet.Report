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
    public class SingleMomentPlot : SinglePlotBase
    {
        enum MomentType { Mean, Variance, ThirdMoment };

        MomentType momenttype = MomentType.Mean;

        public SingleMomentPlot(ReportNode rp)
            : base(rp)
        {
            AddMenuItem("Statistics","Mean", delegate
            {
                momenttype = MomentType.Mean;
                Plot();
            });
            AddMenuItem("Statistics","Variance", delegate
            {
                momenttype = MomentType.Variance;
                Plot();
            });
            AddMenuItem("Statistics","Third Moment", delegate
            {
                momenttype = MomentType.ThirdMoment;
                Plot();
            });
        }

        public override List<NextGenLab.Chart.ChartData> OnPlot(ReportNode rp)
        {
            PlotType = momenttype.ToString() + "(" + this.Name + ")";
            ChartData cd = ChartData.GetInstance();
            List<double> values = rp.GetPlot(this.Name);
                   double[] meanlist =  Mean(values);
                   
                   cd.Y = new double[][] { meanlist };
                   cd.TitlesY = new string[] { this.Name };
                   cd.TitleX = "Sample";
                   double[] x = new double[values.Count];
                   for (int i = 0; i < x.Length; i++)
                   {
                       x[i] = i;
                   }
                   cd.AxisLabelX = "[n]";
                   cd.X = x;

                   List<ChartData> cds = new List<ChartData>();
                   cds.Add(cd);
                   return cds;
        }

        double[] Mean(List<double> values)
        {
            double mean = 0;
            double[] meanlist = new double[values.Count];

            mean = values[0];
            meanlist[0] = mean;

            for (int i = 1; i < values.Count; i++)
            {

                switch (momenttype)
                {
                    case MomentType.Mean:
                        
                        break;
                    case MomentType.Variance:
                        mean += Math.Pow(values[i] - meanlist[i-1],2);
                        break;
                    case MomentType.ThirdMoment:
                        mean += Math.Pow(values[i] - meanlist[i-1],3);
                        break;

                }
               
                meanlist[i - 1] = mean / i;

            }

            return meanlist;
        }

    }

}
