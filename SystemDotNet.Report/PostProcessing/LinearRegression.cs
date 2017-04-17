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
    public class LinearRegression
    {
        double[] X;
        double[] Y;
        double N;

        public LinearRegression(double[] X, double[] Y)
        {
           N =  ArrayMath.MakeEqualLength(ref X,ref Y);
           this.X = X;
           this.Y = Y;

       }

        /* 	
	The point on the Y - axis where the line Y = A + BX intercepts it is given by the equation:
	A = [(SUM(Y))*(SUM(SQUARE(X))) - (SUM(X))*(SUM(X*Y))] / [N*SUM(SQUARE(X)) - SQUARE((SUM(X)))].

	

	The slope of the line Y = A + BX is given by the equation:
	B = N*SUM(X*Y) - (SUM(X))*(SUM(Y)) / [N*SUM(SQUARE(X)) - SQUARE((SUM(X))]

	

	The correlation coefficient of the line Y = A + BX where 0 means no correlation and 1 means perfect
	correlation is given by the equation:
	R = N*SUM(X*Y) - (SUM(X))*(SUM(Y)) / SQRT[N*SUM(SQUARE(X)) - SQUARE(SUM(X))]*SQRT[N*SUM(SQUARE(Y)) - SQUARE((SUM(Y)))]
*/


        public void Coefficients(out double a, out double b, out double r)
        {
            double x2sum = ArrayMath.Sum(ArrayMath.Pow(X,2));
            double xsum2 = Math.Pow(ArrayMath.Sum(X),2);
            double ysum = ArrayMath.Sum(Y);
            double y2sum = ArrayMath.Sum(ArrayMath.Pow(Y,2));
            double ysum2 = Math.Pow(ArrayMath.Sum(Y),2);
            double xsum = ArrayMath.Sum(X);
            double xysum = ArrayMath.Sum(ArrayMath.Multiply(X,Y));


            
            a = (ysum*x2sum - xsum*xysum)/(N*x2sum - xsum2);
            b = (N * xysum - xsum * ysum) / (N * x2sum - xsum2);
            r = (N * xysum - xsum * ysum) / (Math.Sqrt(N * x2sum - xsum2) * Math.Sqrt(N * y2sum - ysum2));
        }


    }
}
