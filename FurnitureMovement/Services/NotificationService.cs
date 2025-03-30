namespace FurnitureMovement.Data
{
    public class NotificationService
    {
        private readonly List<Notification> _notifications = new();
        public event Action OnChange;

        public void AddNotification(string message, OrderStatus status)
        {
            var color = status switch
            {
                OrderStatus.Generated => "bg-light",
                OrderStatus.Decorated => "bg-info text-white",
                OrderStatus.InThePreparationProcess => "bg-warning",
                OrderStatus.Manufactured => "bg-primary text-white",
                OrderStatus.Completed => "bg-success text-white",
                OrderStatus.Cancelled => "bg-danger text-white",
                _ => "bg-light"
            };

            _notifications.Insert(0, new Notification
            {
                Id = _notifications.Count + 1,
                Message = message,
                Timestamp = DateTime.Now,
                ColorClass = color
            });

            OnChange?.Invoke();
        }

        public List<Notification> GetNotifications() => _notifications;

        public int GetUnreadCount() => _notifications.Count;

        public void ClearNotifications()
        {
            _notifications.Clear();
            OnChange?.Invoke();
        }
    }
}