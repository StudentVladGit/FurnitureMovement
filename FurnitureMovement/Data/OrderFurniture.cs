using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace FurnitureMovement.Data
{
    // OrderFurniture.cs
    public class OrderFurniture
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "#")]
        public int ID { get; set; }

        // Внешний ключ для связи с OrderName
        [Required]
        public int OrderNameID { get; set; } = 0;

        // Навигационное свойство
        [ForeignKey("OrderNameID")]
        public OrderName? OrderName { get; set; }

        [Range(1, 99999)]
        public long OrderQuantity { get; set; }

        // Внешний ключ для связи с Order
        public int OrderID { get; set; }
        // Навигационное свойство
        public Order? Order { get; set; }
    }
}
