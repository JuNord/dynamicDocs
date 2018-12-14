using System.Collections.Generic;
using RestService.Model.Database;

namespace RestService.RestDTOs
{
    public class ReplyGetEntryList
    {
        public List<Entry> Entries { get; set; }
    }
}