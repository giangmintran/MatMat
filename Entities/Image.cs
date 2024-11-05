using System;
using System.Collections.Generic;

namespace MatMatShop.Entities
{
    public class Image
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = null!;
        public string Tags { get; set; } = null!;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public List<OrderDetail>? OrderDetails { get; set; }

    }

}
