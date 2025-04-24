using SmartEcommerce.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartEcommerce.Models.Store
{
    public class Mall
    {
        public int DT_RowId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public City City { get; set; }
        public string Description { get; set; }
        public string LogoImage { get; set; }
        public string CoverImage { get; set; }
        public string MapURL { get; set; }
        public bool Active { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int TotalStores { get; set; }
        public int TotalCategories { get; set; }
    }
}
