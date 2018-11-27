using DynamicDocsWPF.Model.Base;

namespace DynamicDocsWPF.Model.Process
{
    public class ArchivePermissionElement : Tag
    {
        public string Target { get; set; }
        
        public ArchivePermissionElement(Tag parent) : base(parent)
        {
            
        }
    }
}