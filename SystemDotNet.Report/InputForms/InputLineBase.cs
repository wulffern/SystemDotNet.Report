using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace SystemDotNet.Reporter.InputForms
{
    public partial class InputLineBase : UserControl
    {
        double val = 0;
        string title = "Untitled";
        public double Value { get { return val; } }
        public string Title { get { return title; } }
        string number = @"\+\-\deE\.";

        public InputLineBase(string Title)
        {

            InitializeComponent();
                Regex rx = new Regex("{([" + number + "]+)}");
                if (rx.IsMatch(Title))
                {
                    Match m = rx.Match(Title);
                    if (m.Groups.Count > 0)
                    {
                        this.tValue.Text = m.Groups[1].Value;
                    }
                    Title = rx.Replace(Title, "");
                }
            

            
            this.lText.Text = Title + ":";
            title = Title;
            this.comboBox1.SelectedIndex = 0;
        }



        public void Parse()
        {
            double.TryParse(tValue.Text, out val);
            Scale(this.comboBox1, ref val);
        }

        private void CheckValue(object sender, EventArgs e)
        {
            if (!double.TryParse(tValue.Text, out val)){
                MessageBox.Show("Value must be parsable to double. E.g. 2e6", "Error in input", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            this.tValue.Focus();
        }

        void Scale(ComboBox cb, ref double result)
        {
            switch ((string)cb.SelectedItem)
            {
                case "k":
                    result *= 1e3;
                    break;
                case "Meg":
                    result *= 1e6;
                    break;
                case "G":
                    result *= 1e9;
                    break;
                case "T":
                    result *= 1e12;
                    break;
                default:
                    break;
            }
        }
    }
}
