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
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.ComponentModel;

namespace SystemDotNet
{
    /// <summary>
    /// Simulator core. Controls running of the event queue
    /// </summary>
    public class Simulator : EventQueue
    {
        //Is simulator running
        bool running = true;

        //Time to stop
        long stop = -1;

        //Initialize
        bool init = false;

        static bool usethreadedprocess;

        //Simulator
        static Simulator sim;
        static Variables variables;
        public static Variables Settings
        {
            get
            {
                if (variables == null)
                    variables = new Variables();
                return variables;
            }
        }
        //Simualtion thread
        Thread simthread;

        //State if the simulator
        State SimState = State.Idle;

        //Loaded modules
        List<ModuleBase> modules = new List<ModuleBase>();
        List<ModuleBase> tracmodules = new List<ModuleBase>();
        List<int> tracelevels = new List<int>();

        /// <summary>
        /// Tells all modules to initialize
        /// </summary>
        public event EmptyHandler Init;

        /// <summary>
        /// Fired before a step
        /// </summary>
        public event EmptyHandler PreStep;

        /// <summary>
        /// Fired after a step
        /// </summary>
        public event EmptyHandler PostStep;

        //public event EmptyHandler ModulesLoaded;


        public event EmptyHandler Load;

        /// <summary>
        /// Fired when the simulator is about to stop
        /// </summary>
        public event EmptyHandler OnStop;

        public event EmptyHandler OnStopped;

        public event EmptyHandler PreInit;
        public event EmptyHandler PostInit;

        /// <summary>
        /// Fired when a error has occured
        /// </summary>
        public event StringHandler Error;

        /// <summary>
        /// Fired when a message has been sent
        /// </summary>
        public event StringHandler Message;

        public event ProgressHandler Progress;

        [Browsable(false)]
        public State SimulatorState { get { return SimState; } }

        [Obsolete("Implementation of this function does not work with mono. In addition no speed increase has been seen with threaded processes")]
        [CategoryAttribute("Simulator Settings"),
              DescriptionAttribute("Use Threaded processes or not")]
        public static bool UseThreadedProcess { get { return usethreadedprocess; } set { usethreadedprocess = value; } }

        /// <summary>
        /// Loaded Modules
        /// </summary>
        /// <value></value>
        [Browsable(false)]
        public List<ModuleBase> Modules { get { return modules; } }


        [Browsable(false)]
        public Thread SimulationThread { get { return simthread; } }

        //State of the simulator
        public enum State { Idle, Start, Running, Paused, Stop, Error }

        private static bool _AutoInstantiate = true;

        public static bool AutoInstantiate
        {
            get { return _AutoInstantiate; }
            set { _AutoInstantiate = value; }
        }


        long start;
        /// <summary>
        /// Default constructor, calls EventQueue constructor and starts simulation thread
        /// </summary>
        private Simulator():base()
        {
            SimState = State.Idle;
            //this.OnStopped += new EmptyHandler(Simulator_OnStopped);
            variables = new Variables();
        }

        /// <summary>
        /// Get the instance of the Simualtor
        /// </summary>
        /// <returns></returns>
        public static Simulator GetInstance()
        {
            if (AutoInstantiate && sim == null)
               sim = new Simulator();
            else if ( !AutoInstantiate && sim == null)
            {
                throw new CommandException("Simulator has not been loaded, please use GetNew() before the first simulator access.");
            }

            return sim;
        }

        /// <summary>
        /// Create a new simulator. But only one simulator can exist at any point in time.
        /// </summary>
        /// <returns></returns>
        public static Simulator GetNew()
        {
            if (sim != null)
                sim.SimState = State.Stop;
            sim = new Simulator();
            return sim;
        }

        public void Initialize()
        {
            if (!init)
            {
                if (PreInit != null)
                    PreInit();

                if (Init != null)
                {


                    try
                    {
                        Init();
                    }
                    catch (CommandException ex)
                    {
                        SendError(ex.Message);
                    }
                }

                if (PostInit != null)
                    PostInit();

                if (Load != null)
                    Load();

                init = true;

                //Load Traces
                for (int i = 0; i < tracmodules.Count; i++)
                {
                    Trace(tracmodules[i], tracelevels[i]);
                }
               
            }
        }

        /// <summary>
        /// Load a module
        /// </summary>
        /// <param name="m"></param>
        public void AddModule(ModuleBase m)
        {


            //if (!m.Initialize())
            //    SimState = State.Error;
            if(!modules.Contains(m))
                modules.Add(m);
        }

        /// <summary>
        /// Load Modules
        /// </summary>
        /// <param name="m"></param>
        public void AddModule(params ModuleBase[] m)
        {
            for (int i = 0; i < m.Length; i++)
            {
                AddModule(m[i]);
            }
        }

        /// <summary>
        /// Unload all Modules
        /// </summary>
        public void Unload()
        {
            //           CheckState();
//
            //          SimState = State.Paused;
            foreach (ModuleBase m in modules)
            {
                m.Unload();
            }
            modules = new List<ModuleBase>();
            //base.Reset();
        }


