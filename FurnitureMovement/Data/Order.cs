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
    public enum OrderAuthor : long
    {
        [Description("ШульженкоВМ")]
        ShulzhenkoVM = 0,

        [Description("ИвановИИ")]
        IvanovII = 1,

        [Description("ПетровПП")]
        PetrovPP = 2
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

        public List<OrderFurniture>? Orders { get; set; }
    }

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
        public OrderAuthor OrderAuthor { get; set; }

        public Order? Order { get; set; }
    }

    public static class OrderStatusMethods
    {
        public static string GetName(this OrderStatus state)
        {
            var field = typeof(OrderStatus).GetField(state.ToString());

            if (field != null)
            {
                var attributes = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            }
            throw new Exception($"The name for the status \"{state}\" was not found, perhaps you did not use the DescriptionAttribute when declaring the status!");
        }

    }
}
