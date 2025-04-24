using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartEcommerce.Models.Product
{
    public class VideoSearch
    {
        public int TotalRecords { get; set; }

        public List<VideoSearchVideos> Videos { get; set; }
        public int RemainingRecords { get; set; }
        public int StartingRecord { get; set; }
        public int EndingRecord { get; set; }
        public VideoSearch()
        {
            Videos = new List<VideoSearchVideos>();
        }

    }

    public class VideoSearchVideos
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string UserName { get; set; }
        public int Views { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public DateTime AddedOn { get; set; }
        public string ImageURL { get; set; }
        public int IsList { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string URL { get; set; }
        public string FormatNumber { get; set; }
        public string TimeAgo { get; set; }
        public string ProfileURL { get; set; }
    }
}
