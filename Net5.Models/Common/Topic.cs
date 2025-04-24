using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartEcommerce.Models.Common
{
    public class Topic
    {
        public int DT_RowId { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
        public bool Active { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
