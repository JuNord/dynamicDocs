using DynamicDocsWPF.Model.Base;

namespace DynamicDocsWPF.Model.Process
{
    public class ArchivePermissionElement : Tag
    {
        public string Target { get; }
        
        public ArchivePermissionElement(Tag parent, string target) : base(parent)
        {
            Target = target;
        }
    }
}