using Core.Entities;
using Core.Entities.orderAggregate;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreAPI.ResponseModule;
using Stripe;

namespace StoreAPI.Controllers
{
    public class PaymentsController : BaseController
    {
        private readonly IPaymentService paymentService;
        private readonly ILogger<PaymentsController> logger;
        private readonly string WhSecret = "whsec_d9de67d530bbbe119fd557c4171bc06324ab28bef9c1ed6e8f70c9f9a1dd5bdd";

        public PaymentsController(IPaymentService paymentService,ILogger<PaymentsController> logger)
        {
            this.paymentService = paymentService;
            this.logger = logger;
        }
        [HttpPost("{basketId}")]
        public async Task<ActionResult<CustomerBasket>> CreateOrUpdatePaymentIntent(string basketId)
        {
            var basket =await paymentService.CreateOrUpdatePaymentIntent(basketId);
            if (basket == null)
            {
                return BadRequest(new ApiResponse(400,"Problem With Your Basket"));
            }
            return Ok(basket);
        }
        [HttpPost("WebHook")]
        public async Task<IActionResult> StripWebHook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json,
                    Request.Headers["Stripe-Signature"], WhSecret);
                PaymentIntent paymentIntent;
                Order order;
                switch (stripeEvent.Type)
                {
                    case Events.PaymentIntentPaymentFailed:
                        paymentIntent = (PaymentIntent)stripeEvent.Data.Object;
                        logger.LogInformation("Payment Failed : ", paymentIntent.Id);
                        order = await paymentService.UpdateOrderPaymentFailed(paymentIntent.Id);
                        logger.LogInformation("Payment Failed : ", order.Id);
                        break;

                    case Events.PaymentIntentSucceeded:
                        paymentIntent = (PaymentIntent)stripeEvent.Data.Object;
                        logger.LogInformation("Payment Succeeded : ", paymentIntent.Id);
                        order = await paymentService.UpdateOrderPaymentSucceeded(paymentIntent.Id);
                        logger.LogInformation("order updated to payment recived : ", order.Id);
                        break;
                }

                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest();
            }
        }
    }
}
