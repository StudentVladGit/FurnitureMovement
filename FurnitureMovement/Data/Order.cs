using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

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
        public string OrderNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string OrderName { get; set; }

        [Range(1, 99999)]
        public long OrderQuantity { get; set; }

        [Required]
        [StringLength(50)]
        public string OrderStatus { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime AdmissionDate { get; set; }

        [Required]
        [StringLength(50)]
        public string OrderAuthor { get; set; }
    }
}
