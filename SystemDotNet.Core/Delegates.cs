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
//using NextGenLab.Chart;
using System.Collections.Generic;

namespace SystemDotNet
{
    /// <summary>
    /// Used to pass strings/messages
    /// </summary>
    /// <param name="message"></param>
	public delegate void StringHandler(string message);
	

    public delegate void ProgressHandler(int progress);

    /// <summary>
    /// General delegate
    /// </summary>
	public delegate void EmptyHandler();



    //Definition of a value writer
    public delegate string ValueWriter(object obj);

    
}
