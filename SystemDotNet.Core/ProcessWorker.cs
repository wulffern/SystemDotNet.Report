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
using System.Threading;
using System.Reflection;

#endregion

namespace SystemDotNet
{
    public class ProcessWorker
    {
        EmptyHandler eh;
        bool threaded = false;
       // AutoResetEvent ev = new AutoResetEvent(false);
        Simulator sim;
        bool stopthread = false;

        string infostring;

        public ProcessWorker(ModuleBase module, MethodInfo mi)
        {
            sim = Simulator.GetInstance();
            eh = (EmptyHandler)Delegate.CreateDelegate(typeof(EmptyHandler), module, mi.Name);

            infostring = module.Name + "(" + mi.Name + ")";

            object[] o = mi.GetCustomAttributes(typeof(ThreadedProcessAttribute), true);
            //if (o.Length > 0 && Simulator.UseThreadedProcess)
            //{
            //    threaded = true;
            //    Thread t = new Thread(new ThreadStart(RunThread));
            //    t.Start();
            //}
           // sim.OnStop +=new EmptyHandler(sim_OnStop);
        }

        public void Run()
        {
            if (!threaded || !Simulator.UseThreadedProcess)
            {
                try
                {
                    eh();
                }
                catch (CommandException ce)
                {
                    throw ce;
                }
                catch(Exception ex)
                {
                    throw new Exception("An unhandled exception occured in " + infostring + " of type: " + ex.Message, ex);
                }
                
            }
            else
            {
              //  ev.Set();
                //Thread t = new Thread(new ThreadStart(RunThread));
                //t.Start();
             //   Thread.Sleep(0);

            }
        }

        //public void RunThread()
        //{
        //    while (true)
        //    {
               
        //        ev.WaitOne();
        //        sim.AddWorker(this);
        //        if (stopthread)
        //            break;
        //        try
        //        {
        //            eh();
        //        }
        //        catch 
        //        {
        //            sim.Stop();
        //        }
        //        sim.RemoveWorker(this);
        //        ev.Reset();
        //    }
        //}

        //void sim_OnStop()
        //{
        //    stopthread = true;
        //    ev.Set();
        //}
    }
}
