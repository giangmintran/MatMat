namespace MatMatShop.Entities
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;
        public int ImageId { get; set; }
        public Image Image { get; set; } = null!;
    }
}
