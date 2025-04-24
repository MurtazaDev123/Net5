using SmartEcommerce.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartEcommerce.Models.Store
{
    public class Store
    {
        public int DT_RowId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public City City { get; set; }
        public Mall Mall { get; set; }
        public string Description { get; set; }
        public string CoverImage { get; set; }
        public string LogoImage { get; set; }
        public string Address { get; set; }
        public string ContactNo { get; set; }
        public string Email { get; set; }
        public bool Active { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
