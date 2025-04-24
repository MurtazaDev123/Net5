using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartEcommerce.Models.Common
{
    public class User
    {
        public int DT_RowId { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
        public int LoginId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int LoginType { get; set; }
        public bool Active { get; set; }
        public bool Approval { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastLogin { get; set; }

        public string PhoneNo { get; set; }
        public string Gender { get; set; }
        public string DateOfBirth { get; set; }
        public int CountryId { get; set; }
        public int StateId { get; set; }
        public int CityId { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public string ProfilePicture { get; set; }
        public string Address { get; set; }
        public string SubscriptionType { get; set; }
        public string SubscriptionStartDate { get; set; }
        public string SubscriptionEndDate { get; set; }
        public string SubscriptionStatus { get; set; }
    }
}
