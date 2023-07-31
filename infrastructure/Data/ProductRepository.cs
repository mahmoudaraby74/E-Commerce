using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Data
{
    public class ProductRepository : IProductRepository
    {
        private readonly StoreDbContext context;

        public ProductRepository(StoreDbContext context)
        {
            this.context = context;
        }
        public async Task<IReadOnlyList<ProductBrand>> GetProductBrandsAsync()
        => await context.Brands.ToListAsync();
        public async Task<Product> GetProductByIdAsync(int id)
        => await context.Products.Include(p=> p.ProductType).Include(p => p.ProductBrand).FirstOrDefaultAsync(p => p.Id==id);

        public async Task<IReadOnlyList<Product>> GetProductsAsync()
        => await context.Products.Include(p => p.ProductType).Include(p => p.ProductBrand).ToListAsync();
        public async Task<IReadOnlyList<ProductType>> GetProductTypesAsync()
        => await context.Types.ToListAsync();
    }
}
