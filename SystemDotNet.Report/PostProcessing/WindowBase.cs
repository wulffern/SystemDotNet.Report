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
    public abstract class WindowBase
    {
        public WindowBase()
        {

        }

        public virtual void ApplyWindow(ref double[] d)
        {
            if (d == null)
                throw new ArgumentException("Double array cannot be null");

            double M = d.Length;
            double val;
            for (int i = 0; i < M; i++)
            {
                Calc(out val, (double)i,M);
                d[i] *= val;
            }
        }

        public abstract void Calc(out double d, double n, double M);

        public abstract double[] Calc(int M);
    }
}
