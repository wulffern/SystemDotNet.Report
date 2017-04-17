/*
Copyright (C) 2004 Carsten Wulff

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
using System;
using System.IO;
using System.Text;
using System.Collections;
using SystemDotNet;
using System.Diagnostics;
using System.Configuration;
using System.Collections.Generic;

namespace SystemDotNet
{
    /// <summary>
    /// Writer for Vcd files that implements ISignalWriter
    /// </summary>
    public class VcdWriter : ISignalWriter
    {
        StreamWriter filewriter;
        string filename;

        char[] asci = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'x', 'y', 'z' };

        int i1 = 0;
        int i2 = 0;
        int i3 = 0;

        string[] prev = new string[0];

        bool open = false;

        private bool useadvancedfilename = true;

        public bool UseAdvancedFilename
        {
            get { return useadvancedfilename; }
            set { useadvancedfilename = value; }
        }


        /// <summary>
        /// Sets filename
        /// </summary>
        /// <param name="filename"></param>
        public VcdWriter(string filename)
        {
            this.filename = filename;

            

        }

        /// <summary>
        /// Generate label from 3 asci letters
        /// </summary>
        /// <returns></returns>
        string GetLabel()
        {
            i1++;

            if (i1 > asci.Length - 1)
            {
                i1 = 0;
                i2++;
            }

            if (i2 > asci.Length - 1)
            {
                i2 = 0;
                i3++;
            }

            if (i3 > asci.Length - 1)
            {
                throw new Exception("Maximum number of signals reached");
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(asci[i3]);
            sb.Append(asci[i2]);
            sb.Append(asci[i1]);

            return sb.ToString();
        }



        #region IChannelWriter Members

        /// <summary>
        /// Opens file and writes header
        /// </summary>
        public void Open()
        {

            if (open) return;

            string path = filename;
            if (UseAdvancedFilename)
            {
                if (!Directory.Exists(Simulator.Settings.OutputDir))
                    Directory.CreateDirectory(Simulator.Settings.OutputDir);

                path = Simulator.Settings.OutputDir + "/" + Simulator.Settings.FileNamePrefix + "_" + filename;
            }

            filewriter = new StreamWriter(path);

            //Writer Date
            filewriter.WriteLine("$date");
            filewriter.WriteLine(DateTime.Now);
            filewriter.WriteLine("$end");
            filewriter.WriteLine();

            //Writer version
            filewriter.WriteLine("$version");
            filewriter.WriteLine("SystemDotNet v0.01\nALL RIGHTS RESERVED");
            filewriter.WriteLine("$end");
            filewriter.WriteLine();

            //Write timescale
            filewriter.WriteLine("$timescale");
            filewriter.WriteLine("1 ns");
            filewriter.WriteLine("$end");
            filewriter.WriteLine();

            open = true;

        }

        /// <summary>
        /// Closes file if it is open
        /// </summary>
        public void Close()
        {
            if (filewriter != null)
                filewriter.Close();

            open = false;
        }

        /// <summary>
        /// Writes definitions of the signals to be recorded
        /// </summary>
        /// <param name="cs">Signals</param>
        public void WriteNames(List<SystemDotNet.SignalBase> cs)
        {
            i1 = i2 = i3 = 0;
            filewriter.WriteLine("$scope module SystemDotNet $end");
            for (int i = 0; i < cs.Count; i++)
            {
                SignalBase c = cs[i];

                if (c is Signal<bool>)
                {
                    filewriter.WriteLine("$var wire 1 " + GetLabel() + " " + c.FullName + " $end");
                }
                else if (c is BitVectorUnsigned)
                {
                    BitVectorUnsigned bvu = (BitVectorUnsigned)c;
                    filewriter.WriteLine("$var reg " + bvu.Bits + " " + GetLabel() + " " + c.FullName + " [" + (bvu.Bits - 1) + ": 0 ] $end");
                }
                else if (c is SignalCollection<bool>)
                {
                    SignalCollection<bool> sc = c as SignalCollection<bool>;
                    for (int j = 0; j < sc.Count; j++)
                    {
                        filewriter.WriteLine("$var reg " + sc.Count + " " + GetLabel() + " " + c.FullName + "(" + j + ")" + " [" + (sc.Count - 1) + ": 0 ] $end");
                    }
                }
                else if (c is SignalCollection<double>)
                {
                    SignalCollection<double> sc = c as SignalCollection<double>;
                    for (int j = 0; j < sc.Count; j++)
                    {
                        filewriter.WriteLine("$var real 1 " + GetLabel() + " " + sc.FullName + "(" + j + ")" + " $end");
                    }
                }
                else if (c is SignalCollection<ulong>)
                {
                    SignalCollection<ulong> sc = c as SignalCollection<ulong>;
                    filewriter.WriteLine("$var reg " + sc.Count + " " + GetLabel() + " " + c.FullName + " [" + (sc.Count - 1) + ": 0 ] $end");
                }
                else
                {
                    filewriter.WriteLine("$var real 1 " + GetLabel() + " " + c.FullName + " $end");
                }
            }
            filewriter.WriteLine();
            filewriter.WriteLine("$upscope $end");
            filewriter.WriteLine("$enddefinitions $end");
            filewriter.WriteLine();
        }

        
/// <summary>
/// Writes signal changes to the file
/// </summary>
/// <param name="cs">Signals</param>
/// <param name="time">Time</param>
        public void WriteSignals(List<SystemDotNet.SignalBase> cs, long time)
        {
            //Reset indexes for asci array
            i1 = i2 = i3 = 0;

            //Holds output
            StringBuilder sb = new StringBuilder();

            //Holds label
            string clabel = "";

            //Prev holds previous value. This checks that prev has the right length
            if (cs.Count != prev.Length)
                prev = new string[cs.Count];

            //Foreach signal
            for (int i = 0; i < cs.Count; i++)
            {
                //Get labe. Note that if the number of signals changes between WriteNames
                //WriteSignals the label will be wrong.
                clabel = GetLabel();

                //Get signal
                SignalBase c = cs[i];

                //Check signal type
                if (c is Signal<bool>)
                {
                    //If it is bool; convert to string (0 or 1) and check if it has changed. If it has changed append it to the string builder
                    bool b = ((Signal<bool>)c).Read();
                    string sbool = "0";
                    if (b)
                        sbool = "1";
                    if (prev[i] != sbool)
                        sb.Append(sbool + clabel + '\n');
                    prev[i] = sbool;

                }
                else if (c is SignalCollection<bool>)
                {
                    //If it is bool[]; convert to string (i.e 00010) and check if it has changed. If it has changed append it to the string builder
                    string val;
                    SignalCollection<bool>.Convert(((SignalCollection<bool>)c).Read(), out val);

                    if (prev[i] != val)
                        sb.Append("b" + val + " " + clabel + '\n');
                    prev[i] = val;

                }
                else if (c is SignalCollection<ulong>)
                {
                    SignalCollection<ulong> sc = c as SignalCollection<ulong>;
                    for (int j = 0; j < sc.Count; j++)
                    {

                        BitVectorUnsigned bc = new BitVectorUnsigned(64);
                        bc.Value = sc[j].Value;
                        string val = ((BitVectorUnsigned)bc).ToBinaryString();

                        if (prev[i] != val)
                            sb.Append("b" + val + " " + GetLabel() + '\n');
                        prev[i] = val;
                    }

//                    //If it is bool[]; convert to string (i.e 00010) and check if it has changed. If it has changed append it to the string builder
//                    string val;
//                    BitVectorUnsigned bc = new BitVectorUnsigned();
//                    bc.Value = 
//                    SignalCollection<bool>.Convert(((SignalCollection<bool>)c).Read(), out val);
//
//                    if (prev[i] != val)
//                        sb.Append("b" + val + " " + clabel + '\n');
//                    prev[i] = val;

                }
                else if (c is BitVectorUnsigned)
                {
                    string val = ((BitVectorUnsigned)c).ToBinaryString();
                    if (prev[i] != val)
                        sb.Append("b" + val + " " + clabel + '\n');
                    prev[i] = val;
                }
                else if (c is Signal<Enum>)
                {
                    //If it is enum; convert to string (i.e Add) and check if it has changed. If it has changed append it to the string builder
                    string senum = ((Signal<Enum>)c).Read().ToString();
                    if (prev[i] != senum)
                        sb.Append("s" + senum + " " + clabel + '\n');
                    prev[i] = senum;
                }
                else if (c is Signal<double>)
                {
                    //If it is double; convert to string (i.e 0.1) and check if it has changed. If it has changed append it to the string builder
                    string sreal = ((Signal<double>)c).Read().ToString();
                    if (prev[i] != sreal)
                        sb.Append("r" + sreal + " " + clabel + '\n');
                    prev[i] = sreal;
                }
                else if (c is SignalCollection<double>)
                {
                    StringBuilder sb1 = new StringBuilder();
                    SignalCollection<double> sc = c as SignalCollection<double>;
                    sb1.Append("r" + sc[0].Read().ToString() + " " + clabel + '\n');
                    for (int j = 1; j < sc.Count; j++)
                    {
                        sb1.Append("r" + sc[j].Read().ToString() + " " + GetLabel() + '\n');
                    }

                    string screal = sb1.ToString();
                    if (prev[i] != screal)
                        sb.Append(screal);
                    prev[i] = screal;
//                }else if (c is SignalCollection<ulong>){
//                    foreach(Signal<ulong> in
               }
                else
                {
                    //If it is otherwise; convert to string and check if it has changed. If it has changed append it to the string builder.
                    string sobj = c.ToString();
                    if (prev[i] != sobj)
                        sb.Append("r" + sobj + " " + clabel + '\n');
                    prev[i] = sobj;
                }
            }

            //If any signals changed write them to the file
            if (sb.ToString().Length > 0)
            {
                filewriter.WriteLine("#" + time);
                filewriter.Write(sb.ToString());
                filewriter.WriteLine();
            }
        }

		#endregion

//        void OpenGtkWave()
//        {
//            try
//            {
//                string prog = ConfigurationSettings.AppSettings["GtkWave"];
//                if (prog != null)
//                {
//                    if (File.Exists(prog))
//                    {
//                        ProcessStartInfo psi = new ProcessStartInfo(prog, Variables.GetInstance().StartupPath + "\\" + filename);
//                        psi.CreateNoWindow = true;
//                        psi.UseShellExecute = false;
//                        Process p = new Process();
//                        p.StartInfo = psi;
//                        p.StartInfo.RedirectStandardError = true;
//                        p.StartInfo.RedirectStandardOutput = false;
//                        p.Start();
//
//                        System.Threading.Thread.Sleep(500);
//
//                        Simulator.GetInstance().SendMessage(p.StandardError.ReadToEnd());
//                        Simulator.GetInstance().SendMessage(p.StandardOutput.ReadToEnd());
//
//
//                    }
//                    else
//                    {
//                        Simulator.GetInstance().SendMessage(prog + "does not exist");
//
//                        //MessageBox.Show(prog + " does not exist");
//                    }
//                }
//                else
//                {
//
//                    //Simulator.GetInstance().SendMessage("Can't find GTKWave in config file");
//                    //MessageBox.Show("Can't find GTKWave in config file");
//                }
//            }
//            catch (Exception ex)
//            {
//                Simulator.GetInstance().SendMessage(ex.Message);
//                //MessageBox.Show(ex.Message);
//            }
//        }
    }
}
