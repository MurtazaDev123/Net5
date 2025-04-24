using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartEcommerce.Models.Product
{
    public class Brand
    {
        public int DT_RowId { get; set; } // This field added for datatable row key
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string LogoImage { get; set; }
        public bool Active { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
