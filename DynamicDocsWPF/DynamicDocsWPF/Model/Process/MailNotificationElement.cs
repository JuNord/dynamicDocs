using DynamicDocsWPF.Model.Base;

namespace DynamicDocsWPF.Model.Process
{
    public class MailNotificationElement : Tag
    {
        public string Target { get; set; }
        public string Text { get; set; }
        
        public MailNotificationElement(Tag parent) : base(parent)
        {
        }
    }
}