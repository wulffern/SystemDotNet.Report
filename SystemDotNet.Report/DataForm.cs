using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SystemDotNet.Reporter
{
    public partial class DataForm : Form
    {
   

        public DataForm( ReportNode rp)
        {
            InitializeComponent();
            this.Text = rp.Text;
                AddAll(rp);
            

        }

        void AddAll(ReportNode rn)
        {

            List<List<double>> ml = new List<List<double>>();
            DataGridViewColumn dc;
     
            List<double> dl;
            int max = 0;
            foreach (string s in rn.PlotKeys)
            {
                dc = new DataGridViewTextBoxColumn();
                dc.HeaderText = s;
                dc.ReadOnly = true;
                dl = rn.GetPlot(s);
                if (max < dl.Count)
                    max = dl.Count;
                ml.Add(dl);
                dc.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                this.dataGridView1.Columns.Add(dc);

            }

      
            object[] da ;
            for (int i = 0; i < max; i++)
            {
                da = new object[ml.Count];
                for (int z = 0; z < ml.Count; z++)
                {
                    da[z] = ml[z][i];
                }
                this.dataGridView1.Rows.Add(da);
            }
        }


    }
}