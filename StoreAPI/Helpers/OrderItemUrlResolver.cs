using AutoMapper;
using Core.Entities;
using Core.Entities.orderAggregate;
using StoreAPI.Dtos;

namespace StoreAPI.Helpers
{
    public class OrderItemUrlResolver : IValueResolver<OrderItem, OrderItemDto, string>
    {
        private readonly IConfiguration configuration;

        public OrderItemUrlResolver(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public string Resolve(OrderItem source, OrderItemDto destination, string destMember, ResolutionContext context)
        {
            if (source.ItemOdered.PictureUrl != null)
                return configuration["ApiUrl"] + source.ItemOdered.PictureUrl;
            return null;
        }
    }
}
