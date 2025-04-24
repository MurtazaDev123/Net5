using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartEcommerce.Models.Common
{
    public class Payments
    {
        public int DT_RowId { get; set; }
        public int Id { get; set; }
        public string PaymentId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UserId { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }

    }
}
