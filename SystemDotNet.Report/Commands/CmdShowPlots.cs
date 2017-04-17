#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using SystemDotNet.PostProcessing;
using NextGenLab.Chart;
using System.Windows.Forms;

#endregion

namespace SystemDotNet.Console.Commands
{
    public class CmdShowPlots:CommandBase
    {
        Form f;
        public CmdShowPlots(Form f)
        {
            this.f = f;
            CMD = "showplots";
            USAGE = "showplots";
            DESC = "Show Defined plots";
        }

        public override void Execute(string[] args)
        {
            Report report = ReportContainer.GetInstance().report;
            report.NewChartData +=new Report.ChartDataHandler(report_NewChartData);
            report.DoReport();
        }

        void report_NewChartData(List<NextGenLab.Chart.ChartData> cds)
        {
            Chart ch = null;
            foreach (ChartData cd in cds)
            {
                ch = new Chart();
                ch.Open(cd, false);
                ch.MdiParent = f;
                //ch.WindowState = FormWindowState.Maximized;
                //f = new FormLauncher(ch);
                ch.Show();

            }
        }
    }
}
