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
using SystemDotNet;


namespace SystemDotNet.Circuits
{
    public class Clock : ModuleBase
    {
        int period = 200;
        double frequency = 10;
        int delay = 200;
        Signal<bool> clk = new Signal<bool>(SignalDirectionType.Output,true);
        
        public Signal<bool> Clk { get { return clk; } }
        public int ClockPeriod { get { return period; } }
        public int PulseWidth { get { return period / 2; } }
        public double Frequency { get { return frequency; } }
        public int Delay { get { return delay; } set { delay = value; } }
        public Clock(ModuleBase parent, int ClockPeriod) : base(parent) 
        { 
           this.period = ClockPeriod; 
           Simulator sim = Simulator.GetInstance();
           this.frequency =  1/(period * sim.TimeUnit);
        
        }
        public Clock(ModuleBase parent, double Frequency) : base(parent) 
        {
            Simulator sim = Simulator.GetInstance();
            this.frequency = Frequency;
            this.period = (int)(1 / (Frequency * sim.TimeUnit));
        }

        public override void First()
        {
            //Write the first value to get things started
            Clk.Write(!Clk.Read(), delay);
        }

        [Process("Clk")]
        public void Next()
        {
            Clk.Write(!Clk.Read(), period/2);
        }

    }
}
