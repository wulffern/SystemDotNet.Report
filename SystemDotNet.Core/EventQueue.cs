/*
Copyright (C) 2004 Carsten Wulff

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
using System.Collections;
using System.Text;
using System.IO;
using SystemDotNet;
using System.Collections.Generic;
using System.Threading;
using System.ComponentModel;
//using NextGenLab.Chart;



namespace SystemDotNet
{
    /// <summary>
    /// Base for the eventqueue, heart of the simulator
    /// </summary>
    public class EventQueue
    {
        //Holds timesteps and the events to be run
        Dictionary<long, List<IRunnable>> queue = new Dictionary<long, List<IRunnable>>();

        //Current time
        long time = 0;
        double timeunit = 1e-9;

        //The next timestep
        long nexttime = 0;

        //Status code
        protected long events = 0;
        protected long eventswithouteffect = 0;
        protected long maxeventsatanytimestep = 0;
        protected long maxtimestepsinqueue = 0;

        //Is it the first step
        bool firststep = true;

        List<ProcessWorker> workers = new List<ProcessWorker>();
        //Holds the a writer that the user wants i.e a VcdWriter
        ISignalWriter icw = null;

        //Signals to be written
        List<SignalBase> signalsToWrite = new List<SignalBase>();

        /// <summary>
        /// Writer for trace signals, i.e VcdWriter
        /// </summary>
        /// <value></value>
        [Browsable(false)]
        public ISignalWriter SignalWriter { get { return icw; } set { icw = value; } }

        /// <summary>
        /// Fired when the simulator takes its first step
        /// </summary>
        public event EmptyHandler FirstStep;

        /// <summary>
        /// Current time
        /// </summary>
        /// <value></value>
        [Browsable(false)]
        public long Time { get { return time; } set { time = value; } }

        /// <summary>
        /// The unit of the time
        /// </summary>
        /// <value></value>
        [CategoryAttribute("Simulator Settings"),
              DescriptionAttribute("Mapping to real time of one time step")]
        public double TimeUnit { get { return timeunit; } set { timeunit = value; } }

        public double RealTime
        {
            get { return Time * TimeUnit; }

        }


        /// <summary>
        /// Not much done here
        /// </summary>
        protected EventQueue() { }

        public void AddWorker(ProcessWorker pr)
        {
            lock (workers) { workers.Add(pr); }
        }

        public void RemoveWorker(ProcessWorker pr)
        {
            lock (workers) { workers.Remove(pr); }
        }

        /// <summary>
        /// Restarts the eventqueue
        /// </summary>
        public virtual void Restart()
        {
            time = 0;
            nexttime = 0;
            events = 0;
            eventswithouteffect = 0;
            firststep = true;
            queue = new Dictionary<long, List<IRunnable>>();
        }

        /// <summary>
        /// Resets the eventqueue
        /// </summary>
        protected virtual void Reset()
        {
            Restart();
            if (FirstStep != null)
            {
                FirstStep = null;
            }
            signalsToWrite = new List<SignalBase>();
        }

        /// <summary>
        /// Runs a single time step in the eventqueue
        /// </summary>
        /// <returns></returns>
        public virtual bool Step()
        {
            if (firststep)
            {
                if (icw != null)
                {
                    icw.WriteNames(signalsToWrite);
                }

                if (FirstStep != null)
                    FirstStep();
                firststep = false;
            }
            nexttime = long.MaxValue;

            foreach (long key in queue.Keys)
            {
                if (key < nexttime)
                {
                    nexttime = key;
                }
            }

            if (nexttime == long.MaxValue)
                return false;

            time = nexttime;

            //If the queue contains this timestep (which it really should) -- 
            //run the events stored at that location. Notice that with
            //the queue.Remove(time) we allow events to register new events at
            //the current timestep without causing a loop. 
            //These events will be run on next calling of Step.
            if (queue.ContainsKey(time))
            {
                List<IRunnable> ite = queue[time];
                queue.Remove(time);

                if (maxeventsatanytimestep < ite.Count)
                    maxeventsatanytimestep = ite.Count;

                ite.ForEach(delegate(IRunnable r)
                {

                    if (r != null && r.Run())
                    {
                       

                        events++;
                    }
                    else
                    {
                        eventswithouteffect++;
                    };
                }
                );

                if (icw != null)
                {
                    icw.WriteSignals(signalsToWrite, time);
                }
            }

            if (maxtimestepsinqueue < queue.Count)
                maxtimestepsinqueue = queue.Count;

            while (workers.Count > 0)
                Thread.Sleep(0);
            return true;
        }

        /// <summary>
        /// Adds a signal to the event queue
        /// </summary>
        /// <param name="c">SignalBase to write to</param>
        /// <param name="Value">Value to write too channel</param>
        /// <param name="delay">How long delay before writing</param>
        public virtual void Write(IRunnable ite, long delay)
        {
            long key = time + delay;
            if (!queue.ContainsKey(key))
                queue.Add(key, new List<IRunnable>());
            queue[key].Add(ite);
        }

        /// <summary>
        /// Adds a signal to the event queue
        /// </summary>
        /// <param name="c">SignalBase to write too</param>
        /// <param name="Value">Value to write to channel</param>
        public virtual void Write(IRunnable ite)
        {
            long key = time + 1;
            if (!queue.ContainsKey(key))
                queue.Add(key, new List<IRunnable>());
            queue[key].Add(ite);
        }

        /// <summary>
        /// Trace a signal
        /// </summary>
        /// <param name="sig">Signal to trace</param>
        public void Trace(SignalBase sig)
        {
            this.signalsToWrite.Add(sig);
        }

        /// <summary>
        /// Trace signals
        /// </summary>
        /// <param name="sigs">Signals to trace</param>
        public void Trace(params SignalBase[] sigs)
        {
            for (int i = 0; i < sigs.Length; i++)
            {
                Trace(sigs[i]);
            }
        }

        /// <summary>
        /// Remove trace on a signal
        /// </summary>
        /// <param name="sig">Signal to remove</param>
        public void UnTrace(SignalBase sig)
        {
            this.signalsToWrite.Remove(sig);
        }

        /// <summary>
        /// Remove trace on signals
        /// </summary>
        /// <param name="sigs">Signals to remove</param>
        public void UnTrace(params SignalBase[] sigs)
        {
            for (int i = 0; i < sigs.Length; i++)
            {
                UnTrace(sigs[i]);
            }
        }

        public string GetStatus()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Current time is " + time + ".\n");
            sb.Append("Which translated into real time is " + ((double)(time * TimeUnit)).ToString("E2") + "s.\n");
            sb.Append("At which point I've run " + events.ToString("E2") + " events.\n");
            sb.Append("Had a maximum of " + maxeventsatanytimestep + " events at any timestep.\n");
            sb.Append("And a maximum of " + maxtimestepsinqueue + " timesteps in the queue.\n");
            return sb.ToString();
        }

        public string GetStatusShort()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("T = " + DblToString(time));
            sb.Append(", Tr = " + DblToString(time * TimeUnit) + "s, ");
            sb.Append("E = " + DblToString(events) + ", ");
            sb.Append("E0 = " + DblToString(eventswithouteffect) + ", ");
            sb.Append("Emax = " + DblToString(maxeventsatanytimestep) + ", ");
            sb.Append("Tmax =  " + DblToString(maxtimestepsinqueue));
            return sb.ToString();
        }

        protected string DblToString(double d)
        {
            string postfix;
            double denum;
            GetPostfix(Power(d), out postfix, out denum);
            d = d / denum;
            return d.ToString(".#") + postfix;
        }

        /// <summary>
        /// Return true if one or more events are queued to run in the time specified
        /// </summary>
        /// <param name="step">Check for events listed in this next step time in ns</param>
        public bool FindEvents(long step)
        {
            lock (queue)
            {
                foreach (long key in queue.Keys)
                {
                    if ((key - this.time) <= step)
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Calculates prefix and denumerator of a power from yocto(1e-24) to yotta(1e24)
        /// </summary>
        /// <param name="axis_val_temp">Power</param>
        public static void GetPostfix(int power, out string axis_val_label, out double axis_val_adjusted)
        {

            double axis_val_temp = Math.Pow(10, power);
            if (axis_val_temp == 0) { axis_val_adjusted = 0; axis_val_label = ""; }

            double pos_neg = 1;

            if (axis_val_temp < 0) { axis_val_temp *= -1; pos_neg = -1; }

            if (axis_val_temp == 0)
            {
                axis_val_adjusted = 1;
                axis_val_label = "";

            }
            else if (axis_val_temp >= 1e24)
            {
                axis_val_adjusted = 1e24;
                axis_val_label = "Y";
            } // yotta = 1e24
            else if (axis_val_temp >= 1e21)
            {
                axis_val_adjusted = 1e21;
                axis_val_label = "Z";
            } // zetta = 1e21
            else if (axis_val_temp >= 1e18)
            {
                axis_val_adjusted = 1e18;
                axis_val_label = "E";
            } // exa   = 1e18
            else if (axis_val_temp >= 1e15)
            {
                axis_val_adjusted = 1e15;
                axis_val_label = "P";
            } // peta  = 1e15
            else if (axis_val_temp >= 1e12)
            {
                axis_val_adjusted = 1e12;
                axis_val_label = "T";
            } // tera  = 1e12
            else if (axis_val_temp >= 1e9)
            {
                axis_val_adjusted = 1e9;
                axis_val_label = "G";
            } // giga  = 1e9
            else if (axis_val_temp >= 1e6)
            {
                axis_val_adjusted = 1e6;
                axis_val_label = "M";
            } // mega  = 1e6
            else if (axis_val_temp >= 1e3)
            {
                axis_val_adjusted = 1e3;
                axis_val_label = "k";
            } // kilo  = 1e3
            else if (axis_val_temp >= 1)
            {
                axis_val_adjusted = 1;
                axis_val_label = "";
            }
            else if (axis_val_temp >= 1e-3)
            {
                axis_val_adjusted = 1e-3;
                axis_val_label = "m";
            } // milli = 1e-3
            else if (axis_val_temp >= 1e-6)
            {
                axis_val_adjusted = 1e-6;
                axis_val_label = "µ";
            } // micro = 1e-6
            else if (axis_val_temp >= 1e-9)
            {
                axis_val_adjusted = 1e-9;
                axis_val_label = "n";
            } // nano  = 1e-9
            else if (axis_val_temp >= 1e-12)
            {
                axis_val_adjusted = 1e-12;
                axis_val_label = "p";
            } // pico  = 1e-12
            else if (axis_val_temp >= 1e-15)
            {
                axis_val_adjusted = 1e-15;
                axis_val_label = "f";
            } // femto = 1e-15
            else if (axis_val_temp >= 1e-18)
            {
                axis_val_adjusted = 1e-18;
                axis_val_label = "a";
            } // atto  = 1e-18
            else if (axis_val_temp >= 1e-21)
            {
                axis_val_adjusted = 1e-21;
                axis_val_label = "z";
            } // zepto = 1e-21
            else
            {
                axis_val_adjusted = Math.Pow(10, power);
                axis_val_label = "e" + power;
            } // yocto = 1e-24
            //return  axis_val_label;
        }

        /// <summary>
        /// Gets the power of a double
        /// </summary>
        /// <param name="val">double value</param>
        /// <returns>power of the double</returns>
        public static int Power(double val)
        {
            int d = -31;
            double tmp;
            while (true)
            {
                if (val == 0.0)
                {
                    break;
                }
                tmp = Math.Abs(val) / Math.Pow(10, d);
                if (tmp > 0 && tmp < 10)
                    break;
                d++;
                if (d > 31)
                    break;
            }
            return d;
        }
    }
}
