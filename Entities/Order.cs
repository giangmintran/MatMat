using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MatMatShop.Entities
{
    public class Order
    {
        public int Id { get; set; }

        [MaxLength(512)]
        public string CustomerName { get; set; } = null!;

        [MaxLength(20)]
        public string Phone { get; set; } = null!;

        [MaxLength(2048)]
        public string? Address { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public List<OrderDetail> OrderDetails { get; set; }
    }
}
