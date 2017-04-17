using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SystemDotNet.Reporter.InputForms
{
    public partial class InputFormBase : Form
    {
        Dictionary<string, double> values = new Dictionary<string, double>();
        List<InputLineBase> ilbs = new List<InputLineBase>();

        public Dictionary<string, double> Values { get { return values; } }

        public InputFormBase()
        {
            
            InitializeComponent();
            
        }



        public void Init(string Title, List<string> values)
        {
            this.Text = Title;
            InputLineBase ilb;
            foreach (string s in values)
            {
                
                ilb = new InputLineBase(s);
                ilbs.Add(ilb);

               
                this.pInputs.Controls.Add(ilb);
            }

            

            this.Height = ilbs.Count * 26 + 100;
        }

        protected override void OnActivated(EventArgs e)
        {
            if (this.pInputs.Controls.Count > 0)
                this.pInputs.Controls[0].Focus();
        }

        private void bOk_Click(object sender, EventArgs e)
        {
            foreach (InputLineBase ilb in ilbs)
            {
                ilb.Parse();
                if (this.values.ContainsKey(ilb.Title))
                    values[ilb.Title] = ilb.Value;
                else
                    values.Add(ilb.Title, ilb.Value);
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void bCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }




    }
}