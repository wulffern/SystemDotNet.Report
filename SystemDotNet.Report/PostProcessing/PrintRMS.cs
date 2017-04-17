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
    public class PrintRMS : SystemDotNet.PostProcessing.IPrintReport
    {

        protected string name = null;
        public PrintRMS() { }
        public PrintRMS(string name) { this.name = name; }

        #region IPrintReport Members

        public virtual string Print(string signalName, List<double> d)
        {
            //if (name != null && name == signalName)
                return signalName + " (RMS): " + DoRms(d);
            //else
            //    return signalName + " (RMS): " + DoRms(d);

           // throw new Exception("The method or operation is not implemented.");
        }

        public static double DoRms(List<double> d)
        {
            double mean = 0;
            for (int i = 0; i < d.Count; i++)
            {
                mean += Math.Pow(d[i], 2);
            }

            mean /= d.Count;
           return Math.Sqrt(mean);
        }

        #endregion
    }
}
