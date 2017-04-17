#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using SystemDotNet.PostProcessing;

#endregion

namespace SystemDotNet.Console.Commands
{
    public class CmdPSD:CommandBase
    {
        public CmdPSD()
        {
            CMD = "psd";
            USAGE = "open filename";
            DESC = "Power spectral density";
        }

        public override void Execute(string[] args)
        {
            double fs = 1;

            if(args.Length > 0)
                double.TryParse(args[0],out fs);
            PlotPowerSpectralDensity psd = new PlotPowerSpectralDensity(fs);
            ReportContainer.GetInstance().report.AddSinglePlot(psd);
        }

    }
}
