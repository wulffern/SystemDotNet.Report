using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
namespace SystemDotNet.Reporter.Functions
{
    public class SPShowData:SinglePlotBase
    {
        public SPShowData(ReportNode rp)
            : base(rp)
        {

            AddMenuItem("General","Show Data", delegate
            {
                Plot();
            });

        }

        delegate void SetMdiContainer(Form f, Form df);

        public override List<NextGenLab.Chart.ChartData> OnPlot(ReportNode rp)
        {
            DataForm df = new DataForm(rp);
            Form f = rp.MDIContainer;
            if (f.IsMdiContainer)
            {
                f.Invoke(new SetMdiContainer(SetMDICont), f, df);
            }
            else
            {
                df.Show();
            }
            

            return null;
        }
        void SetMDICont(Form f, Form df)
        {
            df.MdiParent = f;
            df.Show();
        }
    }
}
