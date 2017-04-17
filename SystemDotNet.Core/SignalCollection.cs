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
    /// Collection of signals
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SignalCollection<T> : SignalBase
    {
        //Signals in the collection
        List<Signal<T>> signals;

        /// <summary>
        /// Number of signals in the collection
        /// </summary>
        /// <value></value>
        public int Count { get { return signals.Count; } }


        /// <summary>
        /// Default constructor. Makes a new signalcollection of a certain length
        /// </summary>
        /// <param name="length">Length of collection</param>
        public SignalCollection(int length)
        {
            //Create new signal list
            signals = new List<Signal<T>>(length);

            Signal<T> sig;
            for (int i = 0; i < length; i++)
            {
                //Create a new signal
                sig = new Signal<T>();

                //Change the sensitive type
                sig.SensType = SensitiveType.ValueChanged;

                //Listen to the Changed event
                sig.Changed += delegate { FireChanged(); };

                //Add to the signals list
                signals.Add(sig);
            }
        }

        public Signal<T> this[int index]
        {
            get { return signals[index]; }
        }

        public List<Signal<T>> Signals { get { return signals; } }

        /// <summary>
        /// Add a listener
        /// </summary>
        /// <param name="c"></param>
        internal override void AddListener(SignalBase c)
        {
            //Cast c
            SignalCollection<T> sc = c as SignalCollection<T>;

            //Add listeners between the signals
            for (int i = 0; i < signals.Count; i++)
            {
                if (i < sc.signals.Count)
                    signals[i].AddListener(sc.signals[i]);
            }
        }


        internal override void RemoveListener(SignalBase c)
        {
            //Cast c
            SignalCollection<T> sc = c as SignalCollection<T>;

            //Remove listeners between the signals
            for (int i = 0; i < signals.Count; i++)
            {
                if (i < sc.signals.Count)
                    signals[i].RemoveListener(sc.signals[i]);
            }
        }

        /// <summary>
        /// Reads the signal collection
        /// </summary>
        /// <returns></returns>
        public virtual T[] Read()
        {
            T[] vals = new T[signals.Count];
            for (int i = 0; i < signals.Count; i++)
            {
                vals[i] = signals[i].Read();
            }
            return vals;
        }

        /// <summary>
        /// Writes to the signal collection
        /// </summary>
        /// <param name="values"></param>
        public virtual void Write(T[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if (i < signals.Count)
                    signals[i].Write( values[i]);
            }
        }

        /// <summary>
        /// Writes to the signal collection with a certain delay
        /// </summary>
        /// <param name="values"></param>
        /// <param name="delay"></param>
        public virtual void Write(T[] values, long delay)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if (!(i < signals.Count))
                    break;

                signals[i].Write(values[i],delay);
            }
        }

        /// <summary>
        /// Writes to the signal collection with a offset
        /// </summary>
        /// <param name="values"></param>
        /// <param name="delay"></param>
        /// <param name="startindex"></param>
        public virtual void Write(T[] values, long delay, int offset)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if (!(offset < signals.Count))
                    break;

                signals[offset].Write(values[i],delay);
                offset++;
            }
        }

        /// <summary>
        /// Converts the signal collection into a string. If it is a bool signal collection it writes the value as a binary value.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this is SignalCollection<bool>)
            {
                SignalCollection<bool> sc = this as SignalCollection<bool>;
                uint u;
                Convert(sc.Read(),out u);
                //string s;
                //Convert(sc.Read(), out s);
                return u.ToString();
            }
            else
            {
                return base.ToString();
            }
        }



        /// <summary>
        /// Converts from bool array into string value, i.e 01010
        /// </summary>
        /// <param name="inp"></param>
        /// <param name="val"></param>
        public static void Convert(bool[] inp, out string val)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < inp.Length; i++)
            {
                if (inp[i])
                    sb.Append('1');
                else
                    sb.Append('0');
            }
            val = sb.ToString();
        }

        /// <summary>
        /// Converts from bool array into unsigned integer, i.e true,false,false = 4;
        /// </summary>
        /// <param name="inp"></param>
        /// <param name="val"></param>
        public static void Convert(bool[] inp, out uint val)
        {
            val = 0;
            for (int i = 0; i < inp.Length; i++)
            {
                if (inp[i])
                    val += (uint)Math.Pow(2, (inp.Length - 1) - i);
            }
        }

        /// <summary>
        /// Converts from unsigned integer into bool array with a number of bits
        /// </summary>
        /// <param name="val"></param>
        /// <param name="outp"></param>
        /// <param name="bits"></param>
        public static void Convert(uint val, out bool[] outp, int bits)
        {
            outp = new bool[bits];
            double rest = val & (uint)(Math.Pow(2, bits) - 1);
            double divisor;
            double quotient;
            for (int i = 0; i < bits; i++)
            {
                divisor = Math.Pow(2, bits - 1 - i);
                quotient = Math.Floor(rest / divisor);
                rest = rest % divisor;
                if (quotient >= 1)
                    outp[i] = true;
                else
                    outp[i] = false;

            }
        }

        /// <summary>
        /// Conforms to the Logger.ValueWriterHandler delegate. Writes a bool signal collection as a binary string
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string ValueWriterBinary(object o)
        {
            SignalCollection<bool> sc = o as SignalCollection<bool>;
            if (sc != null)
            {
                string s;
                SignalCollection<bool>.Convert(sc.Read(), out s);
                return s;
            }
            else
                return o.ToString();

        }

        /// <summary>
        /// Conforms to the Logger.ValueWriterHandler delegate. Writes a bool signal collection as an unsigned integer
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string ValueWriterUnsignedInteger(object o)
        {
            SignalCollection<bool> sc = o as SignalCollection<bool>;
            if (sc != null)
            {
                uint s;
                SignalCollection<bool>.Convert(sc.Read(), out s);
                return s.ToString();
            }
            else
                return o.ToString();

        }

        public override object GetValue()
        {
            object[] o = new object[signals.Count];

            for (int i = 0; i < signals.Count; i++)
            {
                o[i] = signals[i].GetValue();
            }
            return o;
        }

    }
}
