using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartEcommerce.Models.Product
{
    public class Basket
    {
        public int TotalQty { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TaxPercentage { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal Discount { get; set; }
        public decimal NetAmount { get; set; }
        public int CityId { get; set; }
        public string ShippingRegion { get; set; }
        public decimal ShippingCharges { get; set; }
        public List<BasketProducts> Products { get; set; }

        public Basket()
        {
            Products = new List<BasketProducts>();
        }
    }

    public class BasketProducts
    {
        public long Id { get; set; }
        public int SrNo { get; set; }
        public int Qty { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal DiscountedPrice { get; set; }
        public decimal Amount { get; set; }
        public List<BasketProductAttributes> Attributes { get; set; }

        public BasketProducts()
        {
            Attributes = new List<BasketProductAttributes>();
        }
    }

    public class BasketProductAttributes
    {
        public long AttributeId { get; set; }
        public string AttributeDetailId { get; set; }
        public string AttributeValue { get; set; }
        public string AttributeDetailValue { get; set; }
    }
}
