using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Net.NetworkInformation;
using System.Reflection;

namespace FurnitureMovement.Data
{
    public enum OrderStatus : long
    {
        [Description("Сформирован")]
        Generated = 0,

        [Description("Оформлен")]
        Decorated = 1,

        [Description("ВПроцессеИзготовления")]
        InThePreparationProcess = 2,

        [Description("Изготовлено")]
        Manufactured = 3,

        [Description("Выполнен")]
        Completed = 4,

        [Description("Отменен")]
        Cancelled = 5
    }
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "#")]
        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string? OrderNumber { get; set; }

        [Column(TypeName = "date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime AdmissionDate { get; set; }

        [Required]
        [StringLength(50)]
        public OrderStatus OrderStatus { get; set; }
        // Внешний ключ для связи с OrderAuthor
        public int OrderAuthorID { get; set; }

        // Навигационное свойство
        [ForeignKey("OrderAuthorID")]
        public OrderAuthor? OrderAuthor { get; set; }

        public List<Furniture>? Furnitures { get; set; }
    }
}
