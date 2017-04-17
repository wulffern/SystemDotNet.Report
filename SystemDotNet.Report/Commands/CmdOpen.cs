#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using SystemDotNet.PostProcessing;

#endregion

namespace SystemDotNet.Console.Commands
{
    public class CmdOpen:CommandBase
    {
        public CmdOpen()
        {
            CMD = "open";
            USAGE = "open filename";
            DESC = "Opens a reportfile";
        }

        public override void Execute(string[] args)
        {
            if (args.Length > 0)
            {
                Report rep = new Report(null, args[0]);
                ReportContainer.NewReport(rep);
            }
            else
            {
                throw new CommandException("You must supply a filename");
            }
        }

    }
}
