using Core.Entities;

namespace StoreAPI.Dtos
{
    public class CustomerBasketDto
    {
        public string Id { get; set; }
        public int? DeliveryMethodId { get; set; }
        public int ShipingPrice { get; set; }
        public List<BasketItemDto> basketItems { get; set; } = new List<BasketItemDto>();
        public string? PaymentIntentId { get; set; }
        public string? ClientSecret { get; set; }
    }
}
