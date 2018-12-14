using System.Collections.Generic;
using RestService.Model.Base;

namespace RestService.Model.Process
{
    public class ValidationDeclined : Tag
    {
        private readonly List<INotificationElement> _notifications;
        private readonly List<ReceiptElement> _receipts;

        public ValidationDeclined(Tag parent) : base(parent)
        {
            _receipts = new List<ReceiptElement>();
            _notifications = new List<INotificationElement>();
            Receipts = new CustomEnumerable<ReceiptElement>(_receipts);
            Notifications = new CustomEnumerable<INotificationElement>(_notifications);
        }

        public CustomEnumerable<INotificationElement> Notifications { get; }
        public CustomEnumerable<ReceiptElement> Receipts { get; }

        public int ReceiptCount => _receipts?.Count ?? 0;
        public int NotificationCount => _notifications?.Count ?? 0;


        public void AddReceipt(ReceiptElement receipt)
        {
            _receipts.Add(receipt);
        }

        public void AddNotification(INotificationElement notification)
        {
            _notifications.Add(notification);
        }

        public INotificationElement GetNotificationAtIndex(int index)
        {
            return _notifications?[index];
        }

        public ReceiptElement GetReceiptAtIndex(int index)
        {
            return _receipts?[index];
        }
    }
}