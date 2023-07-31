using Core.Interfaces;
using infrastructure.Data;
using infrastructure.Service;
using Microsoft.AspNetCore.Mvc;
using StoreAPI.Helpers;
using StoreAPI.ResponseModule;

namespace StoreAPI.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection Services)
        {
            Services.AddScoped<IProductRepository, ProductRepository>();
            Services.AddScoped<IBasketRepository, BasketRepository>();
            Services.AddScoped<ITokenService, TokenService>();
            Services.AddScoped<IUnitOfWork, UnitOfWork>();
            Services.AddScoped<IOrderService, OrderService>();
            Services.AddScoped<IPaymentService, PaymentService>();
            Services.AddScoped<iResponseCacheService, ResponseCacheService>();
            Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            Services.AddAutoMapper(typeof(MappingProfiles));
            Services.AddApiBehaviorOptions();
            return Services;
        }
        private static void AddApiBehaviorOptions(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    var errorResponse = new ApiValidationErrorResponse { Errors = errors };
                    return new BadRequestObjectResult(errorResponse);
                };
            });
        }
    }
}
