using DynamicDocsWPF.Model.Base;

namespace DynamicDocsWPF.Model.Process
{
    public class ValidationElement : Tag
    {
        public bool Locks { get; set; }
        
        public ValidationElement(Tag parent, bool locks) : base(parent)
        {
            Locks = locks;
        }
    }
}