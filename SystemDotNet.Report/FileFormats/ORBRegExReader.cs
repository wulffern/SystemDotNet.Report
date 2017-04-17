using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SystemDotNet.PostProcessing
{
    public class ORBRegExReader:OutputReaderBase    
    {
        RegExReaderData RegExReaderData;

        public ORBRegExReader(string filename, bool hasheader, RegExReaderData RegExReaderData)
            : base(filename, hasheader)
        {
            this.RegExReaderData = RegExReaderData;
        }

        
        public override List<string> ReadHeader()
        {

            List<string> names;
            using (StreamReader sr = new StreamReader(filename))
            {
                names = ReadHeader(sr);
                
            }
            return names;
        }

        List<string> ReadHeader(StreamReader sr)
        {
            List<string> names;
            List<string> strs = RegExReaderData.GetNextStrings(sr);
            if (HasHeader)
            {
                names = strs;
            }
            else
            {
                names = new List<string>();
                for (int i = 0; i < strs.Count; i++)
                {
                    names.Add(i.ToString());
                }

            }

            return names;
        }

        protected override Dictionary<string, List<double>> ReadFile(List<string> headers)
        {
            List<int> valuesToRead = new List<int>();
            Dictionary<string, List<double>> values = new Dictionary<string, List<double>>();

            using (StreamReader sr = new StreamReader(filename))
            {
                List<string> myheaders = ReadHeader(sr);

                for (int i = 0; i < myheaders.Count; i++)
                {
                    if (headers == null || headers.Count == 0)
                    {
                        valuesToRead.Add(i);
                    }
                    else if(headers.Contains(myheaders[i]))
                    {
                            valuesToRead.Add(i);
                        
                    }

                    if (!values.ContainsKey(myheaders[i]))
                        values.Add(myheaders[i], new List<double>());
                }

                List<double> vals;
                double value;
                string key;
                long progress = 0;
                while ((vals = RegExReaderData.GetNextDouble(sr)) != null)
                {

                    progress = sr.BaseStream.Position * 100 / sr.BaseStream.Length;

                    if ((this.Progress + 5) < progress)
                        this.Progress = (short)progress;

                    for (int i = 0; i < valuesToRead.Count; i++)
                    {
                        value = vals[valuesToRead[i]];
                        key = myheaders[valuesToRead[i]];
                        values[key].Add(value);
                    }
                }
            }
            return values;
        }
    }
}
