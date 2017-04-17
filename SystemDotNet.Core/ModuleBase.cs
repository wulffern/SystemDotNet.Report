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
using System.Collections.Generic;
using System.Reflection;
using SystemDotNet;
using System.Diagnostics;
using System.ComponentModel;

namespace SystemDotNet
{
    /// <summary>
    /// Base class for all modules.
    /// </summary>
    public abstract class ModuleBase
    {
        //Name of the module
        string name;

        string description;

        //Children in this module
        List<ModuleBase> childs = new List<ModuleBase>();

        //Signals in this module
        List<SignalBase> sigs = new List<SignalBase>();

        //Parent of this module
        ModuleBase parent = null;

        private bool initComplete;

        public bool InitComplete
        {
            get { return initComplete; }
            set { initComplete = value; }
        }


        /// <summary>
        /// Name of this module
        /// </summary>
        /// <value></value>
        public string Name { get { return name; } set { name = value; } }

        public string Description { get { return description; } set { description = value; } }

        /// <summary>
        /// Children of this module
        /// </summary>
        /// <value></value>
        public List<ModuleBase> Children { get { return childs; } }

        /// <summary>
        /// Signals in this module
        /// </summary>
        /// <value></value>
        [Browsable(false)]
        public List<SignalBase> Signals { get { return sigs; } }

        /// <summary>
        /// Parent of this module
        /// </summary>
        /// <value></value>
        public ModuleBase Parent { get { return parent; } set { parent = value; } }

        /// <summary>
        /// Sets the name equal to the type;
        /// </summary>
        //public ModuleBase():this(null)
        //{
        //}

        public ModuleBase(ModuleBase parent)
        {
            if (name == null)
                name = this.GetType().Name.ToString();
            Simulator.GetInstance().Init += delegate { Initialize(); };
            Simulator.GetInstance().PreInit += delegate { OnInit(); };
            this.Parent = parent;
        }

        /// <summary>
        /// Called on initialize
        /// </summary>
        public virtual void OnInit() { }
        public virtual void PostInit() { }

        /// <summary>
        /// Called after initialize
        /// </summary>
        public virtual void OnLoad() { }

        /// <summary>
        /// Called when the simulator stops
        /// </summary>
        public virtual void OnStop() { }

        /// <summary>
        /// Called on the first step
        /// </summary>
        public virtual void First() { }

        /// <summary>
        /// Initializes the module. Loads all modules and signals
        /// </summary>
        /// <returns></returns>
        public void Initialize()
        {

            //Instance of the simulator
            Simulator sim = Simulator.GetInstance();

            //Used for errors, false if errors
            bool ok = true;

            if (Parent != null)
            {
                Parent.Children.Add(this);

                this.name = Parent.Name + "." + this.name;

                int i = 0;
                foreach (ModuleBase mb in Parent.Children)
                {
                    if (mb.name.StartsWith(this.name))
                        i++;
                }
                this.name += i.ToString();
            }

            //Initialize arrays
            sigs = new List<SignalBase>();

            //Add handler for simulator first step
            sim.FirstStep += new EmptyHandler(First);

            //Add handler for simulator before stop
            sim.OnStop += new EmptyHandler(OnStop);

            //Add handler for simulator after loading
            sim.Load += new EmptyHandler(OnLoad);

            //Add module to simulator
           // sim.AddModule(this);

            //Holder for signals found
            Dictionary<string, SignalBase> signals = new Dictionary<string, SignalBase>();

            //All fields in this class
            FieldInfo[] fis = this.GetType().GetFields();

            //Find all signals in this module and store them
            foreach (FieldInfo fi in fis)
            {
                Type t = fi.FieldType;

                //Check if the type is a subclass of Signalbase
                if (t.IsSubclassOf(typeof(SignalBase)))
                {
                    //Get the value of the filed
                    object o = this.GetType().InvokeMember(fi.Name, BindingFlags.GetField, null, this, new object[] { });

                    //If the value is null there is something
                    if (o == null)
                    {
                        sim.SendError(this.GetType() + ": Connection Error: " + fi.Name + " has no instance.");
                        ok = false;
                        continue;
                    }

                    //Cast to signalBase
                    SignalBase c = (SignalBase)o;

                    //Sets the fullname of the signal
                    if (this.Name == null)
                        c.FullName = fi.Name;
                    else
                        c.FullName = this.Name + "." + fi.Name;

                    //Sets the name of the signal
                    c.Name = fi.Name;

                    //Adds the signal to the module
                    Signals.Add(c);

                    //Stores the signals
                    signals.Add(fi.Name, c);
                }
            }

            //Find all signals that are properties and store them
            PropertyInfo[] pis = this.GetType().GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                Type t = pi.PropertyType;

                //Check if the type is a subclass of Signalbase
                if (t.IsSubclassOf(typeof(SignalBase)))
                {
                    //Get the value of the filed
                    object o = pi.GetValue(this, new object[] { });

                    //If the value is null there is something
                    if (o == null)
                    {
                        sim.SendError(this.GetType() + ": Connection Error: " + pi.Name + " has no instance.");
                        ok = false;
                        continue;
                    }

                    //Cast to signalBase
                    SignalBase c = (SignalBase)o;

                    if (Signals.Contains(c))
                        continue;

                    //Sets the fullname of the signal
                    if (this.Name == null)
                        c.FullName = pi.Name;
                    else
                        c.FullName = this.Name + "." + pi.Name;

                    //Sets the name of the signal
                    c.Name = pi.Name;

                    //Adds the signal to the module
                    Signals.Add(c);

                    //Stores the signals
                    signals.Add(pi.Name, c);
                }

            }

