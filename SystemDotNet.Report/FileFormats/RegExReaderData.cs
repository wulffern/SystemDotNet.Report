using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;

namespace SystemDotNet.PostProcessing
{
    [Serializable]
    public class RegExReaderData
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string desc;

        public string Description
        {
            get { return desc; }
            set { desc = value; }
        }       

        List<string> ignore = new List<string>();
        Regex rx;

        NumberFormatInfo nf = new NumberFormatInfo();

        public RegExReaderData(string regex, string[] ignore)
        {
            rx = new Regex(regex);
            this.ignore.AddRange(ignore);
        }

        public RegExReaderData(char delimiter, string[] ignore)
        {
            rx = new Regex(@"\s*([^" + delimiter + @"]+)\s*[" + delimiter + "\n]");
            this.ignore.AddRange(ignore);
        }

        public RegExReaderData(string delimiter)
        {
            rx = new Regex(@"\s*([^"+ delimiter + @"]+)\s*[" + delimiter + "\n]");
            ignore.Add("#");
        }

        public List<double> GetNextDouble(StreamReader sr)
        {
            List<string> strs = GetNextStrings(sr);
            if (strs == null)
                return null;

            List<double> db = new List<double>();

            double res;

            foreach (string s in strs)
            {
                if(!double.TryParse(s,NumberStyles.Any,nf, out  res))
                    res = double.NaN;
                db.Add(res);
            }
            return db;
        }

        public static RegExReaderData GuessFormat(string file) 
        {
            RegExReaderData rx = null;
            if (File.Exists(file))
            {
                using (StreamReader sr = new StreamReader(file))
                {
                    rx = GuessFormat(sr);
                }

            }
            return rx;
        }

        public static RegExReaderData GuessFormat(StreamReader sr)
        {
            string buffer;
            int i = 0;
            List<string> strs;
            string number = @"\+\-\deE\.";
            RegExReaderData rx = new RegExReaderData(@"\s*[" + number + @"]+([^" + number + @"]+)[" + number + @"]", new string[] { });
            

            List<string> guesses = new List<string>();
            while((strs = rx.GetNextStrings(sr) ) != null)
            {
                if (strs.Count > 0)
                    guesses.Add(strs[0]);

                i++;
                if (i > 50)
                    break;
            }

            Dictionary<string, int> frequency = new Dictionary<string, int>();
            foreach (string s in guesses)
            {
                if (!frequency.ContainsKey(s))
                    frequency.Add(s, 0);

                frequency[s]++;
            }

            string mostcommon = ";";
            int max = 0;
            foreach (string key in frequency.Keys)
            {
                if (frequency[key] > max)
                {
                    max = frequency[key];
                    mostcommon = key;
                }
            }

            RegExReaderData reg = new RegExReaderData(mostcommon);

            return reg;
        }

        public List<string> GetNextStrings(StreamReader sr)
        {

            if (sr == null)
                throw new ArgumentException("Stream cannot be null", "sr");

            List<string> strs = new List<string>();

            string buffer;
            bool matchIgnore;
            MatchCollection mc;
            bool readline = false;
            while ((buffer = sr.ReadLine()) != null)
            {
                
                buffer = buffer + "\n";

                if (buffer.Trim() == "")
                    continue;

               
                matchIgnore = false;
                foreach (string s in ignore)
                {
                    if (buffer.StartsWith(s))
                        matchIgnore = true;
                }
                if (buffer.Length > 0 && matchIgnore)
                    continue;

                readline = true;

               mc = rx.Matches(buffer);
               foreach (Match m in mc)
               {
                   if (m.Groups.Count > 1)
                   {
                       strs.Add(m.Groups[1].Value);
                   }
               }
               break;   
            }

            if (readline)
                return strs;
            else
                return null;
        }

        
    }
}
