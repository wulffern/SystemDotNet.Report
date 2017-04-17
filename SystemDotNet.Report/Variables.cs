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
using System;

namespace SystemDotNet.Reporter
{
	/// <summary>
	/// Summary description for Variables.
	/// </summary>
	public class Variables
	{
		static Variables variables;
		private Variables()	{}
		public static Variables GetInstance()
		{
			if(variables == null)
				variables = new Variables();
			return variables;
		}

		string startuppath = "";
		

		public string StartupPath{get{return startuppath;}set{startuppath = value;}}
		public string CmdPrefix{get{return "sdn# ";}}

	}
}
