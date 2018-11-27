using DynamicDocsWPF.Model.Base;

namespace DynamicDocsWPF.Model.Process
{
    public class ReceiptElement : Tag
    {
        public string DraftName { get; set; }
        public string FilePath { get; set; }

        public ReceiptElement(Tag parent) : base(parent)
        {
            
        }
    }
}