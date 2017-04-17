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

#endregion

namespace SystemDotNet.PostProcessing
{
    public class PrintDynamicParameters : SystemDotNet.PostProcessing.IPrintReport
    {
         double f0;
         double fs;
         double signalamplitude = 1;
         bool remove_dc = false;
         double fmin = -1;
         double fmax = -1;
        public PrintDynamicParameters(double f0, double fs,double fmin, double fmax,bool remove_dc)
        {
            this.f0 = f0;
            this.fs = fs;
            this.fmin = fmin;
            this.fmax = fmax;
            this.remove_dc = remove_dc;
        }

        public PrintDynamicParameters(double f0, double fs, int OSR, bool remove_dc)
        {
            this.f0 = f0;
            this.fs = fs;

            this.fmax = fs / (2 * OSR);
            this.fmin = 0;
            this.remove_dc = remove_dc;
        }

        public virtual string Print(string key, List<double> d)
        {
            int M = d.Count;
            FFT fft = new FFT();

            double snr;
            double sndr;
            double enob;

            fft.DynamicParameters(d, new Hanning(), fs, f0, fmin, fmax,remove_dc, 
                out snr, out sndr, out enob);

            StringBuilder sb = new StringBuilder();
            sb.Append("SNR(" + key + "): " + snr.ToString("##.##") + "\n");
            sb.Append("SNDR(" + key + "): " + sndr.ToString("##.##") + "\n");
            sb.Append("ENOB(" + key + "): " + enob.ToString("##.##"));

            return sb.ToString();
        }
    }
}
