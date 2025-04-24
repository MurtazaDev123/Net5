using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartEcommerce.Models.Web
{
    public class HomePage
    {
        public List<Sliders> Sliders { get; set; }
        public List<Product.Videos> RecentWatch { get; set; }
        public List<Product.Videos> NewArrivals { get; set; }
        public List<Product.Category> Categories { get; set; }

        public List<Product.LiveStreaming> LiveStreaming { get; set; }

        public HomePage()
        {
            Sliders = new List<Web.Sliders>();
            RecentWatch = new List<Product.Videos>();
            NewArrivals = new List<Product.Videos>();
            Categories = new List<Product.Category>();
            LiveStreaming = new List<Product.LiveStreaming>();
        }
    }

    public class Sliders
    {
        public string ImageURL { get; set; }
        public string RedirectionURL { get; set; }
    }
}
