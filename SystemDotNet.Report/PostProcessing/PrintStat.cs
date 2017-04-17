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
    public class PrintStat : SystemDotNet.PostProcessing.IPrintReport
    {

        #region IPrintReport Members

        public string Print(string signalName, List<double> d)
        {


            double mean;
            double std;
            MathStatistics.MeanStandardDeviation(d, out mean, out std);

            StringBuilder sb = new StringBuilder();
            sb.Append(signalName);
            sb.Append(" Mean : ");
            sb.Append(mean);
            sb.Append("\n");

            sb.Append(signalName);
            sb.Append(" std : ");
            sb.Append(std);
            sb.Append("\n");

            return sb.ToString();
           
           // throw new Exception("The method or operation is not implemented.");
        }

        #endregion
}
}
