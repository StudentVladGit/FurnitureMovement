using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FurnitureMovement.Data
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "#")]
        public string Number {  get; set; } //Номер
        public string Drawning { get; set; } //Схемы
        public string Furniture { get; set; } //Оснастка
        public DateTime AdmissionDate { get; set; } //Поступление
        public long Quantity { get; set; } //Количество


    }
}
