using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartEcommerce.Models.Common
{
    public class DisputeVideo
    {
        public int DT_RowId { get; set; }
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public int VideoId { get; set; }
        public string VideoTitle { get; set; }
        public string VideoURL { get; set; }
        public string Description { get; set; }
        public int TopicId { get; set; }
        public string TopicName { get; set; }
        public int Type { get; set; }
        public DateTime CreatedOn { get; set; }
        
    }
}
