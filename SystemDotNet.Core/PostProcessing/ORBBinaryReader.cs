using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SystemDotNet;
namespace SystemDotNet.PostProcessing
{
    public class ORBBinaryReader:OutputReaderBase       
    {
        List<TypeCode> columntypes = new List<TypeCode>();
        List<string> headers = new List<string>();


        public ORBBinaryReader(string filename, bool hasheader)
            : base(filename, hasheader)
        {
        }

        protected override Dictionary<string,List<double>> ReadFile(List<string> myheaders)
        {

            using (BinaryReader br = new BinaryReader(File.OpenRead(filename)))
            {
                

                int version = br.ReadInt32();

                switch (version)
                {
                    case 0:
                        ReadHeaderVersion0(br);
                        return ReadFileVersion0(br, myheaders);
                        break;

                }

                br.Close();
            }
            

            return new Dictionary<string,List<double>>();
        }

        public override List<string> ReadHeader()
        {
            using (BinaryReader br = new BinaryReader(File.OpenRead(filename)))
            {
                int version = br.ReadInt32();

                switch (version)
                {
                    case 0:
                        return ReadHeaderVersion0(br);
                        break;

                }

                br.Close();
            }

            return new List<string>();
        }

        List<string> ReadHeaderVersion0(BinaryReader br)
        {
            headers.Clear();
            columntypes.Clear();
            int columns = br.ReadInt32();
            int headerlength = br.ReadInt32();
            char[] namebuffer = br.ReadChars(headerlength);

            string header = new string(namebuffer);
            headers.AddRange(header.Split(';'));
            return headers;
        }

        public Dictionary<string, List<double>> ReadFileVersion0(BinaryReader br, List<string> myheaders)
        {

            Dictionary<string, List<double>> values = new Dictionary<string, List<double>>();

            //foreach (string s in headers)
            //{
            //    values.Add(s,new List<double>());
            //}

            double val;
            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                this.Progress = (short)(br.BaseStream.Position * 100 / br.BaseStream.Length);

                try
                {
                    for (int i = 0; i < headers.Count; i++)
                    {
                        val = double.NaN;
                        val = br.ReadDouble();

                        if (myheaders.Count == 0)
                            values[headers[i]].Add(val);
                        else if (myheaders.Contains(headers[i].Trim()))
                        {
                            if (!values.ContainsKey(headers[i]))
                                values.Add(headers[i], new List<double>());
                            values[headers[i]].Add(val);

                        }
                    }
                }
                catch(Exception ex) 
                {
                    short i = 0;
                }
            }
            return values;
        }
    }
}
