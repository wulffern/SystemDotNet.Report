using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SystemDotNet.PostProcessing;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace SystemDotNet.Reporter
{
    public partial class FileFormatForm : Form
    {
        string dir = "FileFormats";

        public FileFormatForm()
        {
            InitializeComponent();

            this.lExample.Text = @"You can create a new file format by entering a regular expression to select columns from the file. The regular expression that is used for parsing CSV files is: \s*([^;\n]+). Spaces are replaced with _";

            this.rtExample.Text = @"#This line is ignored 
this;is;the;header
1.0;1e6;2.4;0.1012
1.0;1e6;2.4;0.1012";

            LoadFileFormats();
        }

        void LoadFileFormats()
        {
            if (Directory.Exists(dir))
            {
                string[] files = Directory.GetFiles(dir, "*.ftf");
                foreach (string f in files)
                {
                    this.comboBox1.Items.Add(Path.GetFileNameWithoutExtension(f));
                }
            }
        }

        private void bTestExpression_Click(object sender, EventArgs e)
        {
            this.dataGridView1.Rows.Clear();
            this.dataGridView1.Columns.Clear();
            try
            {
                string[] ignore = tIgnore.Text.Split(' ');
                RegExReaderData rx = new RegExReaderData(this.tRegex.Text, ignore);
                using (MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(this.rtExample.Text)))
                {
                    using (StreamReader sr = new StreamReader(ms))
                    {
                        List<string> names = rx.GetNextStrings(sr);
                        foreach (string s in names)
                        {
                            this.dataGridView1.Columns.Add(s.Replace(' ', '_'), s.Replace(' ', '_'));
                        }

                        List<double> vals;
                        while ((vals = rx.GetNextDouble(sr)) != null)
                        {
                            object[] ob = new object[vals.Count];
                            for (int i = 0; i < ob.Length; i++)
                            {
                                ob[i] = (object)vals[i];
                            }
                            this.dataGridView1.Rows.Add(ob);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error in regular expression", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void bSave_Click(object sender, EventArgs e)
        {
            
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(tName.Text);
            sb.AppendLine(Convert.ToBase64String(Encoding.Unicode.GetBytes(this.tRegex.Text)));
            sb.AppendLine(tIgnore.Text);

            using (StreamWriter sw = new StreamWriter(dir + Path.DirectorySeparatorChar+ this.tName.Text + ".ftf"))
            {
                sw.Write(sb.ToString());
            }
            
        }

        private void bLoad_Click(object sender, EventArgs e)
        {
            string file = dir + Path.DirectorySeparatorChar + this.comboBox1.SelectedItem + ".ftf";
            if (File.Exists(file))
            {
                try
                {

                    using (StreamReader sr = new StreamReader(file))
                    {

                        this.tName.Text = sr.ReadLine();
                        this.tRegex.Text = Encoding.Unicode.GetString(Convert.FromBase64String(sr.ReadLine()));
                        this.tIgnore.Text = sr.ReadLine();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Something is wrong with the file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}