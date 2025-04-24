using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartEcommerce.Models
{
    public class AccountUser
    {
        public Account Account { get; set; }
        public int LoginId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int LoginType { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
