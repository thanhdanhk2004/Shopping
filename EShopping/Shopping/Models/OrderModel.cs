using System.ComponentModel.DataAnnotations.Schema;

namespace Shopping.Models
{
    public class OrderModel
    {
        public int Id { get; set; }
        public string OrderCode {  get; set; }
        public string Username {  get; set; }
        public DateTime CreatedDate { get; set; }
        public int Status {  get; set; }
        public decimal PriceShipping {  get; set; }
        public string CouponCode {  get; set; }
        public string? PaymentMethod {  get; set; }
        
    }
}
