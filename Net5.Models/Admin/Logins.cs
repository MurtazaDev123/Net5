using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartEcommerce.Models.Admin
{
    public class Logins
    {
        public Store.Store Store { get; set; }
        public long LoginId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int LoginType { get; set; }
        public string ProfilePicture { get; set; }
        public string SubscriptionStatus { get; set; }
        public string SubscriptionType { get; set; }
        public int PartnerCategoryId { get; set; }
    }
}
