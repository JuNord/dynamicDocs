using System.Collections.Generic;
using RestService.Model.Base;

namespace RestService.Model.Process
{
    public class ValidationElement : Tag
    {
        public bool Locks { get; }
        public ValidationAccepted Accepted { get; set; }
        public ValidationDeclined Declined { get; set; }
        
        public ValidationElement(Tag parent, bool locks) : base(parent)
        {
            Locks = locks;
        }  
    }
}