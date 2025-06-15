﻿using System.ComponentModel.DataAnnotations.Schema;

namespace Shopping.Models
{
    public class OrderDetailModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string OrderCode {  get; set; }
        public int ProductId {  get; set; }
        public decimal Price { get; set; }  
        public int Quantity { get; set; }

        [ForeignKey("ProductId")]
        public ProductModel Product { get; set; }
        [ForeignKey("OrderId")]
        public OrderModel Order { get; set; }
    }
}
