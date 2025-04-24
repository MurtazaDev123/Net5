using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartEcommerce.Models.Admin
{
    public class Other
    {
        public string ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public long Key1 { get; set; }
        public string Key2 { get; set; }
        public bool SubscriptionTrial { get; set; }
        public DateTime SubscriptionEnd { get; set; }

        public string ProfilePicture { get; set; }
    }
}
