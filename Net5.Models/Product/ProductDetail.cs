using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartEcommerce.Models.Product
{
    public class ProductDetail
    {
        public long Id { get; set; }
        public Store.Store Store { get; set; }
        public Brand Brand { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountedPrice { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal Rating { get; set; }
        public int TotalReviews { get; set; }
        public string Categories { get; set; }
        public bool StockAvailable { get; set; }
        public string PrimaryImageURL { get; set; }
        public List<ProductDetailImages> Images { get; set; }
        public List<ProductDetailAttributes> Attributes { get; set; }
        public List<ProductDetailAttributeMaster> AttributeMaster { get; set; }
        public List<ProductDetailSpecification> Specifications { get; set; }
        public List<ProductSearchProducts> SimilarProducts { get; set; }

        public ProductDetail()
        {
            Store = new Models.Store.Store();
            Brand = new SmartEcommerce.Models.Product.Brand();
            Images = new List<SmartEcommerce.Models.Product.ProductDetailImages>();
            Attributes = new List<SmartEcommerce.Models.Product.ProductDetailAttributes>();
            AttributeMaster = new List<SmartEcommerce.Models.Product.ProductDetailAttributeMaster>();
            Specifications = new List<SmartEcommerce.Models.Product.ProductDetailSpecification>();
            SimilarProducts = new List<SmartEcommerce.Models.Product.ProductSearchProducts>();
        }
    }

    public class ProductDetailImages
    {
        public string SmallImage { get; set; }
        public string LargeImage { get; set; }
    }

    public class ProductDetailAttributes
    {
        public int AttributeId { get; set; }
        public string AttributeName { get; set; }
        public bool HasColor { get; set; }
        public string AttributeDetailId { get; set; }
        public string AttributeDetailValue { get; set; }
        public string ColorCode { get; set; }
    }

    public class ProductDetailAttributeMaster
    {
        public string Description { get; set; }
        public string Attributes { get; set; }
        public decimal Price { get; set; }
    }

    public class ProductDetailSpecification
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
