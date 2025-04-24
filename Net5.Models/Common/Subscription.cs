using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartEcommerce.Models.Common
{
    public class Subscription
    {
        public int DT_RowId { get; set; }
        public int Id { get; set; }
        public string SubType { get; set; }
        public string SubPlan { get; set; }
        public string SubStatus { get; set; }
        public string SubStartDate { get; set; }
        public string SubEndDate { get; set; }
        public decimal SubAmount { get; set; }
        public decimal YearlyRate { get; set; }
        public decimal MonthlyRate { get; set; }
        public bool Active { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public Country Country { get; set; }
    }
}
