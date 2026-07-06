using E_Commerce.Application.Common;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Specifications
{
    internal class ProductWithTypeAndBrandSpec : BaseSpecification<Product, int>
    {
        // Get All
        public ProductWithTypeAndBrandSpec() : base(null)
        {
            AddInclude(p=>p.ProductType);
            AddInclude(p => p.ProductBrand);
        }
        // Get by ID
        public ProductWithTypeAndBrandSpec(int id) : base(x=>x.Id == id)
        {
            AddInclude(p => p.ProductType);
            AddInclude(p => p.ProductBrand);
        }
        // Get All by brand id or type id
        public ProductWithTypeAndBrandSpec(ProductQueryParams queryParams)
            : base(P => (!queryParams.BrandId.HasValue || P.BrandId == queryParams.BrandId.Value) 
            && (!queryParams.TypeId.HasValue || P.TypeId == queryParams.TypeId.Value)
            && (string.IsNullOrWhiteSpace(queryParams.SearchValue) || P.Name.ToLower().Contains(queryParams.SearchValue.ToLower())))
        {
            // brandId is not null > P => P.BrandId == brandId
            // typeId is not null > P => P.TypeId == typeId
            // brandId and typeId are not null > P => P.BrandId == brandId && P.TypeId == typeId
            AddInclude(p => p.ProductType);
            AddInclude(p => p.ProductBrand);

            switch(queryParams.Sort)
            {
                case ProductSortingOptions.NameAsc:
                    AddOrderBy(p => p.Name);
                    break;
                case ProductSortingOptions.NameDesc:
                    AddOrderByDescending(p => p.Name);
                    break;
                case ProductSortingOptions.PriceAsc:
                    AddOrderBy(p => p.Price);
                    break;
                case ProductSortingOptions.PriceDesc:
                    AddOrderByDescending(p => p.Price);
                    break;
                default:
                    AddOrderBy(p => p.Id);
                    break;
            }
            ApplyPagination(queryParams.PageIndex, queryParams.PageSize);
        }
    }
}
