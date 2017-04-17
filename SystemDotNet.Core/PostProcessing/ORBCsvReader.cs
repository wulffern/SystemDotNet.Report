/*
Copyright (C) 2005 Carsten Wulff

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
using System.IO;
using System.Globalization;

#endregion

namespace SystemDotNet.PostProcessing
{
    public class ORBCsvReader : OutputReaderBase
    {
        public char Delimiter
        {
            get { return delimiters[0]; }
            set { delimiters[0] = value; }
        }


        private List<char> delimiters = new List<char>();

        public List<char> Delimiters
        {
            get { return delimiters; }
        }


        public ORBCsvReader(string filename, bool hasheader)
            : base(filename, hasheader)
        {
            delimiters.Add(';');
        }

        public override List<string> ReadHeader()
        {
            List<string> columnNames = new List<string>();
            using (StreamReader sr = new StreamReader(filename))
            {
                columnNames = ReadHeader(sr);
            }
            return columnNames;
        }


        List<string> ReadHeader(StreamReader sr)
        {

            string buffer;
            List<string> columnNames = new List<string>();
            string[] keys = new string[0];
            char[] delimits = delimiters.ToArray();
            while ((buffer = sr.ReadLine()) != null)
            {
                //Skip line if commented out
                if (buffer.Trim().StartsWith("#"))
                    continue;
                //Skip empty lines
                if (buffer.Trim() == "")
                    continue;
                //Split values
                keys = buffer.Split(delimits);

                break;
            }



            if (HasHeader)
            {
                foreach (string key in keys)
                {
                    columnNames.Add(key.Trim());
                }
            }
            else
            {
                for (int i = 0; i < keys.Length; i++)
                {
                    columnNames.Add(i.ToString());
                }
            }
            return columnNames;
        }


        protected override Dictionary<string, List<double>> ReadFile(List<string> headers)
        {
            bool first = true;


            using (StreamReader sr = new StreamReader(filename))
            {
                string buffer;
                string[] vals;
                double val;
                string key;
                Dictionary<string, List<double>> values = new Dictionary<string, List<double>>();
                char[] delimits = delimiters.ToArray();

                List<int> valuesToRead = new List<int>();

                List<string> keys = ReadHeader(sr);

                for (int i = 0; i < keys.Count; i++)
                {
                    if (headers == null)
                    {
                        valuesToRead.Add(i);
                        if (!values.ContainsKey(keys[i]))
                            values.Add(keys[i], new List<double>());
                    }
                    else if (headers.Contains(keys[i]))
                    {
                        valuesToRead.Add(i);
                        if (!values.ContainsKey(keys[i]))
                            values.Add(keys[i], new List<double>());
                    }

                }

                string value;

                long progress = 0;

                NumberFormatInfo nf = new NumberFormatInfo();

                while ((buffer = sr.ReadLine()) != null)
                {
                    buffer = buffer.Trim();

                    //Skip line if commented out
                    if (buffer.StartsWith("#") || buffer == "" )
                        continue;

                    progress = sr.BaseStream.Position * 100 / sr.BaseStream.Length;

                    if ((this.Progress + 5) < progress)
                        this.Progress = (short)progress;

                    vals = buffer.Split(delimits);

                    for (int i = 0; i < valuesToRead.Count; i++)
                    {
                        value = vals[valuesToRead[i]].Trim();
                        key = keys[valuesToRead[i]];
                        if (double.TryParse(value,NumberStyles.Any,nf, out  val))
                            values[key].Add(val);
                        else
                            values[key].Add(Double.NaN);
                    }

                }
                sr.Close();
                return values;
            }
        }

    }
}