            //Get all methods
            MethodInfo[] mis = this.GetType().GetMethods();
            //EmptyHandler eh;
            ProcessWorker pw;
            foreach (MethodInfo mi in mis)
            {
                object[] o = mi.GetCustomAttributes(typeof(ProcessAttribute), true);
                //If the method uses the process attribute
                if (o.Length > 0)
                {
                    //Run through all sensitive attributes
                    foreach (ProcessAttribute sa in o)
                    {
                        //Run through all signal names in process attribute
                        foreach (string name in sa.SignalNames)
                        {
                            if (signals.ContainsKey(name))
                            {
                                //If the method is sensitive to the signal add it to the signals list of methods to run
                                signals[name].SensType = sa.SensType;
                                //eh = (EmptyHandler)Delegate.CreateDelegate(typeof(EmptyHandler), this, mi.Name);
                                pw = new ProcessWorker(this, mi);
                                signals[name].Changed += new EmptyHandler(pw.Run);
                            }
                            else
                            {
                                //If the signal was not found call an error
                                sim.SendError(this.GetType() + ": Senstitivity Error: Could not find signal " + name + " that is needed by " + mi.Name);
                                ok = false;
                            }
                        }
                    }

                }
            }

            if (!ok)
                throw new CommandException("Initialize of " + this.Name + " failed");

            InitComplete = true;
        }

        /// <summary>
        /// Unloads this module
        /// </summary>
        public void Unload()
        {
            //Removes Eventhandlers
            Simulator sim = Simulator.GetInstance();
            sim.FirstStep -= new EmptyHandler(First);
            sim.OnStop -= new EmptyHandler(OnStop);

            //Unregisters all signals
            //UnTrace(this);
        }

        #region Code for Commands
        public void SetProperty(string name, string val, bool CaseSensitive)
        {
            bool propertyset = false;
            System.Reflection.PropertyInfo[] pis = this.GetType().GetProperties();
            foreach (System.Reflection.PropertyInfo pi in pis)
            {
                if (CaseSensitive)
                {
                    if (pi.Name != name)
                        continue;
                }
                else
                    if (pi.Name.ToLower() != name.ToLower())
                        continue;

                object[] o = pi.GetCustomAttributes(typeof(ModulePropertyAttribute), true);
                if (o.Length == 0)
                    continue;
                SetProperty(pi, val);
                propertyset = true;
            }

            if (!propertyset)
                throw new CommandException("Could not find property with name " + name);
        }

        void SetProperty(System.Reflection.PropertyInfo pi, string val)
        {
            object oval = null;
            switch (pi.PropertyType.FullName)
            {
                case "System.Double":
                    oval = Convert.ToDouble(val);
                    break;
                case "System.Int64":
                    oval = Convert.ToInt64(val);
                    break;
                case "System.Int32":
                    oval = Convert.ToInt32(val);
                    break;
                case "System.String":
                    oval = val;
                    break;
                case "System.Boolean":
                    oval = Convert.ToBoolean(val);
                    break;
                case "SystemDotNet.ModuleBase":
                    ModuleBase m = GetModule(Simulator.GetInstance().Modules, val, false);
                    if (m == null)
                        throw new CommandException("Could not find module " + val + ". It might not be loaded");
                    oval = m;
                    break;
            }

            if (oval != null)
            {
                pi.SetValue(this, oval, null);

            }
            else
            {
                throw new CommandException("Unknown property type " + pi.PropertyType.FullName);
            }


        }

        public static ModuleBase GetModule(List<ModuleBase> ms, string name, bool CaseSensitive)
        {
            ModuleBase mis;
            foreach (ModuleBase m in ms)
            {
                mis = m.GetModule(name, CaseSensitive);
                if (mis != null)
                    return mis;
            }
            return null;
        }

        public ModuleBase GetModule(string name, bool CaseSensitive)
        {
            return GetModule(this, name, CaseSensitive);
        }

        ModuleBase GetModule(ModuleBase parent, string name, bool CaseSensitive)
        {
            if (CaseSensitive)
            {
                if (parent.Name == name)
                    return parent;
            }
            else
            {
                if (parent.Name.ToLower() == name.ToLower())
                    return parent;
            }

            ModuleBase ms;
            foreach (ModuleBase m in parent.Children)
            {
                ms = GetModule(m, name, CaseSensitive);
                if (ms != null)
                    return ms;
            }

            return null;
        }


        public SignalBase GetChannel(string name)
        {
            return GetChannel(this, name);
        }

        SignalBase GetChannel(ModuleBase parent, string name)
        {
            foreach (SignalBase c in parent.Signals)
            {
                if (c.FullName == name)
                    return c;
            }

            foreach (ModuleBase m in parent.Children)
            {
                return m.GetChannel(name);
            }
            return null;
        }

        #endregion
    }
}
