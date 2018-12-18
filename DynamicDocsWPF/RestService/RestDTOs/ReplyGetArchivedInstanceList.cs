using System.Collections.Generic;
using RestService.Model.Database;

namespace RestService.RestDTOs
{
    public class ReplyGetArchivedInstanceList
    {
        public List<ProcessInstance> Instances { get; set; }
    }
}