using System.Collections.Generic;
using RestService.Model.Database;

namespace WebServerWPF.RestDTOs
{
    public class ReplyGetEntryList
    {
        public List<Entry> Entries { get; set; }
    }
}