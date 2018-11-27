using DynamicDocsWPF.Model.Base;
using RestService;

namespace DynamicDocsWPF.Model.Process
{
    public class ReceiptElement : Tag
    {
        public ReceiptType ReceiptType { get; }
        public string DraftName { get; }
        public string FilePath { get; }

        public ReceiptElement(Tag parent, string draftName, string filePath, ReceiptType receiptType) : base(parent)
        {
            DraftName = draftName;
            FilePath = filePath;
            ReceiptType = receiptType;
        }
    }
}