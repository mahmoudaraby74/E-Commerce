    using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreAPI.Dtos;
using StoreAPI.Helpers;
using StoreAPI.ResponseModule;

namespace StoreAPI.Controllers
{
    public class ProductsController : BaseController
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        //private readonly IProductRepository productRepository;

        public ProductsController(IUnitOfWork unitOfWork,IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        [HttpGet("GetProducts")]
        [Cached(100)]
        public async Task<ActionResult<Pagination<ProductDto>>> GetProducts([FromQuery]ProductSpecPrams productSpecPrams)
        {
            var specs = new ProductsWithTypeAndBrandSpecifications(productSpecPrams);
            var countSpecs = new ProductWithFiltersForCountSpecifations(productSpecPrams);
            var totalItems = await unitOfWork.Repository<Product>().CountAsync(specs);
            var products = await unitOfWork.Repository<Product>().ListAsync(specs);
            var mappedProducts =mapper.Map<IReadOnlyList<ProductDto>>(products);
            var data = new Pagination<ProductDto>(productSpecPrams.PageIndex, productSpecPrams.PageSize, totalItems, mappedProducts);
            return Ok(data);
        }
        [HttpGet("GetProductById")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [Cached(100)]
        public async Task<ActionResult<ProductDto>> GetProductById(int id)
        {
            var specs = new ProductsWithTypeAndBrandSpecifications(id);
            var product = await unitOfWork.Repository<Product>().GetEntityWithSpecifications(specs);

            if (product == null)
            {
                return NotFound(new ApiResponse(404));
            }
            var mappedProduct = mapper.Map<ProductDto>(product);
            return Ok(mappedProduct);
        }
        [HttpGet("GetProductBrands")]
        [Cached(100)]
        public async Task<IReadOnlyList<ProductBrand>> GetProductBrands()
            =>await unitOfWork.Repository<ProductBrand>().ListAllAsync();
        [HttpGet("GetProductTypes")]
        [Cached(100)]
        public async Task<IReadOnlyList<ProductType>> GetProductTypes()
            =>await unitOfWork.Repository<ProductType>().ListAllAsync();
        //[HttpGet("GetProducts")]
        //public async Task<IReadOnlyList<Product>> GetProducts()
        //    => await productRepository.GetAllIncluding(pro => pro.ProductType, pro => pro.ProductBrand);

    }
}
