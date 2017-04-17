using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SystemDotNet.PostProcessing
{
    public class TsvReader : OutputReaderBase    
    {

     

        public TsvReader(string filename): base(filename, true)
        {
       
        }

        
        public override List<string> ReadHeader()
        {

            List<string> names = new List<string>();
           names.Add("I");
            names.Add("Q");
            //using (StreamReader sr = new StreamReader(filename))
           // {
           //     names = ReadHeader(sr);
                
           // }
          
            return names;
        }

        List<string> ReadHeader(StreamReader sr)
        {
            List<string> names = new List<string>();
            names.Add("I");
            names.Add("Q");
            return names;
        }

        protected override Dictionary<string, List<double>> ReadFile(List<string> headers)
        {
            List<int> valuesToRead = new List<int>();
            Dictionary<string, List<double>> values = new Dictionary<string, List<double>>();

            List<double> Ivalues = new List<double>();
            List<double> Qvalues = new List<double>();

            values.Add("I", Ivalues);
            values.Add("Q", Qvalues);
            long progress = 0;
            using (StreamReader sr = new StreamReader(filename))
            {
                while (sr.Peek() >= 0)
                {
                    progress = sr.BaseStream.Position * 100 / sr.BaseStream.Length;

                    if ((this.Progress + 5) < progress)
                        this.Progress = (short)progress;

                    String s = sr.ReadLine();
                    String bstring = byteToBinaryString(s);
                    

                    
                   string I = bstring.Substring(8 * 1 + 4, 4) + bstring.Substring(8 * 0 + 0, 8);
                   string Q = bstring.Substring(8 * 2 + 0, 8) + bstring.Substring(8 * 1 + 0, 4);
                   int ii = Convert.ToInt32(I,2);  
                   if(ii >= 2048){ ii  = ii - 4096;} 
               
                   int iq =Convert.ToInt32(Q,2) ;
                   if(iq >= 2048){ iq  = iq - 4096;}

                   Ivalues.Add(Convert.ToDouble(ii));
                   Qvalues.Add(Convert.ToDouble(iq));

                }

            }
            return values;
        }


        public string byteToBinaryString(string line)
        {
            string[] sline = line.Split('\t');

            String bline = "";

            foreach (String sc in sline)
            {
                int i = Convert.ToUInt16(sc);
                string b = Convert.ToString(i, 2);
                if (b.Length < 8)
                {
                    while (b.Length < 8)
                    {
                        b = "0" + b;
                    }

                }
                bline += b;


            }

       

            return bline;

        }    
        
    }
}
