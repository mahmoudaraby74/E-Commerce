using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreAPI.Dtos;

namespace StoreAPI.Controllers
{
    public class BasketController : BaseController
    {
        private readonly IBasketRepository basketRepository;
        private readonly IMapper mapper;

        public BasketController(IBasketRepository basketRepository,IMapper mapper)
        {
            this.basketRepository = basketRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<CustomerBasket>> GetBasketById (string id)
        {
            var basket =await basketRepository.GetBasketAsync(id);
            return Ok(basket?? new CustomerBasket(id));
        }
        [HttpPost]
        public async Task<ActionResult<CustomerBasket>> UpdateBasket(CustomerBasketDto customerBasketDto)
        {
            var mappedBasket = mapper.Map<CustomerBasket>(customerBasketDto);
            var Updatedbasket = await basketRepository.UpdateBasketAsync(mappedBasket);
            return Ok(Updatedbasket);
        }
        [HttpDelete]
        public async Task DeleteBasket(string id)
            => await basketRepository.DeleteBasketAsync(id);
    }
}
