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

        [Description("В Процессе Изготовления")]
        InThePreparationProcess = 2,

        [Description("Изготовлено")]
        Manufactured = 3,

        [Description("Выполнен")]
        Completed = 4,

        [Description("Отменен")]
        Cancelled = 5
    }

    public enum OrderPriority : long
    {
        [Description("Обычный")]
        Usual = 0,

        [Description("Важный")]
        Important = 1,

        [Description("Быстрый")]
        Fast = 2,

        [Description("Срочный")]
        Emergency = 3
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

		[Required]
		[StringLength(50)]
		public OrderPriority OrderPriority { get; set; }

		public int OrderAuthorID { get; set; }
        public int DeleteIndicator { get; set; } = 0;

        [ForeignKey("OrderAuthorID")]
        public OrderAuthor? OrderAuthor { get; set; }
        public List<Furniture>? Furnitures { get; set; }
    }
}
