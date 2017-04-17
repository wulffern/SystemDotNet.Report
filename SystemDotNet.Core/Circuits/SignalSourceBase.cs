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
using SystemDotNet;

#endregion

namespace SystemDotNet.Circuits
{
    public abstract class SignalSourceBase : ModuleBase
    {
        long index = 0;
        protected double refhi;
        protected double fullscale;
        protected double reflo;
        protected long maxindex = long.MaxValue;
        int maxresolution = 32;
        Random r;
        Clock clock;


        Signal<double> output = new Signal<double>(SignalDirectionType.Output);
        Signal<bool> rising = new Signal<bool>(SignalDirectionType.Input);
        public Signal<bool> Clk { get { return clock.Clk; } }
        public Signal<double> Output { get { return output; } }
        public Signal<bool> Rising { get { return rising; } }
       

        public SignalSourceBase(ModuleBase parent, Clock clock, double refhi, double reflo)
            : base(parent)
        {
            this.clock = clock;
            this.refhi = refhi;
            this.reflo = reflo;
            fullscale = (refhi - reflo);
            r = new Random();
            rising.Write(true);
            Output.Write(reflo + fullscale / 2);
        }

        [Process("Clk")]
        public void Clk_Changed()
        {
            if (!(clock.Clk.Read() ^ rising.Read()))
            {
                double val = Generate(index) + ((refhi - reflo) * (r.NextDouble() - 0.5)) / Math.Pow(2, maxresolution);
                if (index++ >= maxindex)
                    index = 0;
                Output.Write(val);
            }
        }

        protected abstract double Generate(long index);
    }
}
