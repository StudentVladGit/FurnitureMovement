using System.ComponentModel;

namespace FurnitureMovement.Data
{
    public static class OrderStatusMethods
    {
        public static string GetName(this OrderStatus state)
        {
            var field = typeof(OrderStatus).GetField(state.ToString());

            if (field != null)
            {
                var attributes = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            }
            throw new Exception($"The name for the status \"{state}\" was not found, perhaps you did not use the DescriptionAttribute when declaring the status!");
        }
    }
}
