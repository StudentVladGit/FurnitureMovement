using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace FurnitureMovement.Data
{
    // OrderFurniture.cs
    public class Furniture
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "#")]
        public int ID { get; set; }

        [Required]
        public int FurnitureNameID { get; set; } // Было OrderNameID

        [ForeignKey("FurnitureNameID")]
        public FurnitureName? FurnitureName { get; set; } // Было OrderName

        [Range(1, 99999)]
        public long OrderQuantity { get; set; }

        // Внешний ключ для связи с Order
        public int OrderID { get; set; }
        // Навигационное свойство
        public Order? Order { get; set; }
    }
}
