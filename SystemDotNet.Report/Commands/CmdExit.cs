using System;
//using SystemDotNet.Console.Commands;

namespace SystemDotNet.Console.Commands
{
	/// <summary>
	/// Summary description for CmdExit.
	/// </summary>
	public class CmdExit:CommandBase
	{
		Program m;
		public CmdExit(Program m)
		{
			this.m = m;
			CMD = "exit|quit";
			USAGE = "exit";
			DESC = "Exits the simulator";
		}

		public override void Execute(string[] args)
		{
             m.exit = true;  
		}
	}
}
