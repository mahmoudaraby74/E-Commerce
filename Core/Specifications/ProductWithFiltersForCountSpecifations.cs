using Core.Entities;
using System.Linq.Expressions;

namespace Core.Specifications
{
    public class ProductWithFiltersForCountSpecifations : BaseSpecification<Product>
    {
        public ProductWithFiltersForCountSpecifations(ProductSpecPrams productSpecPrams) : base(product =>
        (string.IsNullOrEmpty(productSpecPrams.Search) || product.Name.ToLower().Contains(productSpecPrams.Search)) &&
        (!productSpecPrams.BrandId.HasValue || product.ProductBrandId == productSpecPrams.BrandId) &&
        (!productSpecPrams.TypeId.HasValue || product.ProductTypeId == productSpecPrams.TypeId))
        {
        }
    }
}
