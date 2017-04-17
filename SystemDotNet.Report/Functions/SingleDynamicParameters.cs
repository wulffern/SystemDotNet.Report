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
    public class SingleDynamicParameters:SinglePlotBase
    {
        double fs = 1;
        double f0 = 1;
        double fmin = 0;
        double fmax = 1;
        public double Fs { get { return fs; } set { fs = value; } }


        public SingleDynamicParameters(ReportNode rp):base(rp)
        {
            AddMenuItem("Calculate","Dynamic Parameters (SNDR SNR ENOB) [fs,f0,fmin,fmax]",delegate
            {
                InputFormBase ifb = InputFormFactory.GetInstance("Frequency Information [fs,f0,fmin,fmax]",
                    "Sampling Frequency(Fs)",
                    "Input Frequency(F0)",
                    "Bandlimit noise, lower limit(Fmin)",
                    "Bandlimit noise, high limit(Fmax)");

                if (ifb.ShowDialog() == DialogResult.OK)
                {
                    this.fs = ifb.Values["Sampling Frequency(Fs)"];
                    this.f0 = ifb.Values["Input Frequency(F0)"];
                    this.fmin = ifb.Values["Bandlimit noise, lower limit(Fmin)"];
                    this.fmax = ifb.Values["Bandlimit noise, high limit(Fmax)"];
                    Plot();
                }
                
            });

            AddMenuItem("Calculate", "Dynamic Parameters (SNDR SNR ENOB) [fs,f0]", delegate
            {
                InputFormBase ifb = InputFormFactory.GetInstance("Frequency Information [fs,f0]",
                    "Sampling Frequency(Fs)",
                    "Input Frequency(F0)");

                if (ifb.ShowDialog() == DialogResult.OK)
                {
                    this.fs = ifb.Values["Sampling Frequency(Fs)"];
                    this.f0 = ifb.Values["Input Frequency(F0)"];
                    this.fmin = -1;
                    this.fmax = -1; 
                    Plot();
                }

            });
        }

        public override List<NextGenLab.Chart.ChartData> OnPlot(ReportNode rp)
        {
            PrintDynamicParameters pdp = new PrintDynamicParameters(f0, fs, fmin, fmax, true);

            string s = pdp.Print(this.Name, rp.GetPlot(this.Name));
            MessageBox.Show(s);

            return new List<ChartData>();
        }

    }

}
