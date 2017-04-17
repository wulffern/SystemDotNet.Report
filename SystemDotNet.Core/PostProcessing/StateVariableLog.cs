using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SystemDotNet.PostProcessing
{
    public class StateVariableLog
    {
        ModuleBase module;

        public StateVariableLog(ModuleBase module)
        {
            this.module = module;
        }

        public void Print(Stream s)
        {
            using (StreamWriter sw = new StreamWriter(s))
            {
                Print(module, sw);
            }

        }

        void Print(ModuleBase module, StreamWriter sw)
        {
            sw.Write(ListProperties(module));

            foreach (ModuleBase child in module.Children)
            {
                Print(child, sw);
            }
            
        }

        public string Print()
        {

            string buffer = Print(module);
            return buffer;
        }

        public string Print(ModuleBase mb)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ListProperties(mb));

            foreach(ModuleBase child in module.Children){
                Print(child);
            }

            return sb.ToString();

        }

        public static string ListProperties(ModuleBase module)
        {
            System.Reflection.PropertyInfo[] pis = module.GetType().GetProperties();

            StringBuilder sb = new StringBuilder();

            sb.Append(module.Name);
            sb.Append("\n");
            string indent = "   ";
            foreach (System.Reflection.PropertyInfo pi in pis)
            {
                object[] os = pi.GetCustomAttributes(typeof(ModulePropertyAttribute), true);
                if (os.Length > 0)
                {
                    ModulePropertyAttribute mpa = (ModulePropertyAttribute)os[0];
                    sb.Append(indent + pi.Name +": " + pi.GetValue(module, null).ToString());
                    sb.Append('\n');
                }
            }

            return sb.ToString();

        }

    }
}
