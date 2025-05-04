using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FurnitureMovement.Data
{
    public class FurnitureName
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string? Name { get; set; }  // Само название
        public string? Material { get; set; }  
        public string? ProductionTime { get; set; }
        public string Drawing { get; set; } = "Не указано";
        public string Image { get; set; } = "Не указано";
        public int DeleteIndicator { get; set; } = 0;
    }
}
