using System.Diagnostics;

namespace DynamicDocsWPF.HelperClasses
{
    public interface IReceiptHelper
    {
        void FillReceipt(string template, string path, Process process);
    }
}