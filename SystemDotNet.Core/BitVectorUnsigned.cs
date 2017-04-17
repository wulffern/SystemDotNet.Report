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
    public class BitVectorUnsigned:Signal<ulong>
    {
        int bits = 0;
        ulong maxvalue = 0;

        public int Bits { get { return bits; } }

        public event EmptyHandler Overflow;

        public override ulong Value
        {
            get
            {
                return base.Value;
            }

            set
            {
                base.Value = value & maxvalue;

                if (value > maxvalue && Overflow != null)
                    Overflow();
            }
        }

        public BitVectorUnsigned( SignalDirectionType st,int bits):this(bits,0,st){ }

        public BitVectorUnsigned(int bits):this(bits,0,SignalDirectionType.InputOutput){}

        public BitVectorUnsigned(int bits, uint defaultvalue, SignalDirectionType st):base(st,defaultvalue)
        {
            if (bits > 64)
                throw new ArgumentException("BitVectorUnsigned has a maximum size of 64");

            this.bits = bits;
            maxvalue = (ulong)(Math.Pow(2, bits) - 1);

            AllowedTypes.Add(typeof(SignalCollection<bool>));
        }

        #region Operator Overloading

        public static ulong operator &(BitVectorUnsigned sv1, BitVectorUnsigned sv2) { CheckSize(sv1, sv2); return AdjustToBits(sv1.bits,sv1.Value & sv2.Value); }
        public static ulong operator |(BitVectorUnsigned sv1, BitVectorUnsigned sv2) { CheckSize(sv1, sv2); return AdjustToBits(sv1.bits,sv1.Value | sv2.Value); }
        public static ulong operator ^(BitVectorUnsigned sv1, BitVectorUnsigned sv2) { CheckSize(sv1, sv2); return AdjustToBits(sv1.bits,sv1.Value ^ sv2.Value); }
        public static ulong operator ~(BitVectorUnsigned sv1) { return AdjustToBits(sv1.bits,~sv1.Value); }
        public static ulong operator +(BitVectorUnsigned sv1, BitVectorUnsigned sv2) { CheckSize(sv1, sv2); return AdjustToBits(sv1.bits,sv1.Value + sv2.Value); }
        public static ulong operator -(BitVectorUnsigned sv1, BitVectorUnsigned sv2) { CheckSize(sv1, sv2); return AdjustToBits(sv1.bits,sv1.Value - sv2.Value); }
        public static ulong operator *(BitVectorUnsigned sv1, BitVectorUnsigned sv2) { CheckSize(sv1, sv2); return AdjustToBits(sv1.bits*2,sv1.Value * sv2.Value); }
        public static ulong operator /(BitVectorUnsigned sv1, BitVectorUnsigned sv2) { CheckSize(sv1, sv2); return AdjustToBits(sv1.bits,sv1.Value / sv2.Value); }

        public static implicit operator string(BitVectorUnsigned sv)
        {
            return sv.ToString();
        }
        public static implicit operator bool[](BitVectorUnsigned sv)
        {
            return sv.GetBool();
        }

        public static implicit operator ulong(BitVectorUnsigned sv)
        {
            return sv.Value;
        }
        public static implicit operator uint ( BitVectorUnsigned sv )
        {
            return (uint)sv.Value;
        }

        static void CheckSize(BitVectorUnsigned bv1, BitVectorUnsigned bv2)
        {
            if(bv1.Bits != bv2.bits)
                throw new ArgumentException("Bit vectors cannot be of different size. " + bv1.FullName+ ": " + bv1.Bits + ", " +bv2.FullName +": " + bv2.Bits);
        }
        
        static ulong AdjustToBits(int bits, ulong value)
        {
            return value & (ulong)(Math.Pow(2, bits) - 1);
        }

        #endregion

        internal override void AddListener(SignalBase c)
        {
            if (c is SignalCollection<bool>)
            {
                c.Changed += delegate { this.Value = Convert(((SignalCollection<bool>)c).Read()); };
            }
            else
            {
                base.AddListener(c);
            }
        }

        internal override void RemoveListener(SignalBase c)
        {
            if (c is SignalCollection<bool>)
            {
                c.Changed -= delegate { this.Value = Convert(((SignalCollection<bool>)c).Read()); };
            }
            else
            {
                base.AddListener(c);
            }
        }

        public void Connect(int index, Signal<bool> sig)
        {
            sig.Changed += delegate
            {
                ulong val = (ulong)Math.Pow(2, index);
                if (sig.Read())
                {
                    this.Value = Value | val;
                }
                else
                {
                    this.Value = Value & ~val;
                }
            };
        }


        public virtual void Write(string str)
        {
            Write(str, 1);
        }

        public virtual void Write(string str,long delay)
        {
            StringBuilder sb = new StringBuilder(str);

            PadToBitlength(sb);

            ulong val = 0;
            for(int i=bits-1;i>=0;i--){

                if(sb[i] == '1')
                    val += (ulong)Math.Pow(2,bits - i - 1);
            }

            base.Write(val,delay);
        }

        public string ToBinaryString()
        {
            return GetString();

        }

        public static ulong Convert(bool[] barr)
        {
            ulong val = 0;
            for (int i = 0; i < barr.Length; i++)
            {
                if (barr[i])
                    val += (ulong)Math.Pow(2, (barr.Length - 1) - i);
            }
            return val;
        }

        string GetString()
        {

            StringBuilder sb = new StringBuilder();
            double rest = Value;
            double divisor;
            double quotient;
            for (int i = 0; i < bits; i++)
            {
                divisor = Math.Pow(2, bits - 1 - i);
                quotient = Math.Floor(rest / divisor);
                rest = rest % divisor;
                if (quotient >= 1)
                    sb.Append("1");
                else
                    sb.Append("0");
            }

            PadToBitlength(sb);

            return sb.ToString();
        }

        void PadToBitlength(StringBuilder sb)
        {
            if (sb.Length< bits)
                for (int i = 0; i < bits; i++)
                {
                    sb.Insert(0, 0);
                }
        }

        bool[] GetBool()
        {
            bool[] outp = new bool[bits];
            double rest = Value;
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
            return outp;

        }


    }
}
