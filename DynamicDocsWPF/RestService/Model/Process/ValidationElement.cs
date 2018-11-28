using RestService.Model.Base;

namespace RestService.Model.Process
{
    public class ValidationElement : Tag
    {
        public ValidationElement(Tag parent, bool locks) : base(parent)
        {
            Locks = locks;
        }

        public bool Locks { get; set; }
    }
}