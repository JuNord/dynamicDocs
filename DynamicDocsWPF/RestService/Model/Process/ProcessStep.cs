using System.Collections.Generic;
using RestService.Model.Base;
using RestService.Model.Input;

namespace RestService.Model.Process
{
    public class ProcessStep : NamedTag
    {
        private readonly List<Dialog> _dialogs;
        private readonly List<INotificationElement> _notifications;
        private readonly List<ReceiptElement> _receipts;
        private readonly List<ValidationElement> _validations;

        public ProcessStep(Tag parent, string name, string description, string target) : base(parent, name, description)
        {
            _dialogs = new List<Dialog>();
            _receipts = new List<ReceiptElement>();
            _validations = new List<ValidationElement>();
            _notifications = new List<INotificationElement>();
            Target = target;
        }

        public string Target { get; set; }

        public int DialogCount => _dialogs?.Count ?? 0;
        public int ReceiptCount => _receipts?.Count ?? 0;
        public int ValidationCount => _validations?.Count ?? 0;
        public int NotificationCount => _notifications?.Count ?? 0;

        public void AddDialog(Dialog dialog)
        {
            _dialogs.Add(dialog);
        }

        public void AddReceipt(ReceiptElement receipt)
        {
            _receipts.Add(receipt);
        }

        public void AddValidation(ValidationElement validation)
        {
            _validations.Add(validation);
        }

        public void AddNotification(INotificationElement notification)
        {
            _notifications.Add(notification);
        }

        public Dialog GetDialogAtIndex(int index)
        {
            return _dialogs?[index];
        }
        
        public ReceiptElement GetReceiptAtIndex(int index)
        {
            return _receipts?[index];
        }
        
        public ValidationElement GetValidationAtIndex(int index)
        {
            return _validations?[index];
        }
        
        public INotificationElement GetNotificationAtIndex(int index)
        {
            return _notifications?[index];
        }
    }
}