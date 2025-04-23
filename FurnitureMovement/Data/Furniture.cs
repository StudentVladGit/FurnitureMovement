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
        public int FurnitureNameID { get; set; }

        [ForeignKey("FurnitureNameID")]
        public FurnitureName? FurnitureName { get; set; } 

        [Range(1, 99999)]
        public long OrderQuantity { get; set; }
        public int OrderID { get; set; }
        public Order? Order { get; set; }
    }
}
