using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SystemDotNet;

namespace SystemDotNet.PostProcessing
{
    public abstract class OutputWriterBase : SystemDotNet.PostProcessing.IOutputWriter
    {
        private string _FileName;

        public string FileName
        {
            get { return _FileName; }
        }

        private string  _FilePath;

        public string FilePath
        {
            get { return _FilePath; }
            set { _FilePath = value; }
        }


        private bool _UseAdvancedFileName;

        public bool UseAdvancedFileName
        {
            get { return _UseAdvancedFileName; }
        }

        private bool _ShowTime;

        public bool ShowTime
        {
            get { return _ShowTime; }
        }

        private bool _ShowHeader;

        public bool ShowHeader
        {
            get { return _ShowHeader; }
        }

        List<SignalBase> signalList = new List<SignalBase>();
        public List<SignalBase> SignalList { get { return signalList; } }


        public OutputWriterBase(string FileName):this(FileName,true,true,false){}
        
        public OutputWriterBase(string FileName, bool ShowHeader, bool UseAdvancedFileName, bool ShowTime)
        {
            this._FileName = FileName;
            this._ShowHeader = ShowHeader;
            this._UseAdvancedFileName = UseAdvancedFileName;
            this._ShowTime = ShowTime;

            if (UseAdvancedFileName)
            {
                if (!Directory.Exists(Simulator.Settings.OutputDir))
                    Directory.CreateDirectory(Simulator.Settings.OutputDir);

                _FilePath = Simulator.Settings.OutputDir + "/" + Simulator.Settings.FileNamePrefix + "_" + FileName;
            }
            else
            {
                _FilePath = FileName;
            }
        }

        public void Clear()
        {
            signalList.Clear();
        }
        public void Add(SignalBase sb){
            signalList.Add(sb);
        }

        public void Open()
        {
            OnOpen();

            if (ShowHeader)
            {

                List<string> header = new List<string>();

                if (ShowTime)
                {
                    header.Add("Time");
                }

                foreach (SignalBase ssb in signalList)
                {
                        header.Add(ssb.FullName.Trim());
                }

                WriteHeader(header);
            }
          
        }

        public void Close()
        {
            OnClose();
        }

        protected abstract void OnClose();
        protected abstract void OnOpen();
        protected abstract void WriteHeader(List<string> header);
        public abstract void WriteValues();
        public abstract OutputReaderBase GetReader();

    }
}
