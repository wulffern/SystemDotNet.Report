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

#endregion

namespace SystemDotNet.PostProcessing
{
    public abstract class OutputReaderBase 
    {
        protected string filename;

        short sensitivity = 0;

        protected bool IsNoFile;

        private bool hasheader = true;

        public bool HasHeader
        {
            get { return hasheader; }
        }

        public event ProgressHandler UpdatedProgress;

        private short _Progress;

        public short Progress
        {
            get { return _Progress; }
            set
            {
                short progress = _Progress;
                _Progress = value;
                if (Math.Abs(_Progress - progress) > sensitivity)
                    if (UpdatedProgress != null)
                        UpdatedProgress(_Progress);
            }
        }


        public OutputReaderBase(string filename, bool hasheader)
        {
            this.hasheader = hasheader;

            this.filename = filename;

        }


        public List<double> Read(string header)
        {
            if (File.Exists(filename)||IsNoFile)
            {
                Progress = 0;
                return ReadFile(header);
            }
            else
                return new List<double>();
        }

        public virtual Dictionary<string, List<double>> Read()
        {
            if (File.Exists(filename) || IsNoFile)
            {
                Progress = 0;
                return ReadFile();
            }
            else
                return new Dictionary<string, List<double>>();
        }

        public Dictionary<string, List<double>> Read(List<SignalBase> signals)
        {
            if (File.Exists(filename) || IsNoFile)
            {
                Progress = 0;
                List<string> headers = new List<string>();
                foreach (SignalBase sb in signals)
                {
                    headers.Add(sb.FullName.Trim());
                }
                return ReadFile(headers);
            }
            else
                return new Dictionary<string, List<double>>();
            
        }

        protected virtual List<double> ReadFile(string header)
        {
            Dictionary<string, List<double>> list;
            List<string> headers = new List<string>();
            headers.Add(header.Trim());
            list = ReadFile(headers);
            if (list != null && list.ContainsKey(header))
                return list[header];
            else
                return null;
        }

        protected virtual Dictionary<string, List<double>> ReadFile()
        {
            return ReadFile((List<string>)null);
        }

        public abstract List<string> ReadHeader();
        protected abstract Dictionary<string, List<double>> ReadFile(List<string> headers);
        


    }
}
