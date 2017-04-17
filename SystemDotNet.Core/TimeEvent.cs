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
	/// Writes an object to the value of the signal when run is called
	/// </summary>
	public class TimeEvent<T>:IRunnable
	{
        IValue<T> c;
        T o;

        public TimeEvent(IValue<T> c, T o)
        {
            this.c = c;
			this.o = o;
		}

        public bool Run()
		{
            if (c.SensType == SensitiveType.ValueChanged && c.Value != null && c.Value.Equals(o))
                return false;
            c.Value = o;
            return true;
        }
    }
}
