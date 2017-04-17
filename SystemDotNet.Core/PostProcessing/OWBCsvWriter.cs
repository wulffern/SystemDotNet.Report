using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SystemDotNet;

namespace SystemDotNet.PostProcessing
{
    public class OWBCsvWriter:OutputWriterBase  
    {

        StreamWriter sw;
        string extention = ".csv";

        private char _Delimiter = ';';

        public char Delimiter
        {
            get { return _Delimiter; }
            set { _Delimiter = value; }
        }


        public OWBCsvWriter(string FileName) : base(FileName, true, true, false) { }
        

        public OWBCsvWriter(string FileName, bool ShowHeader, bool UseAdvancedFileName, bool ShowTime)
            : base(FileName, ShowHeader, UseAdvancedFileName, ShowTime)
        {
         }

        ~OWBCsvWriter()
        {
            if (sw != null)
                sw.Close();

        }

        protected override void OnClose()
        {
            sw.Close();
        }

        protected override void OnOpen()
        {
            sw = new StreamWriter(this.FilePath + extention);
            
        }

        protected override void WriteHeader(List<string> header)
        {
            WriteRow(header.ToArray());
        }

        public override void WriteValues()
        {

            string[] vals;

            int x = 0;

            if (ShowTime)
            {
                vals = new string[SignalList.Count + 1];
                vals[0] = Simulator.GetInstance().RealTime.ToString("E");
                x++;
            }
            else
            {
                vals = new string[SignalList.Count];
            }

            string val;
            for (int i = x; i < vals.Length; i++)
            {
                //If the value writer for the current signal exist, call it and append the result to the string builder.
                val = SignalList[i - x].ToString();
                if (val.ToLower() == "true")
                    val = "1";
                if (val.ToLower() == "false")
                    val = "0";
                   vals[i] = val;
            }

            WriteRow(vals);        
        }

        public void WriteRow(string[] columns)
        {
            if (columns.Length == 0)
                return;

            for (int i = 0; i < columns.Length; i++)
            {
                sw.Write(columns[i]);
                if (i < columns.Length - 1)
                    sw.Write(Delimiter);
            }

            sw.Write('\n');
        }

        public override OutputReaderBase GetReader()
        {
            return new ORBCsvReader(this.FilePath + extention, ShowHeader);
        }


    }
}
