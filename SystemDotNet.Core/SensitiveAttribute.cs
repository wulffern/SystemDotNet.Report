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

namespace SystemDotNet
{

    /// <summary>
	/// Marks a method as sensitive to certain signals
	/// </summary>
	[Obsolete("Use the constructors of Process to specify sensitivity",true)]
	public class SensitiveAttribute:Attribute
	{

        public string SignalName { get { return signalnames[0]; } set { signalnames[0] = value; } }
        string[] signalnames = new string[1];
        public string[] SignalNames { get { return signalnames; } }

        private SensitiveType myVar;

        public SensitiveType SensType
        {
            get { return myVar; }
            set { myVar = value; }
        }


        public SensitiveAttribute(string Signal):this(SensitiveType.ValueUpdated,Signal){}

		public SensitiveAttribute(params string[]  Signals):this(SensitiveType.ValueUpdated,Signals){}

        public SensitiveAttribute(SensitiveType type, string Signal):this(type,new string[]{Signal}){ }

        public SensitiveAttribute(SensitiveType type, params string[] Signals)
    { 
            SensType = type;
            this.signalnames = Signals;
        }

    }
}
