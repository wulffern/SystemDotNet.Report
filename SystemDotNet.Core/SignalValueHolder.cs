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

#endregion

namespace SystemDotNet
{
    /// <summary>
    /// Encapsulates the value of the signal
    /// </summary>
    /// <typeparam name="K"></typeparam>
    public class SignalValueHolder<K>:IValue<K>
    {

        //The value
        K obj;

        //Signal the valueholder belongs to
        Signal<K> sig;

        SensitiveType senstype = SensitiveType.ValueChanged;

        public SensitiveType SensType { get { return senstype; } set { senstype = value; } }

        //How valueUpdated handlers should look
        public delegate void ValueUpdatedHandler(SignalValueHolder<K> sender);

        //Called when value is updated/changed
        public event ValueUpdatedHandler ValueUpdated;

        //Called when value is updated/changed
        public event EmptyHandler Changed;

        //Unique id for the valueholder
        public Guid uniqueID = Guid.NewGuid();

        public Signal<K> Signal { get { return sig; } }

        /// <summary>
        /// The value
        /// </summary>
        /// <value></value>
        public K Value
        {
            get { return obj; }
            set
            {
                //If the the signal senstype is valuechanged and the value is unchanged don't call events
              //  if (sig.SensType == SensitiveType.ValueChanged)
               // {
               //     if (obj.Equals(value))
               //         return;
               // }

                //Set new value
                obj = value;

//                if (sig.Edge != EdgeType.Either && obj is bool )
//                {
//                    switch (sig.Edge)
//                    {
//                        case EdgeType.Rising:
//                            if (value.Equals(false))
//                                return;
//                            break;
//                        case EdgeType.Falling:
//                            if (value.Equals(true))
//                                return;
//                            break;
//
//                    }
//                }

                

                //Call valueupdated
                if (ValueUpdated != null) ValueUpdated(this);

                //Call Changed
                if (Changed != null) Changed();
            }
        }

        /// <summary>
        /// Constructor with default value
        /// </summary>
        /// <param name="sig">Signal the value holder belongs to</param>
        /// <param name="obj">Default value</param>
        public SignalValueHolder(Signal<K> sig, K obj):this(sig)
        {

            this.obj = obj;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="sig">Signal the value holder belongs to</param>
        public SignalValueHolder ( Signal<K> sig ) { this.sig = sig; senstype = sig.SensType; }
    }
}
