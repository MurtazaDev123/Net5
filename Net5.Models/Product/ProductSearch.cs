using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartEcommerce.Models.Product
{
    public class ProductSearch
    {
        public int TotalRecords { get; set; }
        public List<ProductSearchCategories> Categories { get; set; }
        public List<decimal> Ratings { get; set; }
        public List<ProductSearchBrands> Brands { get; set; }
        public List<ProductSearchProducts> Products { get; set; }
        public int RemainingRecords { get; set; }
        public int StartingRecord { get; set; }
        public int EndingRecord { get; set; }

        public ProductSearch()
        {
            Categories = new List<ProductSearchCategories>();
            Ratings = new List<decimal>();
            Brands = new List<ProductSearchBrands>();
            Products = new List<ProductSearchProducts>();
        }
    }

    public class ProductSearchCategories
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public int Level { get; set; }
        public List<ProductSearchCategories> Child { get; set; }
    }

    public class ProductSearchBrands
    {
        public int BrandId { get; set; }
        public string BrandName { get; set; }
    }

    public class ProductSearchProducts
    {
        public long Id { get; set; }
        public string FullName { get; set; }
        public string PrimaryImage { get; set; }
        public decimal UserRating { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountedPrice { get; set; }
        public decimal DiscountPercentage { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public string ProductURL { get; set; }
    }
}
