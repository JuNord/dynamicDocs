using RestService.Model.Base;

namespace RestService.Model.Process
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