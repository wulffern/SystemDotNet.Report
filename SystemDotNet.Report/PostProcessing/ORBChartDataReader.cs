using System;
using System.Collections.Generic;
using System.Text;
using SystemDotNet.PostProcessing;
using NextGenLab.Chart;

namespace SystemDotNet.PostProcessing
{
    public class ORBChartDataReader:OutputReaderBase
    {
        //ChartDataList cds;

        Dictionary<string, List<double>> content = new Dictionary<string, List<double>>();

        public ORBChartDataReader(List<ChartData> cds):base(null,true)
        {

            ChartData cd;
            string ytitle;

            for (int i = 0; i < cds.Count; i++)
            {
                cd = cds[i];

                if (!content.ContainsKey(cd.TitleX))
                
                    content.Add(cd.TitleX,new List<double>(cd.X));
                

                for(int z=0;z< cd.Y.Length;z++)
                {
                    if (cd.TitlesY.Length > z)
                        ytitle = cd.TitlesY[z];
                    else
                        ytitle = i + "_" + z;

                    if(!content.ContainsKey(ytitle))
                        content.Add(ytitle,new List<double>(cd.Y[z]));
                    

                }

            }

            this.IsNoFile = true;

        }

        public override List<string> ReadHeader()
        {
            List<string> strs = new List<string>();

            strs.AddRange(content.Keys);

            //foreach (string s in content.Keys)
            //{
            //    strs.Add(s);
            //}
            return strs;
        }

        protected override Dictionary<string, List<double>> ReadFile(List<string> headers)
        {
            return content;
        }
    }
}
