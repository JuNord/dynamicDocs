using System;
using RestService.Model.Base;

namespace RestService.Model.Process
{
    public class MailNotificationElement : Tag, INotificationElement
    {
        public MailNotificationElement(Tag parent) : base(parent)
        {
        }

        public string Target { get; set; }
        public string Text { get; set; }

        public void Send()
        {
            throw new NotImplementedException();
        }
    }
}