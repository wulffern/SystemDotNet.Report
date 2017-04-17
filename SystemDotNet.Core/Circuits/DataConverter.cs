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

namespace SystemDotNet.Circuits
{
    public class DataConverter
    {
        static Random r = new Random();

        public static double DAC(ulong intval, double refHi, double refLo, int bits)
        {
            //Calculate the LSB
            double LSB = (refHi - refLo) / (Math.Pow(2, bits)-1);

            //Calculate the output value
            double value = refLo + LSB * intval;

            //Return the output value
            return value;
        }

        public static double DACDither(ulong intval, double refHi, double refLo, int bits)
        {
            //Calculate the LSB
            double LSB = (refHi - refLo) / (Math.Pow(2, bits)-1);

            //Calculate the output value
            double value = refLo + LSB * intval;// +LSB * (r.NextDouble() - 0.5);

            //Return the output value
            return value;
        }

        /// <summary>
        /// Totally Ideal ADC. Will have quantization noise with odd harmonics only
        /// </summary>
        /// <param name="input">Input Signal</param>
        /// <param name="refhi">High reference</param>
        /// <param name="reflo">Low reference</param>
        /// <param name="bits">Number of bits</param>
        /// <returns></returns>
        public static ulong ADC(double input, double refhi, double reflo, int bits)
        {
            input = (input - reflo);
            input = input / (refhi - reflo);
            double lsb = (refhi - reflo) / (Math.Pow(2, bits) - 1);
            long tmp = (long)Math.Round((input +lsb)* (Math.Pow(2, bits) - 1));
            long oo = tmp;
            if (oo >= (Math.Pow(2, bits)))
                oo = (long)Math.Pow(2, bits) - 1;
            else if (oo < 0)
                oo = 0;
            return (ulong)oo;
        }

        public static ulong ADCSpecial(double input, double refhi, double reflo, int bits)
        {
            input = (input - reflo);
            input = input / (refhi - reflo);

            int ibits = 24;
            long tmp = (long)Math.Round(input  * (Math.Pow(2, ibits) - 1));

            long lsb = (long)(Math.Pow(2, ibits) / Math.Pow(2, bits));

            tmp = tmp / lsb;

            long oo = tmp;
            if (oo >= (Math.Pow(2, bits)))
                oo = (long)Math.Pow(2, bits) - 1;
            else if (oo < 0)
                oo = 0;
            return (ulong)oo;
        }
        /// <summary>
        /// Ideal ADC with dither. Will reduce resolution by 1 bit. Effective number of bits = 
        /// Resolution -1
        /// </summary>
        /// <param name="input">Input Signal</param>
        /// <param name="refhi">High reference</param>
        /// <param name="reflo">Low reference</param>
        /// <param name="bits">Number of bits</param>
        /// <returns></returns>
        public static ulong ADCDither(double input, double refhi, double reflo, int bits)
        {
            input = (input - reflo);
            input = input / (refhi - reflo);
            long tmp = (long)Math.Round(input * (Math.Pow(2, bits) - 1) + (r.NextDouble() - 0.5));
            long oo = tmp;
            if (oo >= (Math.Pow(2, bits)))
                oo = (long)Math.Pow(2, bits) - 1;
            else if (oo < 0)
                oo = 0;
            return (ulong)oo;
        }
    }
}
