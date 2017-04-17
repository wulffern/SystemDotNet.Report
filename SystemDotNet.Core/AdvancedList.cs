/*
Copyright (C) 2004 Carsten Wulff

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
using System.Reflection;

#endregion

namespace SystemDotNet
{
    public class AdvancedList<T>:List<T>
    {
        

        public AdvancedList() { }
        public AdvancedList(int capacity):base(capacity) { }
        public AdvancedList(IEnumerable<T> collection):base(collection) { }

        public void SetAll(T value)
        {
            for (int i = 0; i < Count; i++)
            {
                this[i] = value;
            }
        }

        public void AddAll(T value, int length)
        {
            for (int i = 0; i < length; i++)
                Add(value);

        }

        public void SetRange(T value, int start, int stop)
        {
            for (int i = start; i < stop; i++)
            {
                this[i] = value;
            }
        }

        public T Shift(ShiftDirection sd, T inp)
        {
            if (Count == 0)
                throw new IndexOutOfRangeException("Cannot shift list that contains zero elements");

            int length = this.Count;
            T val = default(T);
            switch (sd)
            {
                case ShiftDirection.Left:
                    val = this[length - 1];
                    for (int i = 1; i < length; i++)
                    {
                        this[i] = this[i - 1];
                    }
                    this[0] = inp;
                    break;
                case ShiftDirection.Right:
                    val = this[0];
                    for (int i = 0; i < length - 1; i++)
                    {
                        this[i] = this[i + 1];
                    }
                    this[length - 1] = inp;
                    break;
            }
            return val;
        }

        public static List<T> GetReflectionList(Assembly a)
        {
            List<T> list = new List<T>();
            Type[] types = a.GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                if (types[i].IsSubclassOf(typeof(T)))
                {
                    list.Add((T)Activator.CreateInstance(types[i]));
                }
            }
            return list;
        }
    }
}
