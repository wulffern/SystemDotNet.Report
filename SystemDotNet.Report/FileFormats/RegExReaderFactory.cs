using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;

namespace SystemDotNet.PostProcessing
{
    public class RegExReaderFactory
    {
        static List<RegExReaderData> datasets;
        static string dir = "FileFormats";

        public static List<string> GetNames()
        {
            if (datasets == null)
                LoadDataSets();

            List<string> names = new List<string>();

            

            
            foreach (RegExReaderData rerd in datasets)
            {
                names.Add(rerd.Name);
            }

            if (Directory.Exists(dir))
            {
                string[] files = Directory.GetFiles(dir, "*.ftf");

                foreach (string s in files)
                {
                    names.Add(Path.GetFileNameWithoutExtension(s));
                }
            }

            return names;
        }

        public static RegExReaderData GetReaderData(string readerdata)
        {
            RegExReaderData rerder = null;
            foreach (RegExReaderData rerd in datasets)
            {
                if (rerd.Name == readerdata)
                {
                    rerder = rerd;
                    break;
                }
            }
            if (Directory.Exists(dir))
            {
                string[] files = Directory.GetFiles(dir, "*.ftf");

                string name;
                string myname;
                string regex;
                string ignore;
                foreach (string s in files)
                {
                    name = Path.GetFileNameWithoutExtension(s);
                    if (name == readerdata)
                    {
                        using (StreamReader sr = new StreamReader(s))
                        {
                            myname = sr.ReadLine();
                            regex = Encoding.Unicode.GetString(Convert.FromBase64String(sr.ReadLine()));
                            ignore = sr.ReadLine();
                            rerder = new RegExReaderData(regex, ignore.Split(' '));
                            rerder.Name = name;
                        }

                    }
                }
            }

            return rerder;
        }

        static void LoadDataSets()
        {
            RegExReaderData rs = new RegExReaderData(';', new string[] { });
           
                Type[] types = rs.GetType().Assembly.GetTypes();
            datasets = new List<RegExReaderData>();
            foreach (Type t in types)
            {
                if (t.IsSubclassOf(typeof(RegExReaderData)))
                {
                    datasets.Add((RegExReaderData)Activator.CreateInstance(t));
                }
            }
            
       
            
        }

    }
}
