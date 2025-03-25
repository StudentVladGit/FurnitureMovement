using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FurnitureMovement.Data
{
    public enum OrderStatus
    {
        Сформирован,
        Оформлен,
        ВПроцессеИзготовления,
        Изготовлено,
        Выполнен,
        Отменен
    }
    public enum OrderAuthor
    {
        ШульженкоВМ,
        ИвановИИ,
        ПетровПП
    }
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
        public OrderStatus OrderStatus { get; set; }

        [Column(TypeName = "date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime AdmissionDate { get; set; }

        [Required]
        [StringLength(50)]
        public OrderAuthor OrderAuthor { get; set; }
    }
}
