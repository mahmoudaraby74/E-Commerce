using AutoMapper;
using Core.Entities;
using Microsoft.EntityFrameworkCore.Metadata;
using StoreAPI.Dtos;

namespace StoreAPI.Helpers
{
    public class ProductUrlResolver : IValueResolver<Product, ProductDto, string>
    {
        private readonly IConfiguration configuration;

        public ProductUrlResolver(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public string Resolve(Product source, ProductDto destination, string destMember, ResolutionContext context)
        {
            if (source.PictureUrl != null)
                return configuration["ApiUrl"] + source.PictureUrl;
                    return null;
        }
    }
}
