using Core.Entities;
using Core.Entities.orderAggregate;
using Core.Interfaces;
using Core.Specifications;
using infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Stripe;
using Product = Core.Entities.Product;

namespace infrastructure.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IBasketRepository basketRepository;
        private readonly IConfiguration configuration;

        public PaymentService(IUnitOfWork unitOfWork,IBasketRepository basketRepository,IConfiguration configuration)
        {
            this.unitOfWork = unitOfWork;
            this.basketRepository = basketRepository;
            this.configuration = configuration;
        }
        public async Task<CustomerBasket> CreateOrUpdatePaymentIntent(string basketId)
        {
            var basket = await basketRepository.GetBasketAsync(basketId);

            if (basket == null)
            {
                return null;
            }
            var ShippingPrice = 0m;
            if(basket.DeliveryMethodId.HasValue)
            {
                var deliveryMethod=await unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(basket.DeliveryMethodId.Value);
                ShippingPrice = deliveryMethod.Price;
            }
            foreach (var item in basket.BasketItems)
            {
                var productItem = await unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
                if (item.Price != productItem.Price)
                {
                    item.Price = productItem.Price;
                }
            }
            StripeConfiguration.ApiKey = configuration["StripSettings:SecretKey"];

            var paymentIntentService = new PaymentIntentService();
            PaymentIntent paymentIntent = null;
            if (!string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                paymentIntent = await paymentIntentService.GetAsync(basket.PaymentIntentId);
            }

            // If the payment intent doesn't exist, create a new one
            if (paymentIntent == null)
            {
                var createOptions = new PaymentIntentCreateOptions
                {
                    Amount = (long)basket.ShipingPrice * 100 + (long)basket.BasketItems.Sum(item => (item.Price * 100) * item.Quantity),
                    Currency = "usd",
                    PaymentMethodTypes = new List<string> { "card" }
                };

                paymentIntent = await paymentIntentService.CreateAsync(createOptions);

                // Update the basket with the payment intent ID
                basket.PaymentIntentId = paymentIntent.Id;
                basket.ClientSecret= paymentIntent.ClientSecret;
            }
            else
            {
                var updateOptions = new PaymentIntentUpdateOptions
                {
                    Amount = (long)basket.ShipingPrice * 100 + (long)basket.BasketItems.Sum(item => (item.Price * 100) * item.Quantity)
                };
                var updatedIntent = await paymentIntentService.UpdateAsync(paymentIntent.Id, updateOptions);

                // Update the basket with the updated payment intent ID
                basket.PaymentIntentId = updatedIntent.Id;
            }
            await basketRepository.UpdateBasketAsync(basket);
            return basket;
        }

        public async Task<Order> UpdateOrderPaymentFailed(string paymentIntentId)
        {
            var spec = new OrderWithPaymentIntentSpecifications(paymentIntentId);
            var order = await unitOfWork.Repository<Order>().GetEntityWithSpecifications(spec);

            if (order == null)
            {
                return null;
            }

            order.OrderStatus = OrderStatus.PaymentFailed;
            unitOfWork.Repository<Order>().Update(order);

            await unitOfWork.Complete();

            return order;
        }

        public async Task<Order> UpdateOrderPaymentSucceeded(string paymentIntentId)
        {
            var spec = new OrderWithPaymentIntentSpecifications(paymentIntentId);
            var order = await unitOfWork.Repository<Order>().GetEntityWithSpecifications(spec);

            if (order == null)
            {
                return null;
            }

            order.OrderStatus = OrderStatus.PaymentReceived;
            unitOfWork.Repository<Order>().Update(order);

            await unitOfWork.Complete();

            return order;
        }
    }
}
