#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace SystemDotNet
{
    public class UnsignedBitVector:SignalCollection<bool>
    {
        public UnsignedBitVector(int length):base(length)
        {
        }


        public new uint Read()
        {
            return ToUint(base.Read());
        }

        public void Write(uint val)
        {
            bool[] outp = new bool[this.Count];
            ToBool(val, ref outp);
            base.Write(outp);
        }

        public void Write(uint val,long delay)
        {
            bool[] outp = new bool[this.Count];
            ToBool(val, ref outp);
            base.Write(outp,delay);
        }

        public static uint ToUint(bool[] inp)
        {
            uint val = 0;
            for (int i = 0; i < inp.Length; i++)
            {
                if (inp[i])
                    val += (uint)Math.Pow(2, (inp.Length - 1) - i);
            }
            return val;
        }

        public static void ToBool(uint val, ref bool[] outp)
        {
            int bits = outp.Length;
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
    }
}
