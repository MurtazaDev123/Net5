using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartEcommerce.Models.Common
{
    public class Sliders
    {
        public int DT_RowId { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
        public string PageName { get; set; }
        public string ImageURL { get; set; }
        public string RedirectionURL { get; set; }
        public int Sequence { get; set; }
        public bool Active { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
