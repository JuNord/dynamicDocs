using System;
using RestService.Model.Base;

namespace RestService.Model.Process
{
    public class MailNotificationElement : Tag, INotificationElement
    {
        public MailNotificationElement(Tag parent, string target, string text) : base(parent)
        {
            Target = target;
            Text = text;
        }

        public string Target { get; }
        public string Text { get; }

        public void Send()
        {
            throw new NotImplementedException();
        }
    }
}