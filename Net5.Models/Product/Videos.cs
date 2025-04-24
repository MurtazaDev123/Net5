using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartEcommerce.Models.Product
{
    public class Videos
    {
        public int DT_RowId { get; set; } // This field added for datatable row key
        public int Id { get; set; }
        public string Title { get; set; }
        public Program Program { get; set; }
        public Category Category { get; set; }
        public string ImageURL { get; set; }
        public string VideoURL { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public List<Videos> RelatedVideos { get; set; }
        public List<Videos> ProgramCategoryVideos { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public int Views { get; set; }
        public int IsList { get; set; }
        public int AddedBy { get; set; }
        public int TotalRecords { get; set; }
        public int RemainingRecords { get; set; }
        public int StartingRecord { get; set; }
        public int EndingRecord { get; set; }
        public string URL { get; set; }
        public string FormatNumber { get; set; }
        public string TimeAgo { get; set; }
        public string ErrorCode { get; set; }
        public string ProfileImageURL { get; set; }
        public string ProfileURL { get; set; }
        public int SubscriberId { get; set; }
        public int IsSubscribed { get; set; }
        public long TotalSubscribers { get; set; }
        public bool Notification { get; set; }
        public bool IsLike { get; set; }
        public bool IsDislike { get; set; }
        public List<Videos> VideoLibrary { get; set; }

        public Videos()
        {
            VideoLibrary = new List<Videos>();
            Program = new Models.Product.Program();
            Category = new Models.Product.Category();
            RelatedVideos = new List<Models.Product.Videos>();
            ProgramCategoryVideos = new List<Models.Product.Videos>();
        }
    }
}
