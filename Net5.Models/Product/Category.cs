using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartEcommerce.Models.Product
{
    public class Category
    {
        public int DT_RowId { get; set; } // This field added for datatable row key
        public int Id { get; set; }
        public string Title { get; set; }
        public bool Featured { get; set; }
        public bool Active { get; set; }
        public string ImageURL { get; set; }
        public string CreatedBy { get; set; }
        public string ErrorCode { get; set; }
        public DateTime CreatedOn { get; set; }
        public List<Videos> Videos { get; set; }
        public string URL { get; set; }
        public int TotalRecords { get; set; }
        public int RemainingRecords { get; set; }
        public int StartingRecord { get; set; }
        public int EndingRecord { get; set; }
        public int Priority { get; set; }
        public string ContentTypeUploadId { get; set; }

        public Category()
        {
            Videos = new List<Models.Product.Videos>();
        }
    }
}
