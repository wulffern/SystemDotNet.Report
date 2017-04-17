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
using NextGenLab.Chart;
using System.IO;

#endregion

namespace SystemDotNet.PostProcessing
{
    public class PlotPowerSpectralDensity:IPlotSingle
    {
        private string title;

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        

        double fs;
        double dc_offset;
        bool remove_dc = false;
        double maxamplitude = double.NaN;

        bool visible = true;
        public bool Visible { get { return visible; } set { visible = value; } }

        public PlotPowerSpectralDensity(double fs)
        {
            this.fs = fs;
            title = "SD";
        }

        public PlotPowerSpectralDensity(double fs, double dc_offset)
        {
            this.fs = fs;
            title = "SD";
            this.dc_offset = dc_offset;
        }

        public PlotPowerSpectralDensity(double fs, bool removedc)
        {
            this.fs = fs;
            title = "SD";
            this.remove_dc = removedc;
        }

        public PlotPowerSpectralDensity(double fs, bool removedc,double maxSignalAmplitude)
        {
            this.fs = fs;
            title = "SD";
            this.remove_dc = removedc;
            this.maxamplitude = maxSignalAmplitude;
        }

        public List<ChartData> Plot(List<double> d)
        {
            if (!visible) return new List<ChartData>();

            dc_offset = 0;

            if (remove_dc)
                dc_offset =  ArrayMath.Sum(d.ToArray())/d.Count;

            FFT fft = new FFT();
            ChartData cd;

            List<double> dd = new List<double>();


                foreach (double da in d)
                {
                    dd.Add(da - dc_offset);
                }
            

            List<ChartData> cds = new List<ChartData>();
            
            fft.PowerSpectralDensity(dd.ToArray(), out cd, new Hanning(), fs,maxamplitude);
            cd.Title = this.Title;
            cd.ShowFullNameLegend = false;
            cds.Add(cd);
            return cds;
        }


        public override string ToString ()
        {
            return "Spectral Density";
        }

    }
}
