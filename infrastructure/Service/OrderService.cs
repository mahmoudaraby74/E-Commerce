using Core.Entities;
using Core.Entities.orderAggregate;
using Core.Interfaces;
using Core.Specifications;
using infrastructure.Data;
using Stripe;
using Product = Core.Entities.Product;

namespace infrastructure.Service
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IBasketRepository basketRepository;
        private readonly IPaymentService paymentService;

        public OrderService(IUnitOfWork unitOfWork,IBasketRepository basketRepository,IPaymentService paymentService)
        {
            this.unitOfWork = unitOfWork;
            this.basketRepository = basketRepository;
            this.paymentService = paymentService;
        }
        public async Task<Order> CreateOrderAsync(string buyerEmail, int deliveryMethodId, string basketId, ShippingAddress shippingAddress)
        {
            var basket = await basketRepository.GetBasketAsync(basketId);
            var items = new List<OrderItem>();
            foreach (var item in basket.BasketItems)
            {
                var productItem = await unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
                var itemOrdered = new ProductItemOdered(productItem.Id, productItem.Name, productItem.PictureUrl);
                var orderItem = new OrderItem( itemOrdered, productItem.Price, item.Quantity);
                items.Add(orderItem);
            }
            
            var deliveryMethod = await unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryMethodId);
            var subTotal = items.Sum(item => item.Price * item.Quantity);

            var spec = new OrderWithPaymentIntentSpecifications(basket.PaymentIntentId);
            var existingOrder = await unitOfWork.Repository<Order>().GetEntityWithSpecifications(spec);
            if  (existingOrder != null)
            {
                unitOfWork.Repository<Order>().Delete(existingOrder);
                await paymentService.CreateOrUpdatePaymentIntent(basketId);
            }

            var order = new Order(buyerEmail, shippingAddress, deliveryMethod, items, subTotal,basket.PaymentIntentId);

            unitOfWork.Repository<Order>().Add(order);
            var result = await unitOfWork.Complete();

            if (result <= 0) return null;


            return order;
        }


        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodAsync()
          => await unitOfWork.Repository<DeliveryMethod>().ListAllAsync();

        public async Task<Order> GetOrderByIdAsync(int id, string buyerEmail)
        {
            var spec = new OrdersWithItemsSpecification(id, buyerEmail);
            return await unitOfWork.Repository<Order>().GetEntityWithSpecifications(spec);
        }

        public async Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail)
        {
            var spec = new OrdersWithItemsSpecification(buyerEmail);
            return await unitOfWork.Repository<Order>().ListAsync(spec);
        }
    }
}
