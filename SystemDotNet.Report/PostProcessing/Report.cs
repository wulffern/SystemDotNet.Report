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
#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NextGenLab.Chart;
using System.Threading;
using System.Windows.Forms;
using SystemDotNet;
using NextGenLab.Chart.FileTypes;
#endregion

namespace SystemDotNet.PostProcessing
{

/// <summary>
	/// Used to pass ChartData
	/// </summary>
	/// <param name="cds"></param>
	public delegate void ChartDataHandler(List<ChartData> cds);

	public class Report : ModuleBase
	{
        
		public event ChartDataHandler NewChartData;
		public event StringHandler NewPrintReport;
		OutputReaderBase csvreader;

		List<IPlotSingle> singleplots = new List<IPlotSingle>();
		List<IPrintReport> prints = new List<IPrintReport>();
		List<IPlotMultiple> multplots = new List<IPlotMultiple>();
List<SignalBase> signals = new List<SignalBase>();

public List<IPlotMultiple> PlotsMultipleValues { get { return multplots; } }
public List<IPlotSingle> PlotSingleValues { get { return singleplots; } }
        public OutputReaderBase OutputReader { get { return csvreader; } }
public List<SignalBase> Signals {get{return signals;}}


Logger logger;

bool visible = true;
public bool Visible
{
get { return visible; }
set
{
visible = value;
singleplots.ForEach(delegate(IPlotSingle ips) { ips.Visible = value; });
                multplots.ForEach(delegate(IPlotMultiple ipm) { ipm.Visible = value; });
            }
        }

        public Report(ModuleBase parent, string filename):base(parent)
        {
            if (Path.GetExtension(filename) == ".bsdr")
            {

                csvreader = new ORBBinaryReader(filename, true);
            }
            else
            {
                csvreader = new ORBCsvReader(filename, true);
            }
            Simulator.GetInstance().OnStopped += new EmptyHandler(DoReport);
        }

        public Report(ModuleBase parent, OutputReaderBase csv)
            : base(parent)
        {
            csvreader = csv;
            Simulator.GetInstance().OnStopped += new EmptyHandler(DoReport);
        }

        public Report ( ModuleBase parent, Logger logger ):base(parent)
        {
            this.logger = logger;

            Simulator.GetInstance().OnStopped += new EmptyHandler( DoReport );
        }


        public override void First ()
        {
           if(NewChartData == null)
               NewChartData +=new ChartDataHandler(ChartDataPrinter);

            if(NewPrintReport == null)
                NewPrintReport +=new StringHandler(OutputPrinter);
        }


        public void AddSinglePlot(IPlotSingle ips)
        {
            singleplots.Add(ips);
        }

        public void AddPrint(IPrintReport ipr)
        {
            prints.Add(ipr);
        }

        public void AddMultiplePlots(IPlotMultiple ipm)
        {
            multplots.Add(ipm);
        }

		public void AddSignal(SignalBase sb){
		  signals.Add(sb);
}

        public void DoReport()
        {
            if (logger != null)
                csvreader = logger.OutputWriter.GetReader();

            Dictionary<string, List<double>> plots = csvreader.Read(signals);

            chartindex = 0;

            if (NewChartData != null)
            {
                foreach (IPlotMultiple ipm in multplots)
                {
                    NewChartData(ipm.Plot(plots));
                }
            }

            foreach (string key in plots.Keys)
            {
                if (NewChartData != null)
                {
                    foreach (IPlotSingle ips in singleplots)
                    {
                        ips.Title = key;
                        NewChartData(ips.Plot(plots[key]));
                    }
                }

                if (NewPrintReport != null)
                {
                    foreach (IPrintReport ipr in prints)
                    {
                        NewPrintReport(ipr.Print(key, plots[key]));
                    }
                }
            }
        }

        int chartindex = 0;

        public void ChartDataPrinter (List<ChartData> cds)
        {
            //FtXnmc ftx = new FtXnmc();
            //FtBnmc ftb = new FtBnmc();
            FtXnc xnc = new FtXnc();

            if(!Directory.Exists(Simulator.Settings.OutputDir))
                Directory.CreateDirectory(Simulator.Settings.OutputDir);

            string filename = "chart_" + chartindex;
            if(cds.Count > 0)
                filename = cds[0].Title.Trim();

            using (Stream s = File.OpenWrite( Simulator.Settings.OutputDir + "/" + Simulator.Settings.FileNamePrefix +"_" +  filename + ".xnc" ))
            {
                NextGenLab.Chart.ChartDataList cds1 = new ChartDataList();
                foreach (ChartData cd in cds)
                    cds1.Add( cd );
                xnc.Save( s, cds1 );
                s.Flush();
                s.Close();
            }
            chartindex++;
        }

        public void OutputPrinter(string message)
        {
            using (StreamWriter sw = new StreamWriter(Simulator.Settings.OutputDir + "/" + Simulator.Settings.FileNamePrefix + "_report.txt",true))
            {
                sw.WriteLine(message);
                sw.Close();
            }
        }
    }

   
}
