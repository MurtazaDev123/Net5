using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartEcommerce.Models.Product
{
    public class Program
    {
        public int DT_RowId { get; set; } // This field added for datatable row key
        public int Id { get; set; }
        public string Title { get; set; }
        public string ImageURL { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public bool Featured { get; set; }
        public string ErrorCode { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public List<Videos> Episodes { get; set; }

        public List<Program> FeaturedPrograms { get; set; }
        public List<Program> AllPrograms { get; set; }
        public List<Videos> TopEpisodes { get; set; }
        public string URL { get; set; }
        public int AddedBy { get; set; }

        public Program()
        {
            Episodes = new List<Models.Product.Videos>();
            FeaturedPrograms = new List<Models.Product.Program>();
            AllPrograms = new List<Models.Product.Program>();
            TopEpisodes = new List<Models.Product.Videos>();
        }
    }
}
