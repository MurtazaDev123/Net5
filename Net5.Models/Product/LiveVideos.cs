using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartEcommerce.Models.Product
{
    public class LiveVideos
    {
        public int DT_RowId { get; set; } // This field added for datatable row key
        public int Id { get; set; }
        public string Title { get; set; }
        public string VideoURL { get; set; }
        public string ImageURL { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public List<LiveVideos> RelatedLiveVideos { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public int Views { get; set; }
        public string URL { get; set; }
        public string ProfileImageURL { get; set; }
        public string ProfileURL { get; set; }
        public int SubscriberId { get; set; }
        public int IsSubscribed { get; set; }
        public long TotalSubscribers { get; set; }
        public bool Notification { get; set; }
        public int AddedBy { get; set; }
        public bool IsLike { get; set; }
        public bool IsDislike { get; set; }

        public DateTime EventDate { get; set; }
        public DateTime FromTime { get; set; }
        public DateTime ToTime { get; set; }

        public string StringFromTime { get; set; }
        public string StringToTime { get; set; }

        public LiveVideos()
        {
            RelatedLiveVideos = new List<Models.Product.LiveVideos>();
        }
    }
}
