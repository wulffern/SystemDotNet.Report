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
using SystemDotNet.PostProcessing;
using NextGenLab.Chart;
using System.Windows.Forms;

#endregion

namespace SystemDotNet.Reporter
{
    public class SingleRMSPlot : SinglePlotBase
    {
        string xaxis = null;
        string yaxis = null;

        enum RMSType { Simple, SNRfromRMS };

        RMSType rmstype = RMSType.Simple;

        public SingleRMSPlot(ReportNode rp)
            : base(rp)
        {
            AddMenuItem("Calculate","RMS", delegate
            {
                rmstype = RMSType.Simple;
                Plot();
            });
            AddMenuItem("Calculate", "SNR from RMS", delegate
            {
                XYSelectForm xy = new XYSelectForm("Signal:","Noise:",rp.PlotKeys);
                if (DialogResult.OK == xy.ShowDialog())
                {
                    xaxis = xy.XAxis;
                    yaxis = xy.YAxis;
                    rmstype = RMSType.SNRfromRMS;
                    Plot();
                }
            });
        }

        public override List<NextGenLab.Chart.ChartData> OnPlot(ReportNode rp)
        {
            string str = "";

            switch (rmstype)
            {
                case RMSType.Simple:
                    PrintRMS rms = new PrintRMS();
                    str = rms.Print(this.Name, rp.GetPlot(this.Name));
                    
                    break;
                case RMSType.SNRfromRMS:
                    List<double> chxaxis = rp.GetPlot(xaxis);
                    List<double> chyaxis = rp.GetPlot(yaxis);

                    PrintSnrFromRMS psrm = new PrintSnrFromRMS(0, "");
                    str = psrm.Print(xaxis, chxaxis, yaxis, chyaxis);
                    break;

            }

            MessageBox.Show(str);

            return new List<ChartData>();
        }

    }

}
