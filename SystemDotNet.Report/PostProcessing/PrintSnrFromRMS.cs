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
using System;
using System.Collections.Generic;
using System.Text;

namespace SystemDotNet.PostProcessing
{
    public class PrintSnrFromRMS : PrintRMS
    {

        double signal_amplitude;
    
        public PrintSnrFromRMS(double signal_amplitude, string name):base(name)
        {
            this.signal_amplitude = signal_amplitude;
        }

        public override string Print(string signalName, List<double> d)
        {
          //  if (name != null && name == signalName)
                return signalName + " (SNR): " + DoSNR(d);
        //    else
           //     return signalName + " (SNR): " + DoSNR(d);
        }

        public string Print(string signalName, List<double> signald, string noiseName, List<double> noised)
        {

            double snr = (20 * Math.Log10(DoRms(signald) / DoRms(noised)));

            return "SNR(" + signalName + "/" + noiseName + "): " + snr.ToString("E");

        }

        double DoSNR(List<double> d)
        {
            return (20 * Math.Log10(signal_amplitude / (Math.Sqrt(2) * DoRms(d))));
        }
    }
}
