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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace SystemDotNet
{
    /// <summary>
    /// Signal definition
    /// </summary>
    public class Signal<T> : SignalBase, ISignal, IValue<T>
    {
        T val;
        public virtual T Value { get { return val; } set { val = value; FireChanged(); } }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Signal() { }

        /// <summary>
        /// Constructor with default value
        /// </summary>
        /// <param name="defaultvalue"></param>
        public Signal(T defaultvalue)
        {
            val = defaultvalue;
        }

        public Signal(SignalDirectionType st)
        {
            SignalDirection = st;
        }

        /// <summary>
        /// Constructor with default value
        /// </summary>
        /// <param name="defaultvalue"></param>
        public Signal(SignalDirectionType st, T defaultvalue)
        {
            SignalDirection = st;
            val = defaultvalue;
        }

        /// <summary>
        /// Adds listener to signal
        /// </summary>
        /// <param name="c"></param>
        internal override void AddListener(SignalBase c)
        {
            ((Signal<T>)c).Changed += delegate { this.Value = ((Signal<T>)c).Value; };
        }

        /// <summary>
        /// Removes listener from signal
        /// </summary>
        /// <param name="c"></param>
        internal override void RemoveListener(SignalBase c)
        {
            ((Signal<T>)c).Changed -= delegate { this.Value = ((Signal<T>)c).Value; };
        }

        /// <summary>
        /// Write a value to the signal with a certain delay. 0 delay is allowed. 
        /// </summary>
        /// <param name="Value">value to write</param>
        /// <param name="delay">delay before writing</param>
        public virtual void Write(T Value, long delay)
        {
            //if (this.SignalDirection == SignalDirectionType.Input)
            //   throw new CommandException("You are trying to write to the input of " + this.Name);

            if (delay < 0)
                throw new CommandException("Negative delay is not allowed. It would sort of ruin the causality of things :-)");

            //Make new time event
            TimeEvent<T> te = new TimeEvent<T>(this, Value);

            //Add it to the event queue
            Sim.Write(te, delay);
        }

        /// <summary>
        /// Write a value to the signal
        /// </summary>
        /// <param name="Value"></param>
        public virtual void Write(T Value)
        {
            //Make new time event
            TimeEvent<T> te = new TimeEvent<T>(this, Value);

            //Add it to the event queue
            Sim.Write(te);
        }

        /// <summary>
        /// Read the value of a signal
        /// </summary>
        /// <returns>value </returns>
        public virtual T Read()
        {
            return val;
        }

        /// <summary>
        /// Read the value of a signal as an object
        /// </summary>
        /// <returns></returns>
        public object ReadObject()
        {
            return val;
        }

        /// <summary>
        /// Returns the string from the values ToString()
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (val != null)
                return val.ToString();
            else
                return "";
        }

        public override object GetValue()
        {
            return val;
        }

    }
}
