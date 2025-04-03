namespace FurnitureMovement.Data
{
    public class NotificationService
    {
        private readonly List<Notification> _notifications = new();
#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
        public event Action OnChange;
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.

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