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
    public class MathStatistics
    {

        public static double Mean(List<double> vals)
        {
            double mean = 0;
            for (int i = 0; i < vals.Count; i++)
                mean += vals[i];
            return mean / (double)vals.Count;
        }

        public static double Variance(List<double> vals)
        {
            double mean = Mean(vals);
            double var = 0;
            for (int i = 0; i < vals.Count; i++)
            {
                var += Math.Pow((vals[i] - mean), 2) / (vals.Count);
            }

            return var;
        }

        public static double StandardDeviation(List<double> vals)
        {
            double var = Variance(vals);
            return Math.Sqrt(var);
        }

        public static void MeanStandardDeviation(List<double> vals, out double mean, out double StandardDeviation)
        {
            mean = Mean(vals);
            double var = 0;
            for (int i = 0; i < vals.Count; i++)
            {
                var += Math.Pow((vals[i] - mean), 2) / (vals.Count);
            }

            StandardDeviation = Math.Sqrt(var);
        }
    }
}
