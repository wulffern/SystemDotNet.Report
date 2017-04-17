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
#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;

#endregion

namespace SystemDotNet.PostProcessing
{

    /// <summary>
    /// Logs a number of signals triggered by a master clock
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Logger : ModuleBase,IDisposable
    {

        //Signals to be logged
        protected List<SignalBase> logSignals = new List<SignalBase>();

        //Master clock
        public Signal<bool> Clk = new Signal<bool>();

        //Rising edge
        public Signal<bool> Rising = new Signal<bool>();

        private bool stop;

        public bool StopOnFinish
        {
            get { return stop; }
            set { stop = value; }
        }

        int clk = 0;
        long maxsamples;
        int percentage;

        private long wait;

        /// <summary>
        /// How long the logger should wait. Given in simulator time.
        /// <example>
        /// Logger lg;
        /// double RealTime = 1e-6; //1µs
        /// lg.Wait = RealTime/Simulator.GetInstance().TimeUnit
        /// </example>
        /// </summary>
        public long Wait
        {
            get { return wait; }
            set { wait = value; }
        }


        private long samples;

        public long Samples
        {
            get { return samples; }
            set { samples = value; }
        }


        public long SampleCount
        {
            get { return maxsamples - samplecount; }
        }

        private double _StopTime = double.MaxValue;

        public double StopTime
        {
            get { return _StopTime; }
            set { _StopTime = value; }
        }

        //public long SampleCount { get { return samples -samplecount; } }
        long samplecount;

        Dictionary<string, string> KeyValue = new Dictionary<string, string>();

        IOutputWriter owb;

        public IOutputWriter OutputWriter { get { return owb; } }

        public Logger(ModuleBase parent, Signal<bool> Clock, IOutputWriter owb)
            : base(parent)
        {

            if (owb == null)
                throw new CommandException("Output Writer CANNOT be null");
            this.owb = owb;

            Samples = long.MaxValue;
            StopOnFinish = false;
            Clk.Connect( Clock );


        }
       

        /// <summary>
        /// Opens the stream writer
        /// </summary>
        public override void First ()
        {

            owb.Open();
            maxsamples = samples;
            percentage = 0;
            samplecount = Samples;

        }

        /// <summary>
        /// Closes the stream writer
        /// </summary>
        public override void OnStop()
        {
            owb.Close();
        }

        public void Clear()
        {
          
        }

        /// <summary>
        /// Add a signal to logger
        /// </summary>
        /// <param name="sig">Signal to be logged</param>
        public void Add(SignalBase sig)
        {

            owb.Add(sig);
        }

        /// <summary>
        /// Add signals to logger
        /// </summary>
        /// <param name="sigs"></param>
        public void Add(params SignalBase[] sigs)
        {
            for (int i = 0; i < sigs.Length; i++)
            {
                owb.Add(sigs[i]);
               // Add(sigs[i], new ValueWriter(ValueWriterDel));
            }
        }


        public void Add(ModuleBase mb,int levels)
        {
            if (!mb.InitComplete)
            {
                Simulator.GetInstance().PostInit += delegate { Add(mb, levels); };
                    return;
            }
            
            foreach (SignalBase sb in mb.Signals)
            {
                owb.Add(sb);
            }

            levels--;
            if (levels > 0)
            {
                foreach (ModuleBase child in mb.Children)
                {
                    Add(child, levels);
                }
            }
        }

        /// <summary>
        /// Process that writes values on rising clock
        /// </summary>
        [Process("Clk")]
        public void Log()
        {


            if (((double)Simulator.GetInstance().Time * Simulator.GetInstance().TimeUnit) >= StopTime)
            {
                samplecount = 0;
            }

            if (samplecount < 0)
                return;

            int pr;
            if (samplecount == 0)
            {
                samplecount--;
                //Simulator.GetInstance().SendMessage("]\n");
               // Simulator.GetInstance().SendMessage("Clock cycles " + NextGenLab.Chart.GraphMath.GetPrettyString((double)clk) + "\n");
                if(StopOnFinish)
                    Simulator.GetInstance().Stop();
                return;
            }


            if(!(Clk.Read() ^Rising.Read()))
                clk++;

            if (Wait < Simulator.GetInstance().Time)
            {
                pr = (int)Math.Round(((double)100 * (maxsamples - samplecount)) / (double)maxsamples);
                if (pr > percentage && pr != 0 && this.StopOnFinish)
                {
                    percentage += 2;
                    Simulator.GetInstance().UpdateProgress( percentage );
                }

                if (!(Clk.Read() ^ Rising.Read()) )
                {
                    owb.WriteValues();
                    samplecount--;
                }
            }
        }

        /// <summary>
        /// Writes signals to file
        /// </summary>
        public virtual void WriteValues()
        {
            
        }



        #region IDisposable Members

        public void Dispose ()
        {
            GC.SuppressFinalize(this);
//            if (writerthread != null && writerthread.IsAlive)
//            {
//                running = false;
//                writerthread.Join();
//
//            }
        }

        #endregion
    }
}
