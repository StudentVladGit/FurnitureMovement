using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace FurnitureMovement.Data
{
    public enum Division: long
    {
        //[Description("ОАСУ")]
        //OACY = 0,

        //[Description("ОИТ")]
        //OIT = 1,

        //[Description("Отдел безопасности")]
        //Security = 2

        [Description("Отдел разработки и проектирования")]
        Div1 = 0,

        [Description("Производственный отдел")]
        Div2 = 1,

        [Description("Отдел автоматизации производства")]
        Div3 = 2,

        [Description("Отдел контроля качества")]
        Div4 = 3,

        [Description("Отдел прототипирования")]
        Div5 = 4,
    }
    public class OrderAuthor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string? Name { get; set; }
        public int DeleteIndicator { get; set; } = 0;
        public Division Division { get; set; }
    }
}