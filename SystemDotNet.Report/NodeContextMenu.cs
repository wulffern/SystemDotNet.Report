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
using System.Windows.Forms;

#endregion

namespace SystemDotNet.Reporter
{
    public class NodeContextMenu
    {
        SortedDictionary<string, SortedDictionary<string, EventHandler>> handlers = new SortedDictionary<string, SortedDictionary<string, EventHandler>>();
        public NodeContextMenu(ReportNode reportnode)
        {
            List<SinglePlotBase> plots = new List<SinglePlotBase>();

            Type[] types = this.GetType().Assembly.GetTypes();
            foreach (Type t in types)
            {
                if(t.IsSubclassOf(typeof(SinglePlotBase)))
                {
                    object o = null;
                    try
                    {
                        o = Activator.CreateInstance(t, new object[] { reportnode });
                    }
                    catch { }

                     if (o != null)
                         plots.Add((SinglePlotBase)o);
                 }
            }

             foreach (SinglePlotBase sb in plots)
             {
                 foreach (string key in sb.EventHandlers.Keys)
                 {
                     if (!handlers.ContainsKey(key))
                         handlers.Add(key, new SortedDictionary<string, EventHandler>());
                         foreach (string innerkey in sb.EventHandlers[key].Keys)
                         {

                                 if (!handlers[key].ContainsKey(innerkey))
                                     handlers[key].Add(innerkey, sb.EventHandlers[key][innerkey]);
                             
                         }
                     
                     
                 }
             }
         }


        public MenuItem[] GetMenuItems()
        {
            List<MenuItem> MenuItems = new List<MenuItem>();
            MenuItem outer;
            
            MenuItem inner;

            //int i = 0;

            //string alphabet = "1234567890ABCDEFGHJIKLMNOPQRSTUVWXYZ";

            foreach (string key in handlers.Keys)
            {
                outer = new MenuItem(key);
                outer.Name = key;
                foreach (string innerkey in handlers[key].Keys)
                {
                    inner = new MenuItem(innerkey, handlers[key][innerkey]);
                    inner.Name = innerkey;
                    //if (i < alphabet.Length)
                    //{
                    //    inner.Shortcut = (Shortcut)Enum.Parse(typeof(Shortcut),"CtrlShift" + alphabet[i]);
                    //}

                    outer.MenuItems.Add(inner);

                //    i++;

                }
                MenuItems.Add(outer);
            }
            return MenuItems.ToArray();
        }
    }
}
