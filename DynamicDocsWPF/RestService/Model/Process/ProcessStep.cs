using System.Collections.Generic;
using RestService.Model.Base;
using RestService.Model.Input;

namespace RestService.Model.Process
{
    public class ProcessStep : NamedTag
    {
        public string Target { get; set; }
        private readonly List<Dialog> _dialogs;
        private readonly List<ReceiptElement> _receipts;
        private readonly List<ValidationElement> _validations;
        private readonly List<INotificationElement> _notifications;
        
        public ProcessStep(Tag parent, string name, string description, string target) : base(parent, name, description)
        {
            _dialogs = new List<Dialog>();
            _receipts = new List<ReceiptElement>();
            _validations = new List<ValidationElement>();
            _notifications = new List<INotificationElement>();
            Target = target;
        }

        public void AddDialog(Dialog dialog) => _dialogs.Add(dialog);
        public void AddReceipt(ReceiptElement receipt) => _receipts.Add(receipt);
        public void AddValidation(ValidationElement validation) => _validations.Add(validation);
        public void AddNotification(INotificationElement notification) => _notifications.Add(notification);

        public int DialogCount => _dialogs?.Count??0;
        public int ReceiptCount => _receipts?.Count??0;
        public int ValidationCount => _validations?.Count??0;
        public int NotificationCount => _notifications?.Count??0;
        
        public Dialog GetDialogAtIndex(int index) => _dialogs?[index];
    }
}