        /// <summary>
        /// Run simualtion for a certaion amount of time.
        /// </summary>
        /// <param name="time">Amount of time to run</param>
        /// <param name="wait">True; wait for the run to finish. False: returns at once</param>
        public void Run(long time, bool wait)
        {
            if (simthread == null || !simthread.IsAlive)
            {
                // base.Restart();
                SimState = State.Idle;
                running = true;
                simthread = new Thread(new ThreadStart(ThreadRun));
                simthread.Start();
            }

            CheckState();

            stop = this.Time + time;

            SimState = State.Start;

            Thread.Sleep(10);

            start = DateTime.Now.Ticks;


            if (wait)
            {
                while (SimState != State.Idle)
                {
                    Thread.Sleep(100);
                }
            }
        }

        /// <summary>
        /// Run all
        /// </summary>
        /// <param name="wait">True; wait for the run to finish. False: returns at once</param>
        public void Run(bool wait)
        {
            Run(long.MaxValue, wait);
        }

        /// <summary>
        /// Restart the event queue and return to zero time. Must call Run() again to make it start.
        /// </summary>
        public override void Restart()
        {
            CheckState();

            SimState = State.Paused;
            base.Restart();
        }


//        protected override void Reset()
//        {
//            if (simthread != null && simthread.IsAlive)
//            {
//                SimState = State.Stop;
//                simthread.Join();
//            }
//
//            SimState = State.Idle;
//
//            Unload();
//
//            simthread = new Thread(new ThreadStart(ThreadRun));
//            simthread.Start();
//        }


        /// <summary>
        /// Checks the state of the simualtor. Throws exceptions if it has an error or is stopped
        /// </summary>
        void CheckState()
        {
            if (init == false)
                throw new CommandException("Simulator has not been initialized. Please run Simulator.GetInstance().Initialize()");
            if (SimState == State.Error)
                throw new CommandException("There are errors in the modules");
            if (SimState == State.Stop)
                throw new CommandException("Simulation has been stopped, cannot be restarted");
        }

        /// <summary>
        /// Stop the simulator. It cannot be started again after this.
        /// </summary>
        public void Stop()
        {
            SimState = State.Stop;

        }

        /// <summary>
        /// Pause the simulator. Can be started with Run()
        /// </summary>
        public void Pause()
        {
            if (SimState == State.Running)
            {
                SimState = State.Paused;
                SendMessage("Simulator was paused");
            }
            else
            {
                SendMessage("Simulator is not running. SimulatorState: " + SimState.ToString());
            }
        }

        /// <summary>
        /// Main simulation control thread
        /// </summary>
        void ThreadRun()
        {
            while (running)
            {
                if (SimState == State.Running)
                {
                    if (PreStep != null)
                        PreStep();

                    //Step the event queue
                    if (!this.Step())
                    {
                        //Pause the simulator step returns false
                        SimState = State.Stop;
                    }

                    //Pause the simulator if the stop time has been reached
                    if (Time >= stop)
                    {
                        SimState = State.Stop;
                    }

                    if (PostStep != null)
                        PostStep();

                    continue;
                }

                switch (SimState)
                {
                    case State.Idle:
                        Thread.Sleep(1);
                        break;
                    case State.Start:
                        if (this.SignalWriter != null)
                            this.SignalWriter.Open();
                        SimState = State.Running;
                        break;
                    case State.Error:
                        Thread.Sleep(200);
                        break;
                    case State.Paused:
                        Thread.Sleep(1);
                        break;
                    case State.Stop:
                        if (this.SignalWriter != null)
                            this.SignalWriter.Close();
                        if (OnStop != null)
                            OnStop();
                        PrintTimeUsed();
                        //SendMessage(GetStatusShort());
                        //Unload();
                        base.Restart();
                        running = false;
                        if (OnStopped != null)
                            OnStopped();
                        SimState = State.Idle;
                        break;

                }
            }
        }

        public void Trace(ModuleBase mb)
        {
            Trace(mb, 0);
        }

        public void Trace(ModuleBase mb,int level)
        {
            if (!init)
            {
                tracmodules.Add(mb);
                tracelevels.Add(level);

            }
            else
            {
                foreach (SignalBase sb in mb.Signals)
                {
                    Trace(sb);
                }

                if (level > 0)
                {
                    foreach (ModuleBase child in mb.Children)
                    {
                        Trace(child, level-1);
                    }

                }
            }

        }


        /// <summary>
        /// Send a message to whoever is listening
        /// </summary>
        /// <param name="message"></param>
        public void SendMessage(string message)
        {
            if (Message != null)
            {
                Message(message);
            }
            else
                Console.Write(message);
        }

        /// <summary>
        /// Send a error message to whoever is listening
        /// </summary>
        /// <param name="message"></param>
        public void SendError(string message)
        {
            if (Error != null)
                Error(message);
            else
                Console.WriteLine(message);

            SimState = State.Paused;
        }

        public void UpdateProgress(int progress)
        {
            if (Progress != null)
                Progress(progress);
        }



        void PrintTimeUsed()
        {
            long stop = DateTime.Now.Ticks;
            TimeSpan tp = new TimeSpan(stop - start);
            string eventsprsecond = DblToString(this.events / tp.TotalSeconds);
            SendMessage("Complete " + tp.Hours + "h, " + tp.Minutes + "m, " + tp.Seconds + "s, " + tp.Milliseconds + "ms (" + GetStatusShort() + ", E/s = " + eventsprsecond + ")");

        }
    }
}
