using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Net.NetworkInformation;
using System.Reflection;

namespace FurnitureMovement.Data
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "#")]
        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string? OrderNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string? OrderName { get; set; }

        public List<OrderFurniture>? Orders { get; set; }
    }
}
