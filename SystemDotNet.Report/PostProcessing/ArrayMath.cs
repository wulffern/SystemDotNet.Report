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
    public class ArrayMath
    {
        private ArrayMath()
        {

        }

        public static int MakeEqualLength(ref double[] X, ref double[] Y)
        {
            int N;
            if (X.Length == Y.Length)
                N = X.Length;
            else if (X.Length > Y.Length)
            {
                N = X.Length;
                double[] ny = new double[N];
                Array.Copy(Y, ny, N);
                for (int i = Y.Length - 1; i < N; i++)
                {
                    ny[i] = 0;
                }
                Y = ny;
            }
            else
            {
                N = Y.Length;
                double[] nx = new double[N];
                Array.Copy(X, nx, N);
                for (int i = X.Length - 1; i < N; i++)
                {
                    nx[i] = 0;
                }
                X = nx;
            }
            return N;
        }

        public static void Multiply(ref double[] array, double d)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = array[i] * d;
            }
        }

        public static double[] Multiply( double[] a,  double[] b)
        {
            
            int la = a.Length;
            int lb = b.Length;
            int M = 0;
            if (la >= lb)
                M = lb;
            else
                M = la;

            double[] o = new double[M];

            for (int i = 0; i < M; i++)
            {
                o[i] = a[i] * b[i];
            }
            return o;
        }

        public static void Multiply(ref double[] a, ref double[] b)
        {
            int la = a.Length;
            int lb = b.Length;
            int M = 0;
            if (la >= lb)
                M = lb;
            else
                M = la;

            for (int i = 0; i < M; i++)
            {
                a[i] = a[i] * b[i];
            }

        }

        public static void Add(ref double[] array, double d)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = array[i] + d;
            }
        }

        public static double Sum(double[] array){
            double sum = 0;
            for (int i = 0; i < array.Length; i++)
            {
                sum += array[i];
            }
            return sum;
        }
        public static double[] Pow(double[] array, double pow)
        {
            double[] o1 = new double[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                o1[i] = Math.Pow(array[i], pow);
            }
            return o1;
        }

        //public static void Pow(ref double[] array,double pow)
        //{
            
        //}

        public static double[] Sqrt( double[] array)
        {
            double[] outar = new double[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                outar[i] = Math.Sqrt(array[i]);
            }
            return outar;
        }

        public static  void Sqrt(ref double[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = Math.Sqrt(array[i]);
            }
        }
    }
}
