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

namespace SystemDotNet.PostProcessing
{
	/// <summary>
	/// Complex value
	/// </summary>
	public struct Complex
	{
		public Complex(double Real, double Imag)
		{
			this.Real = Real;
			this.Imag = Imag;
		}
		public double Real;
		public double Imag;

		public override string ToString()
		{
			return "{ " + Real + ", " + Imag + " }";
		}

		public double MagnitudeDB
		{
			get
			{
				return 20*Math.Log10(Math.Sqrt( (this.Real* this.Real) + (this.Imag * this.Imag) ));
			}
		}

		public double Magnitude
		{
			get
			{
				return Math.Sqrt( (this.Real* this.Real) + (this.Imag * this.Imag) );
			}

		}

        public double Abs { get { return Math.Sqrt((this.Real * this.Real) + (this.Imag * this.Imag)); } }

        public static double[] GetMagnitude( Complex[] c)
		{

			double[] o1 = new double[c.Length];
			for(int i = 0; i < o1.Length; i++) 
			{
				o1[i] = c[i].MagnitudeDB;
			}
			return o1;
		}

	}
}
