using Core.Entities;
using Core.Entities.orderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace infrastructure.Data
{
    public class StoreDbContextSeed
    {
        public static async Task SeedAsync(StoreDbContext dbContext, ILoggerFactory loggerFactory)
        {
			try
			{
                if(dbContext.Types!=null&& !dbContext.Types.Any())
				{
                  var typesData = File.ReadAllText("../infrastructure/Data/SeedData/types.json");
                  var types = JsonSerializer.Deserialize<List<ProductType>>(typesData);
                        dbContext.Types.AddRange(types);
                    await dbContext.SaveChangesAsync();
                }
                if (dbContext.Brands != null && !dbContext.Brands.Any())
                {
                    var brandsData = File.ReadAllText("../infrastructure/Data/SeedData/brands.json");
                    var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandsData);
                        dbContext.Brands.AddRange(brands);
                    await dbContext.SaveChangesAsync();
                }
                if (dbContext.Products != null && !dbContext.Products.Any())
                {
                    var productsData = File.ReadAllText("../infrastructure/Data/SeedData/products.json");
                    var products = JsonSerializer.Deserialize<List<Product>>(productsData);
                        dbContext.Products.AddRange(products);
                    await dbContext.SaveChangesAsync();
                }
                if (dbContext.DeliveryMethods != null && !dbContext.DeliveryMethods.Any())
                {
                    var deliveryData = File.ReadAllText("../infrastructure/Data/SeedData/delivery.json");
                    var delivery = JsonSerializer.Deserialize<List<DeliveryMethod>>(deliveryData);
                    dbContext.DeliveryMethods.AddRange(delivery);
                    await dbContext.SaveChangesAsync();
                }
            }
			catch (Exception ex)
			{
                var logger = loggerFactory.CreateLogger<StoreDbContextSeed>();
                logger.LogError(ex.Message);
			}
        }
    }
}
