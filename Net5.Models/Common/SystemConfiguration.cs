using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartEcommerce.Models.Common
{
    public class SystemConfiguration
    {
        public string ParameterName { get; set; }
        public string ParameterValue { get; set; }
    }

    public class Settings
    {
        public string MonthlyRate { get; set; }
        public string YearlyRate { get; set; }
        public string IsTrial { get; set; }
        public string TrialDays { get; set; }
    }
}
