namespace Core.Entities.orderAggregate
{
    public class OrderItem:BaseEntity
    {
        public OrderItem(ProductItemOdered itemOdered, decimal price, int quantity)
        {
            ItemOdered = itemOdered;
            Price = price;
            Quantity = quantity;
        }
        public OrderItem()
        {
            
        }

        public int Id { get; set; }
        public ProductItemOdered  ItemOdered { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

    }
}