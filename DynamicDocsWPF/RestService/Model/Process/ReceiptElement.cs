using RestService.Model.Base;

namespace RestService.Model.Process
{
    public class ReceiptElement : Tag
    {
        public ReceiptElement(Tag parent, string draftName, string filePath, ReceiptType receiptType) : base(parent)
        {
            DraftName = draftName;
            FilePath = filePath;
            ReceiptType = receiptType;
        }

        public ReceiptType ReceiptType { get; }
        public string DraftName { get; }
        public string FilePath { get; }
    }
}