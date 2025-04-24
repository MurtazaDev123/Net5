using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Data.SqlTypes;
using System.Xml;
using System.Data;
using System.Data.SqlClient;

namespace SmartEcommerce.Models.Common
{
    public class Partner
    {
        public int DT_RowId { get; set; }
        public int Id { get; set; }
        public string FullName { get; set; }
        public PartnerType PartnerType { get; set; }
        public PartnerCategory PartnerCategory { get; set; }
        public string ContactPerson { get; set; }
        public string Telephone { get; set; }
        public string MobileNo { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public Country Country { get; set; }
        public State State { get; set; }
        public City City { get; set; }
        public string Address { get; set; }
        public PartnerContentType PartnerContentType { get; set; }
        //public Product.Category PartnerContentTypeUpload { get; set; }

        public List<Product.Category> PartnerContentTypeUpload { get; set; }
        public string PartnerContentTypeUploadName { get; set; }
        public int LoginType { get; set; }
        public bool Active { get; set; }
        public bool Approval { get; set; }
        public bool IsPassChange { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ProfilePicture { get; set; }
        public int Monetization { get; set; }
        public Partner()
        {
            PartnerType = new Common.PartnerType();
            PartnerCategory = new Common.PartnerCategory();
            Country = new Common.Country();
            State = new Common.State();
            City = new Common.City();
            PartnerContentType = new Common.PartnerContentType();
            //PartnerContentTypeUpload = new Product.Category();
            PartnerContentTypeUpload = new List<Models.Product.Category>();
        }
    }

}
