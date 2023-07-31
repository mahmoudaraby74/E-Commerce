using Core.Entities.orderAggregate;

namespace StoreAPI.Dtos
{
    public class OrderDto
    {
        public string BasketId { get; set; }
        public ShippingAddressDto ShippingAddress { get; set; }
        public int DeliveryMethodId { get; set; }

    }
}
