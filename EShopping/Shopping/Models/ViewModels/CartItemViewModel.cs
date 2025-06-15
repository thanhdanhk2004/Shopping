namespace Shopping.Models.ViewModels
{
    public class CartItemViewModel
    {
        public List<CartItemModel> CartItems { get; set; }
        public decimal GrandTotal {  get; set; }
        public decimal PriceShipping {  get; set; }
        public string CouponTitle {  get; set; }
    }
}
