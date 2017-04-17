using System;
namespace SystemDotNet.PostProcessing
{
    public interface IPrintReport
    {
        string Print(string signalName,System.Collections.Generic.List<double> d);
    }
}
