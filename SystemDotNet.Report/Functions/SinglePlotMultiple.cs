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
using System.Collections;
using System.Text;
using System.Windows.Forms;
using NextGenLab.Chart;

#endregion

namespace SystemDotNet.Reporter
{
    public class SinglePlotMultiple : SinglePlotBase
    {
        Dictionary<string, List<double>> plots;
        List<string> selectedkeys;

        public SinglePlotMultiple(ReportNode rp):base(rp)
        {
            AddMenuItem("General","Multiple dataset", delegate 
            {
                MultiSelect ms = new MultiSelect(rp.PlotKeys);
                if (DialogResult.OK == ms.ShowDialog())
                {
                    selectedkeys = ms.SelectedKeys;
                    Plot(); 
                }
            });

        }

        public override List<ChartData> OnPlot(ReportNode rp)
        {
            List<ChartData> cds = new List<ChartData>();
            PlotType = "Plot (";
            foreach (string s in selectedkeys)
            {
                PlotType += s + ",";
            }
            PlotType = PlotType.Remove(PlotType.Length - 1);
            PlotType += ")";


            if (selectedkeys == null || selectedkeys.Count < 1)
                return null;
            ChartData cd = ChartData.GetInstance();
            cd.Title = "";
            cd.TitlesY = selectedkeys.ToArray();
            cd.TitleX = "Sample";

            double[][] yval = new double[selectedkeys.Count][];
            int maxlength = int.MinValue;
            for (int i = 0; i < selectedkeys.Count; i++)
            {
                yval[i] = rp.GetPlot(selectedkeys[i]).ToArray();// plots[selectedkeys[i]].ToArray();
                if (maxlength < yval[i].Length)
                    maxlength = yval[i].Length;
            }
            cd.Y = yval;

            List<double> time = rp.GetPlot("Time");

            if (time != null && time.Count == maxlength)
            {
                cd.TitleX = "Time";
                cd.AxisLabelX = "[s]";
                cd.X = time.ToArray();
            }
            else
            {
                cd.TitleX = "Sample";
                double[] x = new double[maxlength];
                for (int i = 0; i < x.Length; i++)
                {
                    x[i] = i;
                }
                cd.AxisLabelX = "[n]";
                cd.X = x;
            }
            cds.Add(cd);
            
            return cds;
        }

    }
}
