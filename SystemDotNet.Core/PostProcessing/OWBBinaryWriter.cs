using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SystemDotNet;

namespace SystemDotNet.PostProcessing
{
    public class OWBBinaryWriter:OutputWriterBase  
    {

        BinaryWriter bw;

        int version = 0;
        string extention = ".bsdr";

        private char _Delimiter = ';';

        public char Delimiter
        {
            get { return _Delimiter; }
            set { _Delimiter = value; }
        }


        public OWBBinaryWriter(string FileName) : base(FileName, true, true, false) { }
        

        public OWBBinaryWriter(string FileName, bool ShowHeader, bool UseAdvancedFileName, bool ShowTime)
            : base(FileName, ShowHeader, UseAdvancedFileName, ShowTime)
        {
         }

        ~OWBBinaryWriter()
        {
            if (bw != null)
                bw.Close();

        }

        protected override void OnClose()
        {
            bw.Close();
        }

        protected override void OnOpen()
        {
            bw = new BinaryWriter(File.OpenWrite(this.FilePath + extention));

            bw.Write(version);

            int columncount = SignalList.Count;

            bw.Write(columncount);            
        }

        protected override void WriteHeader(List<string> header)
        {
            StringBuilder sb = new StringBuilder();

            if (ShowTime)
                sb.Append("Time;");

            foreach (String s in header)
            {
                sb.Append(s + ';');
            }

            sb.Remove(sb.Length - 1, 1);

            char[] headerbuffer = sb.ToString().ToCharArray();

            bw.Write(headerbuffer.Length);
            foreach (char c in headerbuffer)
            {
                bw.Write(c);
            }
        }

        public override void WriteValues()
        {

            if (ShowTime)
            {
                bw.Write((double)Simulator.GetInstance().RealTime);
            }

            for (int i = 0; i < SignalList.Count; i++)
            {
                object o = SignalList[i].GetValue();

               TypeCode tc = Convert.GetTypeCode(o);
                switch(tc)
                {
                    case TypeCode.Boolean:
                        if (((bool)o))
                            bw.Write(1.0);
                        else
                            bw.Write(0.0);
                        break;
                    case TypeCode.Int16:
                        bw.Write((double)o);
                        break;
                    case TypeCode.Int32:
                        bw.Write((double)o);
                        break;
                    case TypeCode.Int64:
                        bw.Write((double)o);
                        break;
                    case TypeCode.UInt16:
                        bw.Write((double)o);
                        break;
                    case TypeCode.UInt32:
                        bw.Write((double)o);
                        break;
                    case TypeCode.UInt64:
                        bw.Write((double)o);
                        break;
                    case TypeCode.Double:
                        bw.Write((double)o);
                        break;
                    case TypeCode.Byte:
                        bw.Write(double.NaN);
                        break;
                    case TypeCode.Char:
                        bw.Write(double.NaN);
                   
                        break;
                    case TypeCode.Decimal:
                        bw.Write(double.NaN);
                        break;
                    case TypeCode.SByte:
                        bw.Write(double.NaN);
                        break;
                    case TypeCode.Single:
                        bw.Write(double.NaN);
                        break;
                }
            }
        }



        public override OutputReaderBase GetReader()
        {
            return new ORBBinaryReader(this.FilePath + extention,true);
        }


    }
}
