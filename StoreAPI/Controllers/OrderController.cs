using AutoMapper;
using Core.Entities.orderAggregate;
using Core.Interfaces;
using infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreAPI.Dtos;
using StoreAPI.Extensions;
using StoreAPI.ResponseModule;
using System.Collections.Generic;

namespace StoreAPI.Controllers
{
    [Authorize]
    public class OrderController : BaseController
    {
        private readonly IOrderService orderService;
        private readonly IMapper mapper;

        public OrderController(IOrderService orderService,IMapper mapper)
        {
            this.orderService = orderService;
            this.mapper = mapper;
        }
        [HttpPost("CreateOrder")]
        public async Task<ActionResult<Order>> CreateOrder(OrderDto orderDto)
        {
            var email = HttpContext.User.RetrieveEmailFromPrincipal();
            var address = mapper.Map<ShippingAddress>(orderDto.ShippingAddress);
            if (orderDto.DeliveryMethodId <= 0)
                return BadRequest(new ApiResponse(400, "Invalid delivery method ID"));
            if (address is null)
                return BadRequest(new ApiResponse(400, "Invalid shipping address"));
            var order = await orderService.CreateOrderAsync(email, orderDto.DeliveryMethodId, orderDto.BasketId, address);
            if (order is null)
                return BadRequest(new ApiResponse(400,"Problem When Creating Order"));
            return Ok(order);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDetailsDto>> GetOrderByIdForUser(int id)
        {
            var email = HttpContext.User.RetrieveEmailFromPrincipal();
            var order = await orderService.GetOrderByIdAsync(id, email);
            if (order is null)
                return NotFound(new ApiResponse(404, "Order not found"));
            return Ok(mapper.Map<OrderDetailsDto>(order));
        }
        [HttpGet("GetOrdersForUser")]
        public async Task<ActionResult<IReadOnlyList<OrderDetailsDto>>> GetOrdersForUser()
        {
            var email = HttpContext.User.RetrieveEmailFromPrincipal();
            var orders = await orderService.GetOrdersForUserAsync(email);
            return Ok(mapper.Map<IReadOnlyList<OrderDetailsDto>>(orders));
        }
        [HttpGet("GetDeliveryMethods")]
        public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
            => Ok(await orderService.GetDeliveryMethodAsync());

    }
}
