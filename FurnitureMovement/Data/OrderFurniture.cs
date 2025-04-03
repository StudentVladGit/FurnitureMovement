using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

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
    // OrderFurniture.cs
    public class OrderFurniture
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "#")]
        public int ID { get; set; }

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
        public string? OrderAuthor { get; set; }

        // Внешний ключ для связи с Order
        public int OrderID { get; set; }

        // Навигационное свойство
        public Order? Order { get; set; }
    }
}
