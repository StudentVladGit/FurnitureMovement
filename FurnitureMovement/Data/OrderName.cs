using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FurnitureMovement.Data
{
    public class OrderName
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string? Name { get; set; }  // Само название

        // Дополнительные поля, если нужно (описание, категория и т.д.)
        //public string? Drawing { get; set; }
    }
}
