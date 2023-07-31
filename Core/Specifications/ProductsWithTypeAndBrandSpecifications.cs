using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications
{
    public class ProductsWithTypeAndBrandSpecifications : BaseSpecification<Product>
    {
        public ProductsWithTypeAndBrandSpecifications(ProductSpecPrams productSpecPrams)
            : base(product =>
            (string.IsNullOrEmpty(productSpecPrams.Search) || product.Name.ToLower().Contains(productSpecPrams.Search)) &&
            (!productSpecPrams.BrandId.HasValue|| product.ProductBrandId == productSpecPrams.BrandId) &&
            (!productSpecPrams.TypeId.HasValue || product.ProductTypeId == productSpecPrams.TypeId))
        {
            AddInclude(product => product.ProductType);
            AddInclude(product => product.ProductBrand);
            AddOrderBy(product => product.Name);
            ApplyPaging(productSpecPrams.PageSize*(productSpecPrams.PageIndex-1),productSpecPrams.PageSize);

            if (!string.IsNullOrEmpty(productSpecPrams.Sort))
            {
                switch (productSpecPrams.Sort)
                {
                    case "priceAsc":
                        AddOrderBy(product => product.Price);
                        break;
                    case "priceDesc":
                        AddOrderByDesc(product => product.Price);
                        break;
                    default: AddOrderBy(product => product.Name);
                        break;
                }
            }
        }
        public ProductsWithTypeAndBrandSpecifications(int id):base(prodcut => prodcut.Id == id)
        {
            AddInclude(product => product.ProductType);
            AddInclude(product => product.ProductBrand);
        }
    }
}
