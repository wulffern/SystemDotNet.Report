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
using System;
using System.Collections.Generic;
namespace SystemDotNet
{
	/// <summary>
	/// Marks a method as a process
	/// </summary>
	public sealed class ProcessAttribute:Attribute
	{
		public ProcessAttribute()
		{
		}

        public string SignalName { get { return signalnames[0]; } set { signalnames[0] = value; } }
        string[] signalnames = new string[1];
        public List<string> SignalNames { get { return new List<string>(signalnames); } }


        private SensitiveType myVar;



        public SensitiveType SensType
        {
            get { return myVar; }
            set { myVar = value; }
        }

        public ProcessAttribute(string Signal):this(SensitiveType.ValueChanged,Signal){}

		public ProcessAttribute(params string[]  Signals):this(SensitiveType.ValueChanged,Signals){}

        public ProcessAttribute(SensitiveType type, string Signal):this(type ,new string[]{Signal}){ }


        public ProcessAttribute(SensitiveType type, params string[] Signals)
        {

            SensType = type;
            this.signalnames = Signals;
        }
	}
}
