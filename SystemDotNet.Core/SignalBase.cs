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
using SystemDotNet;
using System.Collections.Generic;
using System.Reflection;

namespace SystemDotNet
{
	/// <summary>
	/// Base for all signals
	/// </summary>
	public abstract class SignalBase
	{

        List<SignalBase> left = new List<SignalBase>();
        List<SignalBase> right = new List<SignalBase>();

        //Unique id of the signal
		Guid uniqueid = Guid.NewGuid();

        //Signal name
		string name;

        //Signal fullname
		string fullname;

        //Event has occured on signal
        bool ev = false;

        //Sensitivity type of this signal
        SensitiveType senstype = SensitiveType.ValueChanged;

        protected List<Type> AllowedTypes = new List<Type>();
        public List<SignalBase> Left { get { return left; } }
        public List<SignalBase> Right { get { return right; } }

        SignalDirectionType signaldirection = SignalDirectionType.InputOutput;

        public SignalDirectionType SignalDirection { get { return signaldirection; } set { signaldirection = value; } }


        /// <summary>
        /// What sensitivity this signal should have
        /// </summary>
        /// <value></value>
        public SensitiveType SensType{ get { return senstype; } set { senstype = value; } }

        /// <summary>
        /// Markes the signal as the originator of the current event. I.e if(Clk.Event && Clk.Read())
        /// </summary>
        /// <value></value>
        public bool Event { get { return ev; } }

        /// <summary>
        /// Name of the signal
        /// </summary>
        /// <value></value>
        public string Name{get{return name;}set{name = value;}}

        /// <summary>
        /// Full name of the signal
        /// </summary>
        /// <value></value>
		public string FullName{get{return fullname;}set{fullname = value;}}

        /// <summary>
        /// Signals unique ID
        /// </summary>
        /// <value></value>
		public Guid UniqueID{get{return uniqueid;}}

        /// <summary>
        /// Event called when the signal changes/is updated. When it is called depends on the sensitivity type.
        /// </summary>
		public event EmptyHandler Changed;

        protected Simulator Sim;

        /// <summary>
        /// Constructor, not much happening here
        /// </summary>
        public SignalBase() { Sim = Simulator.GetInstance(); }

        /// <summary>
        /// Fires the Changed event
        /// </summary>
        protected void FireChanged()
        {
            ev = true;
            if (Changed != null)
                Changed();
            ev = false;
        }

        /// <summary>
        /// Connects this signal to another signal
        /// </summary>
        /// <param name="c">Signal that this signal should listen too</param>
        public void Connect(SignalBase c)
		{
			if(c == null)
				throw new ArgumentException( this.GetType() +  ": Cannot connect,SignalBase is null");

            bool inheritanceok = false;
            if(c.GetType().IsSubclassOf(this.GetType()) || this.GetType().IsSubclassOf(c.GetType()))
                inheritanceok =true;

            bool typeok = false;

            foreach (Type t in AllowedTypes)
            {
                if (t.Equals(c.GetType()))
                    typeok = true;
            }


            if(c.GetType() != this.GetType() && (!inheritanceok && !typeok) )
				throw new ArgumentException("SignalBase " + c.FullName + " is of type " + c.GetType().ToString() + " while I'm " + this.GetType());

			if(c == this)
				throw new Exception("You're trying to connect a channel to itself, that is not allowed! " + c.GetType());

            if(!left.Contains(c))
                left.Add(c);
            if(!c.right.Contains(this))
                c.right.Add(this);

            AddListener(c);
        }

        /// <summary>
        /// Disconnect this signal from another signal
        /// </summary>
        /// <param name="c">Signal to remove</param>
        public void DisConnect(SignalBase c)
		{
            if (left.Contains(c))
                left.Remove(c);

            if (c.right.Contains(this))
                c.right.Remove(c);

            RemoveListener(c);
        }

        /// <summary>
        /// Add listener for a signal
        /// </summary>
        /// <param name="c"></param>
        internal abstract void AddListener(SignalBase c);

        /// <summary>
        /// Remove listener for a signal
        /// </summary>
        /// <param name="c"></param>
        internal abstract void RemoveListener(SignalBase c);

        public abstract object GetValue();


        /// <summary>
        /// Determines if this object is equal to another object. Uses the UniqueID to determine this.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
		{
            if (!(obj is SignalBase))
                return false;
            else
            {
                SignalBase b = (SignalBase)obj;

                if (b.UniqueID.Equals(this.UniqueID))
                    return true;
                else
                    return false;
            }
		}

        /// <summary>
        /// Uses the UniqueID to return the hashcode for this class
        /// </summary>
        /// <returns></returns>
		public override int GetHashCode()
		{
			return UniqueID.GetHashCode();
		}
    }
}
