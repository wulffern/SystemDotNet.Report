using System;
namespace SystemDotNet.PostProcessing
{
    public interface IOutputWriter
    {
        void Add(SignalBase sb);
        void Close();
        string FileName { get; }
        string FilePath { get; set; }
        OutputReaderBase GetReader();
        void Open();
        bool ShowHeader { get; }
        bool ShowTime { get; }
        System.Collections.Generic.List<SignalBase> SignalList { get; }
        bool UseAdvancedFileName { get; }
        void WriteValues();
    }
}
