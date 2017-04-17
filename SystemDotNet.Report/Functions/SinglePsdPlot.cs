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
    public class SingleFFTPlot:SinglePlotBase
    {
        double fs = 1;
        public double Fs { get { return fs; } set { fs = value; } }
        double max_amplitude = double.NaN;

        public SingleFFTPlot(ReportNode rp):base(rp)
        {
            fs = 1;
            AddMenuItem("Frequency", "Spectral Density", delegate {Plot(); });
            AddMenuItem("Frequency","Spectral Density [fs]",delegate
            {
               InputFormBase ifb =  InputFormFactory.GetInstance("Sampling Frequency",
                   "Sampling Frequency(Fs)");
               if (ifb.ShowDialog() == DialogResult.OK)
               {
                   this.fs = ifb.Values["Sampling Frequency(Fs)"];
                   Plot();
               }
            }
            );
            AddMenuItem("Frequency", "Spectral Density [fs,A]", delegate
            {

                InputFormBase ifb = InputFormFactory.GetInstance("SD Parameters",
                    "Sampling Frequency(Fs)","Max Signal Amplitude(A0)");
                if (ifb.ShowDialog() == DialogResult.OK)
                {
                    this.fs = ifb.Values["Sampling Frequency(Fs)"];
                    this.max_amplitude = ifb.Values["Max Signal Amplitude(A0)"];
                    Plot();
                }
            }
            );

            
        }

        public override List<ChartData> OnPlot(ReportNode rp)
        {
            PlotType = "Spectral Density (" + this.Name + ")";

                PlotPowerSpectralDensity psd = new PlotPowerSpectralDensity(fs, true, max_amplitude);
                psd.Title = this.Name;
                List<ChartData> cds = psd.Plot(rp.GetPlot(this.Name));
            return cds;
        }

    }

}
