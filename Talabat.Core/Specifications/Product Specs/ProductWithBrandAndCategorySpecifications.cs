using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications.Product_Specs
{
    public class ProductWithBrandAndCategorySpecifications : BaseSpecifications<Product>
    {
        // This Constructor will be Used fof Creating an Object, That will be Used to Get ALL Products
        public ProductWithBrandAndCategorySpecifications(ProductSpecParams specParams) : base(P =>(string.IsNullOrEmpty(specParams.Search)||(P.Name.ToLower().Contains(specParams.Search))) &&(!specParams.BrandId.HasValue || P.BrandId == specParams.BrandId.Value) && (!specParams.CategoryId.HasValue || P.CategoryId == specParams.CategoryId.Value))
        {
            AddInclodues();

            if (!string.IsNullOrEmpty(specParams.Sort))
            {
                switch (specParams.Sort)
                {
                    case "priceAsc":
                        //OrderBy = P => P.Price; 
                        AddOrderBy(P => P.Price);
                        break;
                    case "priceDesc":
                        //OrderByDesc= P => P.Price;
                        AddOrderByDesc(P => P.Price);
                        break;
                    default:
                        AddOrderBy(P => P.Name);
                        break;




                }
            }

            else
            {
                AddOrderBy(P => P.Name);
            }

            // totalProducts = 18 ~ 20 
            // pageSize =5
            // pageIndex =3 

            ApplyPagination((specParams.PageIndex - 1) * (specParams.PageSize), specParams.PageSize);
        }

        // This Constructor will be Used for Creating an Object, That will be Used to Get a Specific Product
        public ProductWithBrandAndCategorySpecifications(int id) : base(P => P.Id == id)
        {
            AddInclodues();

            

		}

        private void AddInclodues()
        {
			Includes.Add(P => P.Brand);
			Includes.Add(P => P.Category);
		}
    }
}

