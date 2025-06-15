using System.ComponentModel.DataAnnotations;

namespace Shopping.Models
{
    public class StatisticModel
    {
        [Key]
        public int Id { get; set; }
        public int Quantity {  get; set; } // So luong ban
        public int Sold {  get; set; } // So luong don hang
        public decimal Revenue { get; set; }
        public decimal Profit {  get; set; } // Loi nhuan
        public DateTime DateCreate { get; set; }
    }
}
