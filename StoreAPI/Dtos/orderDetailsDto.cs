using Core.Entities.orderAggregate;

namespace StoreAPI.Dtos
{
    public class OrderDetailsDto
    {
        public int Id { get; set; }
        public string BuyerEmail { get; set; }
        public DateTimeOffset OrderDate { get; set; }
        public ShippingAddressDto ShippingAddress { get; set; }
        public string DeliveryMethod { get; set; }
        public IReadOnlyList<OrderItemDto> OrderItems { get; set; }
        public decimal SubTotal { get; set; }
        public string OrderStatus { get; set; }
        public decimal Total { get; set; }
        public decimal ShippingPrice{ get; set;}
        public int PaymentIntentId { get; set; }

    }
}
