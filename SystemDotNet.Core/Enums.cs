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
    /// Decides a signals sensitive type
    /// </summary>
    public enum SensitiveType {
        /// <summary>
        /// Fire every time a signal is updated. Can cause significant increase in simulation time.
        /// But keep in mind that the value of a signal is an object. If the object is an instance of a class the simulator might not 
        /// think the signal value changes, even if the values inside the object changes. To avoid this you can use ValueTypes for the
        /// signal values.
        /// </summary>
        ValueUpdated, 
        /// <summary>
        /// Fire when the value of the signal changes.
        /// </summary>
        ValueChanged 
    };


    public enum SignalDirectionType
    {
        Input,
        Output,
        InputOutput
    }
    public enum ShiftDirection { Left, Right }
}
