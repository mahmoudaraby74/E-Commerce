using AutoMapper;
using Core.Entities;
using Core.Entities.Identity;
using Core.Entities.orderAggregate;
using StoreAPI.Dtos;

namespace StoreAPI.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.ProductBrand, option => option.MapFrom(src => src.ProductBrand.Name))
                .ForMember(dest => dest.ProductType, option => option.MapFrom(src => src.ProductType.Name))
                .ForMember(dest => dest.PictureUrl, option => option.MapFrom<ProductUrlResolver>());
            CreateMap<CustomerBasket, CustomerBasketDto>().ReverseMap();
            CreateMap<BasketItem, BasketItemDto>().ReverseMap();
            CreateMap<Address, AddressDto>().ReverseMap();
            CreateMap<ShippingAddress, ShippingAddressDto>().ReverseMap();
            CreateMap<Order, OrderDetailsDto>()
                .ForMember(d => d.DeliveryMethod , o => o.MapFrom(src =>src.DeliveryMethod.ShortName))
                .ForMember(d => d.ShippingPrice, o => o.MapFrom(src => src.DeliveryMethod.Price));
            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(d => d.ProductId, o => o.MapFrom(src => src.ItemOdered.ProductItemId))
                .ForMember(d => d.ProductName, o => o.MapFrom(src => src.ItemOdered.ProductName))
                .ForMember(d => d.PictureUrl, o => o.MapFrom(src => src.ItemOdered.PictureUrl))
                .ForMember(d => d.PictureUrl, o => o.MapFrom<OrderItemUrlResolver>());

        }
    }
}
