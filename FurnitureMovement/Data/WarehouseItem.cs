using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FurnitureMovement.Data
{
    public class WarehouseItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "#")]
        public int ID { get; set; }

        [Required]
        public string? FurnitureName { get; set; }

        [Required]
        public int FurnitureNameId { get; set; }

        [Required]
        public long Quantity { get; set; }

        [Required]
        public DateTime AdmissionDate { get; set; }

        public string? Material { get; set; } 
        public string? Drawing { get; set; }  
        public string? Image { get; set; }
    }
}