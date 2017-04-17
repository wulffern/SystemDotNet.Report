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
using SystemDotNet.Reporter.InputForms;

#endregion

namespace SystemDotNet.Reporter
{
    public class SingleResample:SinglePlotBase
    {
        bool match_names = false;
        int index = 0;
        double x_step = 0;
        double x_delay = 0;
        double x_uncert = 0;


        public SingleResample(ReportNode rp)
            : base(rp)
        {
            AddMenuItem("Wave functions", "Resample", delegate {

                InputFormBase ifb = InputFormFactory.GetInstance("Resample parameters","X-Step",
                    "X-Delay{0}",
                    "X-Uncertainty{1e-10}");
                if (ifb.ShowDialog() == DialogResult.OK)
                {
                    x_step = ifb.Values["X-Step"];
                    x_delay = ifb.Values["X-Delay{0}"];
                    x_uncert = ifb.Values["X-Uncertainty{1e-10}"];
                    Plot();
                }
                });  
        }

        public override List<ChartData> OnPlot(List<double> values)
        {
            List<double> time = reportnode.GetPlot(reportnode.DefaultXAxis);


            List<double> newtime = new List<double>();
            List<double> newvalues = new List<double>();

            double mytime = x_step;
            if (time.Count == 0)
            {
                for (int i = 0; i < values.Count; i++)
                {
                    if (Math.Abs(i-mytime+x_delay) <x_uncert)
                    {
                        newtime.Add(mytime);
                        newvalues.Add(values[i]);
                        mytime += x_step;
                    }
                }
            }
            else
            {
                for (int i = 0; i < time.Count; i++)
                {
                    if (Math.Abs((time[i] - mytime+x_delay)) <= x_uncert)
                    {
                        newtime.Add(mytime);
                        newvalues.Add(values[i]);
                        mytime += x_step;
                    }
                }
            }


            MainForm1 mf = (MainForm1)reportnode.TreeView.FindForm();

            ChartData cd = ChartData.GetInstance();
            cd.Title = "Resample(" + this.Name + ")";
            cd.X = newtime.ToArray();
            cd.Y = new double[][] { newvalues.ToArray() };
            cd.TitleX = reportnode.DefaultXAxis;
            cd.TitlesY = new string[] { this.Name };

            List<ChartData> cds = new List<ChartData>();
            cds.Add(cd);

            mf.OpenFromThread(this.Name, cds);
            return null;
            
        }


    }
}
