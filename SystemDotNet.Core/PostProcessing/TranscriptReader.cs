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
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;

namespace SystemDotNet.PostProcessing
{
    public class TranscriptReader
    {
        char[] delimiters = new char[] { ':' };
        string filename;
        public TranscriptReader(string filename)
        {
            this.filename = filename;
        }

        Dictionary<string, List<double>> container = new Dictionary<string, List<double>>();

        public void Print(string filename)
        {
            string s = Parse();
            using (StreamWriter sw = new StreamWriter(filename))
            {
                sw.WriteLine(s);
                sw.Close();
            }
        }

        public string Parse()
        {

            using (StreamReader sw = new StreamReader(filename))
            {
                string buffer;
                string[] vals;
                int index = 1;
                double val;
                NumberFormatInfo nf = new NumberFormatInfo();
                while ((buffer = sw.ReadLine()) != null)
                {
                    if (buffer.IndexOfAny(delimiters,0) > -1)
                    {
                        vals = buffer.Split(delimiters);
                        if (vals.Length < 2)
                            Simulator.GetInstance().SendError("Transcript: Something is wrong on line " + index);
                        else
                        {
                            
                            
                            if (double.TryParse(vals[vals.Length - 1],NumberStyles.Any,nf, out  val))
                            {
                                if (!container.ContainsKey(vals[0]))
                                    container.Add(vals[0], new List<double>());
                                container[vals[0]].Add(val);
                            }   
                        }
                    }
                }
            }

            int maxlength = 0;
            string[] keys = new string[container.Keys.Count];
            container.Keys.CopyTo(keys, 0);

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < keys.Length; i++)
            {
                if (container[keys[i]].Count > maxlength)
                    maxlength = container[keys[i]].Count;

                sb.Append(keys[i]);
                if (i < keys.Length - 1)
                    sb.Append(";");
            }

            sb.Append("\n");

            for (int i = 0; i < maxlength; i++)
            {
                for (int k = 0; k < keys.Length; k++)
                {
                    if(container[keys[k]].Count > i)
                        sb.Append(container[keys[k]][i]);

                    if (k < keys.Length - 1)
                        sb.Append(";");
                }
                sb.Append("\n");

            }

            

            return sb.ToString();
        }
    }
}
