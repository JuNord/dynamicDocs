using RestService.Model.Base;

namespace RestService.Model.Process
{
    public class ArchivePermissionElement : Tag
    {
        public ArchivePermissionElement(Tag parent, string target) : base(parent)
        {
            Target = target;
        }

        public string Target { get; }
    }
}