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
using SystemDotNet;

namespace SystemDotNet.Circuits
{
    public class SignalSourceSine : SignalSourceBase
    {
        protected double fs;
        protected double f0;
        protected double magnitude;
        protected double amplitude;

        public double SignalFrequency
        {
            get { return f0; }
            set { f0 = value; }
        }

        public SignalSourceSine(ModuleBase parent, Clock clock, double refhi, double reflo,
            double SignalFrequency,double Magnitude):base(parent,clock,refhi,reflo)
        {
            this.fs = clock.Frequency;
            f0 = SignalFrequency;
            this.magnitude = Magnitude;
            this.maxindex = (long)fs;
            amplitude = fullscale / 2;

        }

        protected override double Generate(long index)
        {
            return reflo + fullscale / 2 + fullscale / 2 * magnitude * Math.Sin((2 * Math.PI * f0 * (double)index++) / maxindex);
        }
    }
}
