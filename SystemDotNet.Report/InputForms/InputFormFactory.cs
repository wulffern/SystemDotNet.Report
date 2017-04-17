using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace SystemDotNet.Reporter.InputForms
{
    public class InputFormFactory
    {
        public static Dictionary<string, InputFormBase> ifbs = new Dictionary<string,InputFormBase>();

        //static InputFormFactory()
        //{
        //   Type[] ts =  Assembly.GetAssembly(typeof(InputFormBase)).GetTypes();
        //   foreach (Type t in ts)
        //   {
        //       if (t.IsSubclassOf(typeof(InputFormBase)))
        //       {
        //           ifbs.Add((InputFormBase)Activator.CreateInstance(t));
        //       }
        //   }
        //}

        public static InputFormBase GetInstance(string Title,params string[] names)
        {
            if (ifbs.ContainsKey(Title))
                return ifbs[Title];
            else
            {
                InputFormBase ifb = new InputFormBase();
                List<string> mynames = new List<string>();
                mynames.AddRange(names);
                ifb.Init(Title, mynames);
                ifbs.Add(Title, ifb);
                return ifb;
            }

     
        }
    }
}
