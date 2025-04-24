using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using SmartEcommerce.Models.Product;
using System.Data.SqlTypes;
using System.Xml;
using System.Configuration;

namespace BusinessLogic
{
    public class Products
    {
        #region Category

        public DataTable GetAllCategories(int Status = BusinessLogic.Status.Current)
        {
            try
            {
                SqlParameter[] param = {

                    new SqlParameter { ParameterName = "@Status", Value = Status }
                };
                ErrorResponse response = new ErrorResponse();
                DataTable dt = DatabaseObject.FetchTableFromSP("CategoryGetAll", param, ref response);

                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return null;
                }

                return dt;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public List<SmartEcommerce.Models.Product.Category> GetCategoryByStatus(int Status)
        {
            try
            {
                List<SmartEcommerce.Models.Product.Category> categories = new List<SmartEcommerce.Models.Product.Category>();
                DataTable dt = GetAllCategories(Status);

                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        categories.Add(new SmartEcommerce.Models.Product.Category()
                        {
                            DT_RowId = DataHelper.intParse(row["Id"]),
                            Id = DataHelper.intParse(row["Id"]),
                            Title = row["Title"].ToString(),
                            ImageURL = DataHelper.getCategoryImageURL(row["ImageURL"].ToString()),
                            Featured = DataHelper.boolParse(row["IsFeatured"]),
                            Active = DataHelper.boolParse(row["IsActive"]),
                            CreatedBy = row["LastModifiedBy"].ToString(),
                            CreatedOn = DataHelper.dateParse(row["LastModifiedOn"]),
                            Priority = DataHelper.intParse(row["Priority"]),
                            URL =  "/category/" + DataHelper.FriendlyUrlParse(row["Title"].ToString())
                        });
                    }
                }

                return categories;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public SmartEcommerce.Models.Product.Category GetCategoryById(int Id)
        {
            SmartEcommerce.Models.Product.Category category = new SmartEcommerce.Models.Product.Category() { Id = Id };

            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@Id", Value = Id }
                };

                DataTable dt = DatabaseObject.FetchTableFromSP("CategoryGetById", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                }

                if (DataHelper.HasRows(dt))
                {
                    DataRow row = dt.Rows[0];
                    
                    category.Title = row["Title"].ToString();
                    category.Featured = DataHelper.boolParse(row["IsFeatured"]);
                    category.ImageURL = row["ImageURL"].ToString();
                    category.Active = DataHelper.boolParse(row["IsActive"]);
                    category.Priority = DataHelper.intParse(row["Priority"]);
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
            }

            return category;
        }

        public bool SaveCategory(int LoginId, int Id, string Title, bool Featured, bool Active, string ImageURL, int Priority, out int entryLevel)
        {
            entryLevel = 0;
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {

                    new SqlParameter { ParameterName = "@Id", Value = Id },
                    new SqlParameter { ParameterName = "@Title", Value = Title },
                    new SqlParameter { ParameterName = "@Priority", Value = Priority },
                    new SqlParameter { ParameterName = "@IsFeatured", Value = Featured },
                    new SqlParameter { ParameterName = "@IsActive", Value = Active },
                    new SqlParameter { ParameterName = "@ImageURL", Value = ImageURL },
                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId }
                };

                DataTable dt = DatabaseObject.FetchTableFromSP("CategorySave", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return false;
                }

                entryLevel = DataHelper.intParse(dt.Rows[0]["EntryLevel"]);

                return true;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return false;
            }
        }

        public bool ArchiveCategory(int LoginId, int CategoryId, int Status)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {

                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId },
                    new SqlParameter { ParameterName = "@CategoryId", Value = CategoryId },
                    new SqlParameter { ParameterName = "@Status", Value = Status }
                };

                DatabaseObject.ExecuteSP("CategoriesMoveToArchive", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return false;
                }

                return true;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return false;
            }
        }

        #endregion Category

        #region Live Streaming

        public DataTable GetAllLiveStreaming(int Status = BusinessLogic.Status.Current)
        {
            try
            {
                SqlParameter[] param = {
                    new SqlParameter { ParameterName = "@Status", Value = Status },
                    new SqlParameter { ParameterName = "@LoginType", Value = clsSession.LoginType },
                    new SqlParameter { ParameterName = "@LoginId", Value = clsSession.LoginId }
                };
                ErrorResponse response = new ErrorResponse();
                DataTable dt = DatabaseObject.FetchTableFromSP("LiveStreamingGetAll", param, ref response);

                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return null;
                }

                return dt;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public DataTable GetAllLiveVideos(int Status = BusinessLogic.Status.Current)
        {
            try
            {
                SqlParameter[] param = {
                    new SqlParameter { ParameterName = "@Status", Value = Status },
                    new SqlParameter { ParameterName = "@LoginType", Value = clsSession.LoginType },
                    new SqlParameter { ParameterName = "@LoginId", Value = clsSession.LoginId }
                };
                ErrorResponse response = new ErrorResponse();
                DataTable dt = DatabaseObject.FetchTableFromSP("LiveVideosGetAll", param, ref response);

                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return null;
                }

                return dt;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public List<SmartEcommerce.Models.Product.LiveStreaming> GetLiveStreamingByStatus(int Status)
        {
            try
            {
                List<SmartEcommerce.Models.Product.LiveStreaming> streaming = new List<SmartEcommerce.Models.Product.LiveStreaming>();
                DataTable dt = GetAllLiveStreaming(Status);

                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        streaming.Add(new SmartEcommerce.Models.Product.LiveStreaming()
                        {
                            DT_RowId = DataHelper.intParse(row["Id"]),
                            Id = DataHelper.intParse(row["Id"]),
                            Title = row["Title"].ToString(),
                            Description = row["Description"].ToString(),
                            VideoURL = row["VideoURL"].ToString(),
                            ImageURL = DataHelper.getLiveStreamingImageURL(row["ImageURL"].ToString()),
                            Active = DataHelper.boolParse(row["IsActive"]),
                            CreatedBy = row["LastModifiedBy"].ToString(),
                            CreatedOn = DataHelper.dateParse(row["AddedOn"]),
                            Likes = DataHelper.intParse(row["Likes"]),
                            Dislikes = DataHelper.intParse(row["Dislikes"]),
                            Views = DataHelper.intParse(row["Views"]),
                            URL = "/live-tv/" + DataHelper.FriendlyUrlParse(row["Title"].ToString())
                        });
                    }
                }

                return streaming;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public List<SmartEcommerce.Models.Product.LiveVideos> GetLiveVideosByStatus(int Status)
        {
            try
            {
                List<SmartEcommerce.Models.Product.LiveVideos> streaming = new List<SmartEcommerce.Models.Product.LiveVideos>();
                DataTable dt = GetAllLiveVideos(Status);

                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        streaming.Add(new SmartEcommerce.Models.Product.LiveVideos()
                        {
                            DT_RowId = DataHelper.intParse(row["Id"]),
                            Id = DataHelper.intParse(row["Id"]),
                            Title = row["Title"].ToString(),
                            Description = row["Description"].ToString(),
                            VideoURL = row["VideoURL"].ToString(),
                            ImageURL = DataHelper.getLiveStreamingImageURL(row["ImageURL"].ToString()),
                            Active = DataHelper.boolParse(row["IsActive"]),
                            CreatedBy = row["LastModifiedBy"].ToString(),
                            CreatedOn = DataHelper.dateParse(row["AddedOn"]),
                            Likes = DataHelper.intParse(row["Likes"]),
                            Dislikes = DataHelper.intParse(row["Dislikes"]),
                            Views = DataHelper.intParse(row["Views"]),
                            URL = "/live-tv/" + DataHelper.FriendlyUrlParse(row["Title"].ToString())
                        });
                    }
                }

                return streaming;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public List<SmartEcommerce.Models.Product.LiveStreaming> GetLiveStreamingWeb(int Status)
        {
            try
            {
                List<SmartEcommerce.Models.Product.LiveStreaming> streaming = new List<SmartEcommerce.Models.Product.LiveStreaming>();

                SqlParameter[] param = {
                    new SqlParameter { ParameterName = "@Status", Value = Status }
                };
                ErrorResponse response = new ErrorResponse();
                DataTable dt = DatabaseObject.FetchTableFromSP("LiveStreamingGetAllWeb", param, ref response);

                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        streaming.Add(new SmartEcommerce.Models.Product.LiveStreaming()
                        {
                            DT_RowId = DataHelper.intParse(row["Id"]),
                            Id = DataHelper.intParse(row["Id"]),
                            Title = row["Title"].ToString(),
                            Description = row["Description"].ToString(),
                            VideoURL = row["VideoURL"].ToString(),
                            ImageURL = DataHelper.getLiveStreamingImageURL(row["ImageURL"].ToString()),
                            Active = DataHelper.boolParse(row["IsActive"]),
                            CreatedBy = row["LastModifiedBy"].ToString(),
                            CreatedOn = DataHelper.dateParse(row["AddedOn"]),
                            Likes = DataHelper.intParse(row["Likes"]),
                            Dislikes = DataHelper.intParse(row["Dislikes"]),
                            Views = DataHelper.intParse(row["Views"]),
                            URL = "/live-tv/" + DataHelper.FriendlyUrlParse(row["Title"].ToString())
                        });
                    }
                }

                return streaming;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public List<SmartEcommerce.Models.Product.LiveVideos> GetLiveVideosWeb(int Status)
        {
            try
            {
                List<SmartEcommerce.Models.Product.LiveVideos> livevideos = new List<SmartEcommerce.Models.Product.LiveVideos>();

                SqlParameter[] param = {
                    new SqlParameter { ParameterName = "@Status", Value = Status }
                };
                ErrorResponse response = new ErrorResponse();
                DataTable dt = DatabaseObject.FetchTableFromSP("LiveVideosGetAllWeb", param, ref response);

                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        livevideos.Add(new SmartEcommerce.Models.Product.LiveVideos()
                        {
                            DT_RowId = DataHelper.intParse(row["Id"]),
                            Id = DataHelper.intParse(row["Id"]),
                            Title = row["Title"].ToString(),
                            Description = row["Description"].ToString(),
                            VideoURL = row["VideoURL"].ToString(),
                            ImageURL = DataHelper.getLiveVideosImageURL(row["ImageURL"].ToString()),
                            Active = DataHelper.boolParse(row["IsActive"]),
                            CreatedBy = row["LastModifiedBy"].ToString(),
                            CreatedOn = DataHelper.dateParse(row["AddedOn"]),
                            Likes = DataHelper.intParse(row["Likes"]),
                            Dislikes = DataHelper.intParse(row["Dislikes"]),
                            Views = DataHelper.intParse(row["Views"]),
                            URL = "/live-videos/" + DataHelper.FriendlyUrlParse(row["Title"].ToString())
                        });
                    }
                }

                return livevideos;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public SmartEcommerce.Models.Product.LiveStreaming GetLiveStreamingByIdWithRelated(int Id)
        {
            SmartEcommerce.Models.Product.LiveStreaming streaming = new SmartEcommerce.Models.Product.LiveStreaming() { Id = Id };

            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@Id", Value = Id },
                    new SqlParameter { ParameterName = "@LoginId", Value = clsWebSession.LoginId }
                };

                DataTableCollection dtbls = DatabaseObject.FetchFromSP("LiveStreamingGetByIdWithRelated", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                }

                DataTable dt = dtbls[0];
                if (DataHelper.HasRows(dt))
                {
                    DataRow row = dt.Rows[0];

                    streaming.Title = row["Title"].ToString();
                    streaming.Description = row["Description"].ToString();
                    streaming.VideoURL = row["VideoURL"].ToString();
                    streaming.ImageURL = row["ImageURL"].ToString();
                    streaming.Active = DataHelper.boolParse(row["IsActive"]);
                    streaming.CreatedOn = DataHelper.dateParse(row["AddedOn"]);
                    streaming.CreatedBy = row["AddedByPartner"].ToString();
                    streaming.Likes = DataHelper.intParse(row["Likes"]);
                    streaming.Dislikes = DataHelper.intParse(row["DisLikes"]);
                    streaming.Views = DataHelper.intParse(row["Views"]);
                    streaming.ProfileImageURL = DataHelper.getProfileImageURL(row["ProfilePicture"].ToString());
                    streaming.ProfileURL = DataHelper.FriendlyUrlParse(row["AddedByPartner"].ToString());
                    streaming.SubscriberId = DataHelper.intParse(row["SubscriberId"]);
                    streaming.IsSubscribed = DataHelper.intParse(row["IsSubscribed"]);
                    streaming.TotalSubscribers = DataHelper.longParse(row["TotalSubscribers"]);
                    streaming.Notification = DataHelper.boolParse(row["Notification"]);
                    streaming.IsLike = DataHelper.boolParse(row["IsLike"]);
                    streaming.IsDislike = DataHelper.boolParse(row["IsDislike"]);

                    foreach (DataRow dr in dtbls[1].Rows)
                    {
                        streaming.RelatedLiveStreaming.Add(new SmartEcommerce.Models.Product.LiveStreaming()
                        {
                            Id = DataHelper.intParse(dr["Id"]),
                            Title = dr["Title"].ToString(),
                            Description = dr["Description"].ToString(),
                            ImageURL = DataHelper.getLiveStreamingImageURL(dr["ImageURL"].ToString()),
                            CreatedBy = dr["LastModifiedBy"].ToString(),
                            CreatedOn = DataHelper.dateParse(dr["AddedOn"]),
                            Likes = DataHelper.intParse(dr["Likes"]),
                            Dislikes = DataHelper.intParse(dr["Dislikes"]),
                            Views = DataHelper.intParse(dr["Views"]),
                            URL = "/live-tv/" + DataHelper.FriendlyUrlParse(dr["Title"].ToString())
                        });
                    }
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
            }

            return streaming;
        }

        public SmartEcommerce.Models.Product.LiveVideos GetLiveVideosByIdWithRelated(int Id)
        {
            SmartEcommerce.Models.Product.LiveVideos streaming = new SmartEcommerce.Models.Product.LiveVideos() { Id = Id };

            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@Id", Value = Id },
                    new SqlParameter { ParameterName = "@LoginId", Value = clsWebSession.LoginId }
                };

                DataTableCollection dtbls = DatabaseObject.FetchFromSP("LiveVideosGetByIdWithRelated", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                }

                DataTable dt = dtbls[0];
                if (DataHelper.HasRows(dt))
                {
                    DataRow row = dt.Rows[0];

                    streaming.Title = row["Title"].ToString();
                    streaming.Description = row["Description"].ToString();
                    streaming.VideoURL = row["VideoURL"].ToString();
                    streaming.ImageURL = row["ImageURL"].ToString();
                    streaming.Active = DataHelper.boolParse(row["IsActive"]);
                    streaming.CreatedOn = DataHelper.dateParse(row["AddedOn"]);
                    streaming.CreatedBy = row["AddedByPartner"].ToString();
                    streaming.Likes = DataHelper.intParse(row["Likes"]);
                    streaming.Dislikes = DataHelper.intParse(row["DisLikes"]);
                    streaming.Views = DataHelper.intParse(row["Views"]);
                    streaming.ProfileImageURL = DataHelper.getProfileImageURL(row["ProfilePicture"].ToString());
                    streaming.ProfileURL = DataHelper.FriendlyUrlParse(row["AddedByPartner"].ToString());
                    streaming.SubscriberId = DataHelper.intParse(row["SubscriberId"]);
                    streaming.IsSubscribed = DataHelper.intParse(row["IsSubscribed"]);
                    streaming.TotalSubscribers = DataHelper.longParse(row["TotalSubscribers"]);
                    streaming.Notification = DataHelper.boolParse(row["Notification"]);
                    streaming.IsLike = DataHelper.boolParse(row["IsLike"]);
                    streaming.IsDislike = DataHelper.boolParse(row["IsDislike"]);

                    foreach (DataRow dr in dtbls[1].Rows)
                    {
                        streaming.RelatedLiveVideos.Add(new SmartEcommerce.Models.Product.LiveVideos()
                        {
                            Id = DataHelper.intParse(dr["Id"]),
                            Title = dr["Title"].ToString(),
                            Description = dr["Description"].ToString(),
                            ImageURL = DataHelper.getLiveStreamingImageURL(dr["ImageURL"].ToString()),
                            CreatedBy = dr["LastModifiedBy"].ToString(),
                            CreatedOn = DataHelper.dateParse(dr["AddedOn"]),
                            Likes = DataHelper.intParse(dr["Likes"]),
                            Dislikes = DataHelper.intParse(dr["Dislikes"]),
                            Views = DataHelper.intParse(dr["Views"]),
                            URL = "/live-videos/" + DataHelper.FriendlyUrlParse(dr["Title"].ToString())
                        });
                    }
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
            }

            return streaming;
        }

        public SmartEcommerce.Models.Product.LiveStreaming GetLiveStreamingById(int Id)
        {
            SmartEcommerce.Models.Product.LiveStreaming streaming = new SmartEcommerce.Models.Product.LiveStreaming() { Id = Id };

            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@Id", Value = Id }
                };

                DataTable dt = DatabaseObject.FetchTableFromSP("LiveStreamingGetById", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                }

                if (DataHelper.HasRows(dt))
                {
                    DataRow row = dt.Rows[0];

                    streaming.Title = row["Title"].ToString();
                    streaming.Description = row["Description"].ToString();
                    streaming.VideoURL = row["VideoURL"].ToString();
                    streaming.ImageURL = row["ImageURL"].ToString();
                    streaming.Active = DataHelper.boolParse(row["IsActive"]);
                    streaming.AddedBy = DataHelper.intParse(row["AddedBy"]);
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
            }

            return streaming;
        }

        public SmartEcommerce.Models.Product.LiveVideos GetLiveVideosById(int Id)
        {
            SmartEcommerce.Models.Product.LiveVideos livevideos = new SmartEcommerce.Models.Product.LiveVideos() { Id = Id };

            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@Id", Value = Id }
                };

                DataTable dt = DatabaseObject.FetchTableFromSP("LiveVideosGetById", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                }

                if (DataHelper.HasRows(dt))
                {
                    DataRow row = dt.Rows[0];

                    livevideos.Title = row["Title"].ToString();
                    livevideos.Description = row["Description"].ToString();
                    livevideos.VideoURL = row["VideoURL"].ToString();
                    livevideos.ImageURL = row["ImageURL"].ToString();
                    livevideos.Active = DataHelper.boolParse(row["IsActive"]);
                    livevideos.AddedBy = DataHelper.intParse(row["AddedBy"]);

                    livevideos.EventDate = DataHelper.dateParse(row["EventDate"]);
                    livevideos.StringFromTime = DataHelper.dateParse(row["FromTime"]).ToString("hh:mm");
                    livevideos.StringToTime = DataHelper.dateParse(row["ToTime"]).ToString("hh:mm");
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
            }

            return livevideos;
        }

        public bool SaveLiveStreaming(int LoginId, int AddedBy, LiveStreaming streaming, out int entryLevel)
        {
            entryLevel = 0;
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {

                    new SqlParameter { ParameterName = "@Id", Value = streaming.Id },
                    new SqlParameter { ParameterName = "@Title", Value = streaming.Title },
                    new SqlParameter { ParameterName = "@Description", Value = streaming.Description },
                    new SqlParameter { ParameterName = "@VideoURL", Value = streaming.VideoURL },
                    new SqlParameter { ParameterName = "@ImageURL", Value = streaming.ImageURL },
                    new SqlParameter { ParameterName = "@IsActive", Value = streaming.Active },
                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId },
                    new SqlParameter { ParameterName = "@AddedBy", Value = AddedBy }
                };

                DataTable dt = DatabaseObject.FetchTableFromSP("LiveStreamingSave", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return false;
                }

                entryLevel = DataHelper.intParse(dt.Rows[0]["EntryLevel"]);

                return true;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return false;
            }
        }

        public bool SaveLiveVideos(int LoginId, int AddedBy, LiveVideos livevideos, out int entryLevel)
        {
            entryLevel = 0;
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {

                    new SqlParameter { ParameterName = "@Id", Value = livevideos.Id },
                    new SqlParameter { ParameterName = "@Title", Value = livevideos.Title },
                    new SqlParameter { ParameterName = "@Description", Value = livevideos.Description },
                    new SqlParameter { ParameterName = "@VideoURL", Value = livevideos.VideoURL },
                    new SqlParameter { ParameterName = "@ImageURL", Value = livevideos.ImageURL },
                    new SqlParameter { ParameterName = "@IsActive", Value = livevideos.Active },
                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId },
                    new SqlParameter { ParameterName = "@AddedBy", Value = AddedBy },
                    new SqlParameter { ParameterName = "@EventDate", Value = livevideos.EventDate },
                    new SqlParameter { ParameterName = "@FromTime", Value = livevideos.FromTime },
                    new SqlParameter { ParameterName = "@ToTime", Value = livevideos.ToTime }
                };

                DataTable dt = DatabaseObject.FetchTableFromSP("LiveVideosSave", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return false;
                }

                entryLevel = DataHelper.intParse(dt.Rows[0]["EntryLevel"]);

                return true;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return false;
            }
        }

        public bool ArchiveLiveStreaming(int LoginId, int Id, int Status)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {

                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId },
                    new SqlParameter { ParameterName = "@Id", Value = Id },
                    new SqlParameter { ParameterName = "@Status", Value = Status }
                };

                DatabaseObject.ExecuteSP("LiveStreamingMoveToArchive", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return false;
                }

                return true;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return false;
            }
        }

        public bool ArchiveLiveVideos(int LoginId, int Id, int Status)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {

                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId },
                    new SqlParameter { ParameterName = "@Id", Value = Id },
                    new SqlParameter { ParameterName = "@Status", Value = Status }
                };

                DatabaseObject.ExecuteSP("LiveVideosMoveToArchive", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return false;
                }

                return true;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return false;
            }
        }

        #endregion Live Streaming

        #region Programs

        public DataTable GetAllPrograms(int Status = BusinessLogic.Status.Current)
        {
            try
            {
                SqlParameter[] param = {

                    new SqlParameter { ParameterName = "@Status", Value = Status },
                    new SqlParameter { ParameterName = "@LoginType", Value = clsSession.LoginType },
                    new SqlParameter { ParameterName = "@LoginId", Value = clsSession.LoginId }
                };
                ErrorResponse response = new ErrorResponse();
                DataTable dt = DatabaseObject.FetchTableFromSP("ProgramsGetAll", param, ref response);

                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return null;
                }

                return dt;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public List<SmartEcommerce.Models.Product.Program> GetProgramsByStatus(int Status)
        {
            try
            {
                List<SmartEcommerce.Models.Product.Program> programs = new List<SmartEcommerce.Models.Product.Program>();
                DataTable dt = GetAllPrograms(Status);

                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        programs.Add(new SmartEcommerce.Models.Product.Program()
                        {
                            DT_RowId = DataHelper.intParse(row["Id"]),
                            Id = DataHelper.intParse(row["Id"]),
                            Title = row["Title"].ToString(),
                            Description = row["Description"].ToString(),
                            ImageURL = DataHelper.getProgramImageURL(row["ImageURL"].ToString()),
                            Active = DataHelper.boolParse(row["IsActive"]),
                            Featured = DataHelper.boolParse(row["IsFeatured"]),
                            CreatedBy = row["LastModifiedBy"].ToString(),
                            CreatedOn = DataHelper.dateParse(row["LastModifiedOn"])
                        });
                    }
                }

                return programs;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public SmartEcommerce.Models.Product.Program GetProgramsWithReatedData(int Status)
        {
            try
            {
                SmartEcommerce.Models.Product.Program programs = new SmartEcommerce.Models.Product.Program();

                SqlParameter[] param = {
                    new SqlParameter { ParameterName = "@Status", Value = Status },
                    new SqlParameter { ParameterName = "@LoginId", Value = clsWebSession.LoginId }
                };
                ErrorResponse response = new ErrorResponse();
                DataTableCollection dtbls = DatabaseObject.FetchFromSP("ProgramsGetAllWithRelatedData", param, ref response);

                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return null;
                }


                if (dtbls[0] != null)
                {
                    foreach (DataRow row in dtbls[0].Rows)
                    {
                        programs.AllPrograms.Add(new SmartEcommerce.Models.Product.Program()
                        {
                            DT_RowId = DataHelper.intParse(row["Id"]),
                            Id = DataHelper.intParse(row["Id"]),
                            Title = row["Title"].ToString(),
                            Description = row["Description"].ToString(),
                            ImageURL = DataHelper.getProgramImageURL(row["ImageURL"].ToString()),
                            Active = DataHelper.boolParse(row["IsActive"]),
                            Featured = DataHelper.boolParse(row["IsFeatured"]),
                            CreatedBy = row["LastModifiedBy"].ToString(),
                            CreatedOn = DataHelper.dateParse(row["LastModifiedOn"]),
                            URL = "/programs/" + DataHelper.FriendlyUrlParse(row["Title"].ToString())
                        });
                    }

                    foreach (DataRow row in dtbls[1].Rows)
                    {
                        programs.FeaturedPrograms.Add(new SmartEcommerce.Models.Product.Program()
                        {
                            DT_RowId = DataHelper.intParse(row["Id"]),
                            Id = DataHelper.intParse(row["Id"]),
                            Title = row["Title"].ToString(),
                            ImageURL = DataHelper.getProgramImageURL(row["ImageURL"].ToString()),
                            URL = "/programs/" + DataHelper.FriendlyUrlParse(row["Title"].ToString())
                        });
                    }

                    foreach (DataRow row in dtbls[2].Rows)
                    {
                        programs.TopEpisodes.Add(new SmartEcommerce.Models.Product.Videos()
                        {
                            DT_RowId = DataHelper.intParse(row["Id"]),
                            Id = DataHelper.intParse(row["Id"]),
                            Title = row["Title"].ToString(),
                            ImageURL = DataHelper.getVideoImageURL(row["ImageURL"].ToString()),
                            CreatedOn = DataHelper.dateParse(row["AddedOn"]),
                            Likes = DataHelper.intParse(row["Likes"]),
                            Dislikes = DataHelper.intParse(row["DisLikes"]),
                            Views = DataHelper.intParse(row["Views"]),
                            IsList = DataHelper.intParse(row["IsList"]),
                            URL = "/watch?v=" + SBEncryption.Encrypt(DataHelper.stringParse(row["Id"])),
                            CreatedBy = row["UserName"].ToString(),
                            ProfileURL = DataHelper.FriendlyUrlParse(row["UserName"].ToString())
                        });
                    }
                }

                return programs;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public SmartEcommerce.Models.Product.Program GetProgramById(int Id)
        {
            SmartEcommerce.Models.Product.Program program = new SmartEcommerce.Models.Product.Program() { Id = Id };

            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@Id", Value = Id }
                };

                DataTable dt = DatabaseObject.FetchTableFromSP("ProgramsGetById", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                }

                if (DataHelper.HasRows(dt))
                {
                    DataRow row = dt.Rows[0];

                    program.Title = row["Title"].ToString();
                    program.Description = row["Description"].ToString();
                    program.ImageURL = row["ImageURL"].ToString();
                    program.Featured = DataHelper.boolParse(row["IsFeatured"]);
                    program.Active = DataHelper.boolParse(row["IsActive"]);
                    program.AddedBy = DataHelper.intParse(row["AddedBy"]);
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
            }

            return program;
        }

        public bool SaveProgram(int LoginId, Program program, int AddedBy, out int entryLevel, out int programId)
        {
            entryLevel = 0; programId = 0;
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {

                    new SqlParameter { ParameterName = "@Id", Value = program.Id },
                    new SqlParameter { ParameterName = "@Title", Value = program.Title },
                    new SqlParameter { ParameterName = "@Description", Value = program.Description },
                    new SqlParameter { ParameterName = "@ImageURL", Value = program.ImageURL },
                    new SqlParameter { ParameterName = "@IsActive", Value = program.Active },
                    new SqlParameter { ParameterName = "@Featured", Value = program.Featured },
                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId },
                    new SqlParameter { ParameterName = "@AddedBy", Value = AddedBy }
                };

                DataTable dt = DatabaseObject.FetchTableFromSP("ProgramsSave", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return false;
                }

                entryLevel = DataHelper.intParse(dt.Rows[0]["EntryLevel"]);
                programId = DataHelper.intParse(dt.Rows[0]["Id"]);

                return true;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return false;
            }
        }

        public bool ArchiveProgram(int LoginId, int Id, int Status)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {

                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId },
                    new SqlParameter { ParameterName = "@Id", Value = Id },
                    new SqlParameter { ParameterName = "@Status", Value = Status }
                };

                DatabaseObject.ExecuteSP("ProgramsMoveToArchive", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return false;
                }

                return true;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return false;
            }
        }

        public SmartEcommerce.Models.Product.Program GetProgramDetailWithEpisodes(int Id)
        {
            SmartEcommerce.Models.Product.Program program = new SmartEcommerce.Models.Product.Program();

            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@Id", Value = Id },
                    new SqlParameter { ParameterName = "@LoginId", Value = clsWebSession.LoginId }
                };

                DataTableCollection dtbls = DatabaseObject.FetchFromSP("ProgramsGetByIdWithEpisodes", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                }

                DataTable dt = dtbls[0];

                if (DataHelper.HasRows(dt))
                {
                    DataRow row = dt.Rows[0];

                    program.Id = DataHelper.intParse(row["Id"]);
                    program.Title = row["Title"].ToString();
                    program.Description = row["Description"].ToString();
                    program.ImageURL = DataHelper.getProgramImageURL(row["ImageURL"].ToString()); //row["ImageURL"].ToString();
                    program.Active = DataHelper.boolParse(row["IsActive"]);


                    foreach (DataRow dr in dtbls[1].Rows)
                    {
                        program.Episodes.Add(new Videos
                        {
                            Id = DataHelper.intParse(dr["Id"]),
                            Title = dr["Title"].ToString(),
                            ImageURL = DataHelper.getVideoImageURL(dr["ImageURL"].ToString()),
                            VideoURL = dr["VideoURL"].ToString(),
                            CreatedOn = DataHelper.dateParse(dr["AddedOn"]),
                            Likes = DataHelper.intParse(dr["Likes"]),
                            Dislikes = DataHelper.intParse(dr["DisLikes"]),
                            Views = DataHelper.intParse(dr["Views"]),
                            IsList = DataHelper.intParse(dr["IsList"]),
                            URL = "/watch?v=" + SBEncryption.Encrypt(DataHelper.stringParse(dr["Id"])),
                            CreatedBy = dr["UserName"].ToString(),
                            ProfileURL = "/" + DataHelper.FriendlyUrlParse(dr["UserName"].ToString())
                        });
                    }
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                program.ErrorCode = "999";
                SBSession.CreateSession("error", ae.Message);
            }

            return program;
        }

        #endregion Programs

        #region Videos

        public DataTable GetAllVideos(int Status = BusinessLogic.Status.Current)
        {
            try
            {
                SqlParameter[] param = {
                    new SqlParameter { ParameterName = "@Status", Value = Status },
                    new SqlParameter { ParameterName = "@LoginType", Value = clsSession.LoginType },
                    new SqlParameter { ParameterName = "@LoginId", Value = clsSession.LoginId }
                };
                ErrorResponse response = new ErrorResponse();
                DataTable dt = DatabaseObject.FetchTableFromSP("VideosGetAll", param, ref response);

                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return null;
                }

                return dt;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public DataTable GetAllVideosByCategory(int CategoryId)
        {
            try
            {
                SqlParameter[] param = {

                    new SqlParameter { ParameterName = "@CategoryId", Value = CategoryId },
                    new SqlParameter { ParameterName = "@LoginId", Value = clsWebSession.LoginId }
                };
                ErrorResponse response = new ErrorResponse();
                DataTable dt = DatabaseObject.FetchTableFromSP("VideosGetAllByCategory", param, ref response);

                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return null;
                }

                return dt;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public List<SmartEcommerce.Models.Product.Videos> GetVideosByStatus(int Status)
        {
            try
            {
                List<SmartEcommerce.Models.Product.Videos> videos = new List<SmartEcommerce.Models.Product.Videos>();
                DataTable dt = GetAllVideos(Status);

                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        videos.Add(new SmartEcommerce.Models.Product.Videos()
                        {
                            DT_RowId = DataHelper.intParse(row["Id"]),
                            Id = DataHelper.intParse(row["Id"]),
                            Title = row["Title"].ToString(),
                            Program = new Program
                            {
                                Id = DataHelper.intParse(row["ProgramId"]),
                                Title = row["ProgramName"].ToString()
                            },
                            Category = new Category
                            {
                                Id = DataHelper.intParse(row["CategoryId"]),
                                Title = row["CategoryName"].ToString()
                            },
                            Description = row["Description"].ToString(),
                            ImageURL = DataHelper.getVideoImageURL(row["ImageURL"].ToString()),
                            Active = DataHelper.boolParse(row["IsActive"]),
                            CreatedBy = row["LastModifiedBy"].ToString(),
                            CreatedOn = DataHelper.dateParse(row["AddedOn"]),
                            Likes = DataHelper.intParse(row["Likes"]),
                            Dislikes = DataHelper.intParse(row["DisLikes"]),
                            Views = DataHelper.intParse(row["Views"]),
                            IsList = DataHelper.intParse(row["IsList"])
                        });
                    }

                    
                }

                return videos;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public Videos GetVideosByStatusLoadMore(int Status, int page)
        {
            Videos result = new Videos();

            try
            {

                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@Status", Value = Status },
                    new SqlParameter { ParameterName = "@page", Value = page },
                    new SqlParameter { ParameterName = "@LoginId", Value = clsWebSession.LoginId }
                };

                DataTableCollection dtbls = DatabaseObject.FetchFromSP("VideosGetAllLoadMore", param, ref response);

                if (!response.Error)
                {
                    // Vidoes
                    foreach (DataRow row in dtbls[0].Rows)
                    {
                        result.VideoLibrary.Add(new SmartEcommerce.Models.Product.Videos()
                        {
                            DT_RowId = DataHelper.intParse(row["Id"]),
                            Id = DataHelper.intParse(row["Id"]),
                            Title = row["Title"].ToString(),
                            Program = new Program
                            {
                                Id = DataHelper.intParse(row["ProgramId"]),
                                Title = row["ProgramName"].ToString()
                            },
                            Category = new Category
                            {
                                Id = DataHelper.intParse(row["CategoryId"]),
                                Title = row["CategoryName"].ToString()
                            },
                            ImageURL = DataHelper.getVideoImageURL(row["ImageURL"].ToString()),
                            CreatedOn = DataHelper.dateParse(row["AddedOn"]),
                            Likes = DataHelper.intParse(row["Likes"]),
                            Dislikes = DataHelper.intParse(row["DisLikes"]),
                            Views = DataHelper.intParse(row["Views"]),
                            IsList = DataHelper.intParse(row["IsList"]),
                            URL = "/watch?v=" + SBEncryption.Encrypt(DataHelper.stringParse(row["Id"])),
                            FormatNumber = DataHelper.FormatNumber(DataHelper.intParse(row["Views"])),
                            TimeAgo = DataHelper.TimeAgo(DataHelper.dateParse(row["AddedOn"])),
                            CreatedBy = row["UserName"].ToString(),
                            ProfileURL = DataHelper.FriendlyUrlParse(row["UserName"].ToString())
                        });
                    }

                    // Total Records 
                    result.TotalRecords = DataHelper.intParse(dtbls[1].Rows[0]["TotalRecords"]);

                    // RemainingRecords
                    result.RemainingRecords = DataHelper.intParse(dtbls[1].Rows[0]["RemainingRecords"]);
                    result.StartingRecord = DataHelper.intParse(dtbls[1].Rows[0]["ShowingRecords"]);

                    int perPageRecords = DataHelper.intParse(dtbls[1].Rows[0]["PerPageRecords"]);

                    if (result.TotalRecords > (result.StartingRecord - 1) + perPageRecords)
                        result.EndingRecord = (result.StartingRecord - 1) + perPageRecords;
                    else
                        result.EndingRecord = result.TotalRecords;

                }
                else
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
            }

            return result;
        }

        public Videos GetVideosByRecentWatchLoadMore(int page)
        {
            Videos result = new Videos();

            try
            {

                XmlDocument xmldoc = new XmlDocument();
                XmlElement doc = xmldoc.CreateElement("doc");
                xmldoc.AppendChild(doc);

                foreach (long itemId in clsCookie.RecentlyViewedProducts)
                {
                    XmlElement item = xmldoc.CreateElement("item");
                    doc.AppendChild(item);

                    item.AppendChild(xmldoc.CreateElement("ItemId")).InnerText = itemId.ToString();
                }

                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param = {
                                        new SqlParameter("@ViewedItems", SqlDbType.Xml)
                                        { Value = new SqlXml(new XmlTextReader(xmldoc.InnerXml, XmlNodeType.Document, null)) },
                                        new SqlParameter("@LoginId",BusinessLogic.clsWebSession.LoginId),
                                        new SqlParameter("@page",page)
                                    };

                DataTableCollection dtbls = DatabaseObject.FetchFromSP("RecentWatchLoadMore", param, ref response);
                if (!response.Error)
                {
                    // Vidoes
                    foreach (DataRow row in dtbls[0].Rows)
                    {
                        result.VideoLibrary.Add(new SmartEcommerce.Models.Product.Videos()
                        {
                            DT_RowId = DataHelper.intParse(row["Id"]),
                            Id = DataHelper.intParse(row["Id"]),
                            Title = row["Title"].ToString(),
                            Program = new Program
                            {
                                Id = DataHelper.intParse(row["ProgramId"]),
                                Title = row["ProgramName"].ToString()
                            },
                            Category = new Category
                            {
                                Id = DataHelper.intParse(row["CategoryId"]),
                                Title = row["CategoryName"].ToString()
                            },
                            ImageURL = DataHelper.getVideoImageURL(row["ImageURL"].ToString()),
                            CreatedOn = DataHelper.dateParse(row["AddedOn"]),
                            Likes = DataHelper.intParse(row["Likes"]),
                            Dislikes = DataHelper.intParse(row["DisLikes"]),
                            Views = DataHelper.intParse(row["Views"]),
                            IsList = DataHelper.intParse(row["IsList"]),
                            URL = "/watch?v=" + SBEncryption.Encrypt(DataHelper.stringParse(row["Id"])),
                            FormatNumber = DataHelper.FormatNumber(DataHelper.intParse(row["Views"])),
                            TimeAgo = DataHelper.TimeAgo(DataHelper.dateParse(row["AddedOn"]))

                        });
                    }

                    // Total Records 
                    result.TotalRecords = DataHelper.intParse(dtbls[1].Rows[0]["TotalRecords"]);

                    // RemainingRecords
                    result.RemainingRecords = DataHelper.intParse(dtbls[1].Rows[0]["RemainingRecords"]);
                    result.StartingRecord = DataHelper.intParse(dtbls[1].Rows[0]["ShowingRecords"]);

                    int perPageRecords = DataHelper.intParse(dtbls[1].Rows[0]["PerPageRecords"]);

                    if (result.TotalRecords > (result.StartingRecord - 1) + perPageRecords)
                        result.EndingRecord = (result.StartingRecord - 1) + perPageRecords;
                    else
                        result.EndingRecord = result.TotalRecords;

                }
                else
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
            }

            return result;
        }

        public Videos GetVideosByUserLoadMore(int UserId, int page)
        {
            Videos result = new Videos();

            try
            {

                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@UserId", Value = UserId },
                    new SqlParameter { ParameterName = "@page", Value = page },
                    new SqlParameter { ParameterName = "@LoginId", Value = clsWebSession.LoginId }
                };

                DataTableCollection dtbls = DatabaseObject.FetchFromSP("VideosGetByUserIdLoadMore", param, ref response);
                if (!response.Error)
                {
                    // Vidoes
                    foreach (DataRow row in dtbls[0].Rows)
                    {
                        result.VideoLibrary.Add(new SmartEcommerce.Models.Product.Videos()
                        {
                            DT_RowId = DataHelper.intParse(row["Id"]),
                            Id = DataHelper.intParse(row["Id"]),
                            Title = row["Title"].ToString(),
                            Program = new Program
                            {
                                Id = DataHelper.intParse(row["ProgramId"]),
                                Title = row["ProgramName"].ToString()
                            },
                            Category = new Category
                            {
                                Id = DataHelper.intParse(row["CategoryId"]),
                                Title = row["CategoryName"].ToString()
                            },
                            ImageURL = DataHelper.getVideoImageURL(row["ImageURL"].ToString()),
                            CreatedOn = DataHelper.dateParse(row["AddedOn"]),
                            Likes = DataHelper.intParse(row["Likes"]),
                            Dislikes = DataHelper.intParse(row["DisLikes"]),
                            Views = DataHelper.intParse(row["Views"]),
                            IsList = DataHelper.intParse(row["IsList"]),
                            URL = "/watch?v=" + SBEncryption.Encrypt(DataHelper.stringParse(row["Id"])),
                            FormatNumber = DataHelper.FormatNumber(DataHelper.intParse(row["Views"])),
                            TimeAgo = DataHelper.TimeAgo(DataHelper.dateParse(row["AddedOn"])),
                            CreatedBy = row["UserName"].ToString(),
                            ProfileURL = DataHelper.FriendlyUrlParse(row["UserName"].ToString())
                        });
                    }

                    // Total Records 
                    result.TotalRecords = DataHelper.intParse(dtbls[1].Rows[0]["TotalRecords"]);

                    // RemainingRecords
                    result.RemainingRecords = DataHelper.intParse(dtbls[1].Rows[0]["RemainingRecords"]);
                    result.StartingRecord = DataHelper.intParse(dtbls[1].Rows[0]["ShowingRecords"]);

                    int perPageRecords = DataHelper.intParse(dtbls[1].Rows[0]["PerPageRecords"]);

                    if (result.TotalRecords > (result.StartingRecord - 1) + perPageRecords)
                        result.EndingRecord = (result.StartingRecord - 1) + perPageRecords;
                    else
                        result.EndingRecord = result.TotalRecords;


                    // Check Current Subscriber is subscribed or not
                    result.IsSubscribed = DataHelper.intParse(dtbls[2].Rows[0]["IsSubscribed"]);

                }
                else
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
            }

            return result;
        }

        public List<SmartEcommerce.Models.Product.Videos> GetVideosByCategory(int CategoryId)
        {
            try
            {
                List<SmartEcommerce.Models.Product.Videos> videos = new List<SmartEcommerce.Models.Product.Videos>();
                DataTable dt = GetAllVideosByCategory(CategoryId);

                if (dt != null)
                {

                    foreach (DataRow row in dt.Rows)
                    {
                        videos.Add(new SmartEcommerce.Models.Product.Videos()
                        {
                            DT_RowId = DataHelper.intParse(row["Id"]),
                            Id = DataHelper.intParse(row["Id"]),
                            Title = row["Title"].ToString(),
                            Program = new Program
                            {
                                Id = DataHelper.intParse(row["ProgramId"]),
                                Title = row["ProgramName"].ToString()
                            },
                            Category = new Category
                            {
                                Id = DataHelper.intParse(row["CategoryId"]),
                                Title = row["CategoryName"].ToString()
                            },
                            Description = row["Description"].ToString(),
                            ImageURL = DataHelper.getVideoImageURL(row["ImageURL"].ToString()),
                            Active = DataHelper.boolParse(row["IsActive"]),
                            CreatedBy = row["LastModifiedBy"].ToString(),
                            CreatedOn = DataHelper.dateParse(row["AddedOn"]),
                            Likes = DataHelper.intParse(row["Likes"]),
                            Dislikes = DataHelper.intParse(row["DisLikes"]),
                            Views = DataHelper.intParse(row["Views"]),
                            IsList = DataHelper.intParse(row["IsList"]),
                            URL = "/watch?v=" + SBEncryption.Encrypt(DataHelper.stringParse(row["Id"]))
                        });
                    }
                }

                return videos;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public SmartEcommerce.Models.Product.Category GetVideosByCategoryLoadMore(int Id, int page)
        {
            SmartEcommerce.Models.Product.Category category = new SmartEcommerce.Models.Product.Category();

            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@Id", Value = Id },
                    new SqlParameter { ParameterName = "@page", Value = page },
                    new SqlParameter { ParameterName = "@LoginId", Value = clsWebSession.LoginId }
                };

                DataTableCollection dtbls = DatabaseObject.FetchFromSP("VideosGetAllByCategoryLoadMore", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                }

                DataTable dt = dtbls[0];

                if (DataHelper.HasRows(dt))
                {
                    DataRow row = dt.Rows[0];

                    category.Id = DataHelper.intParse(row["Id"]);
                    category.Title = row["Title"].ToString();
                    category.ImageURL = DataHelper.getCategoryImageURL(row["ImageURL"].ToString());
                    category.Active = DataHelper.boolParse(row["IsActive"]);

                    foreach (DataRow dr in dtbls[1].Rows)
                    {
                        category.Videos.Add(new Videos
                        {
                            Id = DataHelper.intParse(dr["Id"]),
                            Title = dr["Title"].ToString(),
                            ImageURL = DataHelper.getVideoImageURL(dr["ImageURL"].ToString()),
                            VideoURL = dr["VideoURL"].ToString(),
                            CreatedBy = dr["UserName"].ToString(),
                            CreatedOn = DataHelper.dateParse(dr["AddedOn"]),
                            Likes = DataHelper.intParse(dr["Likes"]),
                            Dislikes = DataHelper.intParse(dr["DisLikes"]),
                            Views = DataHelper.intParse(dr["Views"]),
                            IsList = DataHelper.intParse(dr["IsList"]),
                            URL = "/watch?v=" + SBEncryption.Encrypt(DataHelper.stringParse(dr["Id"])),
                            FormatNumber = DataHelper.FormatNumber(DataHelper.intParse(dr["Views"])),
                            TimeAgo = DataHelper.TimeAgo(DataHelper.dateParse(dr["AddedOn"])),
                            ProfileURL = "/" + DataHelper.FriendlyUrlParse(dr["UserName"].ToString())
                        });
                    }

                    // Total Records 
                    category.TotalRecords = DataHelper.intParse(dtbls[2].Rows[0]["TotalRecords"]);

                    // RemainingRecords
                    category.RemainingRecords = DataHelper.intParse(dtbls[2].Rows[0]["RemainingRecords"]);
                    category.StartingRecord = DataHelper.intParse(dtbls[2].Rows[0]["ShowingRecords"]);

                    int perPageRecords = DataHelper.intParse(dtbls[2].Rows[0]["PerPageRecords"]);

                    if (category.TotalRecords > (category.StartingRecord - 1) + perPageRecords)
                        category.EndingRecord = (category.StartingRecord - 1) + perPageRecords;
                    else
                        category.EndingRecord = category.TotalRecords;


                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                category.ErrorCode = "999";
                SBSession.CreateSession("error", ae.Message);
            }

            return category;
        }

        public SmartEcommerce.Models.Product.Videos GetVideoById(int Id)
        {
            SmartEcommerce.Models.Product.Videos video = new SmartEcommerce.Models.Product.Videos() { Id = Id };

            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@Id", Value = Id },
                    new SqlParameter { ParameterName = "@LoginType", Value = clsSession.LoginType }
                };

                DataTable dt = DatabaseObject.FetchTableFromSP("VideosGetById", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                }

                if (DataHelper.HasRows(dt))
                {
                    DataRow row = dt.Rows[0];

                    if (row["ErrorCode"].ToString() == "001")
                    {
                        video.ErrorCode = row["ErrorCode"].ToString();
                        return video;
                    }

                    video.Title = row["Title"].ToString();
                    video.Program.Id = DataHelper.intParse(row["ProgramId"]);
                    video.Category.Id = DataHelper.intParse(row["CategoryId"]);
                    video.ImageURL = row["ImageURL"].ToString();
                    video.VideoURL = row["VideoURL"].ToString();
                    video.Description = row["Description"].ToString();
                    video.Active = DataHelper.boolParse(row["IsActive"]);
                    video.AddedBy = DataHelper.intParse(row["AddedBy"]);
                    video.ErrorCode = row["ErrorCode"].ToString();
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
            }

            return video;
        }

        public bool SaveVideos(int LoginId, int AddedBy, Videos video, out int entryLevel)
        {
            entryLevel = 0;
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {

                    new SqlParameter { ParameterName = "@Id", Value = video.Id },
                    new SqlParameter { ParameterName = "@Title", Value = video.Title },
                    new SqlParameter { ParameterName = "@Description", Value = video.Description },
                    new SqlParameter { ParameterName = "@ProgramId", Value = video.Program.Id },
                    new SqlParameter { ParameterName = "@CategoryId", Value = video.Category.Id },
                    new SqlParameter { ParameterName = "@ImageURL", Value = video.ImageURL },
                    new SqlParameter { ParameterName = "@VideoURL", Value = video.VideoURL },
                    new SqlParameter { ParameterName = "@IsActive", Value = video.Active },
                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId },
                    new SqlParameter { ParameterName = "@AddedBy", Value = AddedBy }
                };

                DataTable dt = DatabaseObject.FetchTableFromSP("VideosSave", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return false;
                }

                entryLevel = DataHelper.intParse(dt.Rows[0]["EntryLevel"]);

                return true;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return false;
            }
        }

        public string ArchiveVideos(int LoginId, int Id, int Status)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {

                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId },
                    new SqlParameter { ParameterName = "@Id", Value = Id },
                    new SqlParameter { ParameterName = "@Status", Value = Status },
                    new SqlParameter { ParameterName = "@LoginType", Value = clsSession.LoginType }
                };

                DataTable dt = DatabaseObject.FetchTableFromSP("VideosMoveToArchive", param, ref response);
                
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return "999";
                }

                DataRow row = dt.Rows[0];
                string ErrorCode = row["ErrorCode"].ToString();

                return ErrorCode;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return "999";
            }
        }

        public bool StatusVideos(int LoginId, int Id, int Status)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {

                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId },
                    new SqlParameter { ParameterName = "@Id", Value = Id },
                    new SqlParameter { ParameterName = "@Status", Value = Status }
                };

                DatabaseObject.ExecuteSP("VideosStatus", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return false;
                }

                return true;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return false;
            }
        }

        public bool StatusLiveStreaming(int LoginId, int Id, int Status)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {

                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId },
                    new SqlParameter { ParameterName = "@Id", Value = Id },
                    new SqlParameter { ParameterName = "@Status", Value = Status }
                };

                DatabaseObject.ExecuteSP("LiveStreamingStatus", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return false;
                }

                return true;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return false;
            }
        }

        public bool StatusLiveVideos(int LoginId, int Id, int Status)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {

                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId },
                    new SqlParameter { ParameterName = "@Id", Value = Id },
                    new SqlParameter { ParameterName = "@Status", Value = Status }
                };

                DatabaseObject.ExecuteSP("LiveVideosStatus", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return false;
                }

                return true;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return false;
            }
        }

        public SmartEcommerce.Models.Product.Videos GetVideoByIdWithRelated(int Id)
        {

            if (clsWebSession.LoginId == 0)
            {
                List<long> list = BusinessLogic.clsCookie.RecentlyViewedProducts;

                if (list.Where(r => r == Id).Count() == 0)
                    list.Add(Id);


                HttpCookie userCookie = new HttpCookie(BusinessLogic.UserInfoCookie.scookiename);                           // Create Cookie Object For Item Info
                userCookie.Expires = DateTime.Now.AddDays(3);
                userCookie[BusinessLogic.UserInfoCookie.sItemId] = SBEncryption.Encrypt(string.Join(",", list.ToArray()));  // Video Id

                System.Web.HttpContext.Current.Response.Cookies.Add(userCookie);
            }

            SmartEcommerce.Models.Product.Videos video = new SmartEcommerce.Models.Product.Videos() { Id = Id };

            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@Id", Value = Id },
                    new SqlParameter { ParameterName = "@LoginId", Value = clsWebSession.LoginId }
                };

                DataTableCollection dtbls = DatabaseObject.FetchFromSP("VideosGetByIdWithRelated", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                }

                DataTable dt = dtbls[0];

                if (DataHelper.HasRows(dt))
                {
                    DataRow row = dt.Rows[0];

                    video.Title = row["Title"].ToString();
                    video.Program.Id = DataHelper.intParse(row["ProgramId"]);
                    video.Category.Id = DataHelper.intParse(row["CategoryId"]);
                    video.ImageURL = DataHelper.getVideoImageURL(row["ImageURL"].ToString());
                    video.VideoURL = row["VideoURL"].ToString();
                    video.Description = row["Description"].ToString();
                    video.Active = DataHelper.boolParse(row["IsActive"]);
                    video.Likes = DataHelper.intParse(row["Likes"]);
                    video.Dislikes = DataHelper.intParse(row["DisLikes"]);
                    video.Views = DataHelper.intParse(row["Views"]);
                    video.CreatedBy = row["AddedBy"].ToString();
                    video.CreatedOn = DataHelper.dateParse(row["AddedOn"]);
                    video.IsList = DataHelper.intParse(row["IsList"]);
                    video.ProfileImageURL = DataHelper.getProfileImageURL(row["ProfilePicture"].ToString());
                    video.ProfileURL = DataHelper.FriendlyUrlParse(row["AddedBy"].ToString());
                    video.SubscriberId = DataHelper.intParse(row["SubscriberId"]);
                    video.IsSubscribed = DataHelper.intParse(row["IsSubscribed"]);
                    video.TotalSubscribers = DataHelper.longParse(row["TotalSubscribers"]);
                    video.Notification = DataHelper.boolParse(row["Notification"]);
                    video.IsLike = DataHelper.boolParse(row["IsLike"]);
                    video.IsDislike = DataHelper.boolParse(row["IsDislike"]);

                    foreach (DataRow dr in dtbls[1].Rows)
                    {
                        video.RelatedVideos.Add(new SmartEcommerce.Models.Product.Videos()
                        {
                            Id = DataHelper.intParse(dr["Id"]),
                            Title = dr["Title"].ToString(),
                            ImageURL = DataHelper.getVideoImageURL(dr["ImageURL"].ToString()),
                            CreatedBy = dr["AddedBy"].ToString(),
                            CreatedOn = DataHelper.dateParse(dr["AddedOn"]),
                            Likes = DataHelper.intParse(dr["Likes"]),
                            Dislikes = DataHelper.intParse(dr["DisLikes"]),
                            Views = DataHelper.intParse(dr["Views"]),
                            IsList = DataHelper.intParse(dr["IsList"]),
                            URL = "/watch?v=" + SBEncryption.Encrypt(DataHelper.stringParse(dr["Id"])),
                            ProfileURL = DataHelper.FriendlyUrlParse(dr["AddedBy"].ToString())
                        });
                    }


                    foreach (DataRow dr in dtbls[2].Rows)
                    {
                        video.ProgramCategoryVideos.Add(new SmartEcommerce.Models.Product.Videos()
                        {
                            Id = DataHelper.intParse(dr["Id"]),
                            Title = dr["Title"].ToString(),
                            ImageURL = DataHelper.getVideoImageURL(dr["ImageURL"].ToString()),
                            CreatedBy = dr["AddedBy"].ToString(),
                            CreatedOn = DataHelper.dateParse(dr["AddedOn"]),
                            Likes = DataHelper.intParse(dr["Likes"]),
                            Dislikes = DataHelper.intParse(dr["DisLikes"]),
                            Views = DataHelper.intParse(dr["Views"]),
                            IsList = DataHelper.intParse(dr["IsList"]),
                            URL = "/watch?v=" + SBEncryption.Encrypt(DataHelper.stringParse(dr["Id"])),
                            ProfileURL = DataHelper.FriendlyUrlParse(dr["AddedBy"].ToString())
                        });
                    }
                }

                // Check Current Subscriber is subscribed or not
                //video.IsSubscribed = DataHelper.intParse(dtbls[3].Rows[0]["IsSubscribed"]);
                
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
            }

            return video;
        }

        public List<SmartEcommerce.Models.Product.Videos> GetAllRelatedVideos(int Id)
        {
            try
            {
                List<SmartEcommerce.Models.Product.Videos> videos = new List<SmartEcommerce.Models.Product.Videos>();

                SqlParameter[] param = {
                                            new SqlParameter { ParameterName = "@Id", Value = Id },
                                            new SqlParameter { ParameterName = "@LoginId", Value = clsWebSession.LoginId }
                                        };
                ErrorResponse response = new ErrorResponse();
                DataTable dt = DatabaseObject.FetchTableFromSP("VideosGetAllRelated", param, ref response);

                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                }


                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        videos.Add(new SmartEcommerce.Models.Product.Videos()
                        {
                            Id = DataHelper.intParse(row["Id"]),
                            Title = row["Title"].ToString(),
                            ImageURL = DataHelper.getVideoImageURL(row["ImageURL"].ToString()),
                            CreatedBy = row["UserName"].ToString(),
                            CreatedOn = DataHelper.dateParse(row["AddedOn"]),
                            Likes = DataHelper.intParse(row["Likes"]),
                            Dislikes = DataHelper.intParse(row["DisLikes"]),
                            Views = DataHelper.intParse(row["Views"]),
                            IsList = DataHelper.intParse(row["IsList"]),
                            URL = "/watch?v=" + SBEncryption.Encrypt(DataHelper.stringParse(row["Id"])),
                            ProfileURL = DataHelper.FriendlyUrlParse(row["UserName"].ToString())
                        });
                    }
                }

                return videos;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public List<SmartEcommerce.Models.Product.Videos> LoadMoreRelatedVideos(int Id, int StartIndex, int Offset)
        {
            try
            {
                List<SmartEcommerce.Models.Product.Videos> videos = new List<SmartEcommerce.Models.Product.Videos>();

                SqlParameter[] param = {
                                            new SqlParameter { ParameterName = "@Id", Value = Id },
                                            new SqlParameter { ParameterName = "@StartIndex", Value = StartIndex },
                                            new SqlParameter { ParameterName = "@Offset", Value = Offset }
                                        };
                ErrorResponse response = new ErrorResponse();
                DataTable dt = DatabaseObject.FetchTableFromSP("LoadMoreRelatedVideos", param, ref response);

                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                }


                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        videos.Add(new SmartEcommerce.Models.Product.Videos()
                        {
                            Title = row["Title"].ToString(),
                            Description = row["Description"].ToString(),
                            ImageURL = DataHelper.getVideoImageURL(row["ImageURL"].ToString()),
                            Active = DataHelper.boolParse(row["IsActive"]),
                            CreatedBy = row["LastModifiedBy"].ToString(),
                            CreatedOn = DataHelper.dateParse(row["LastModifiedOn"]),
                            Likes = DataHelper.intParse(row["Likes"]),
                            Dislikes = DataHelper.intParse(row["DisLikes"]),
                            Views = DataHelper.intParse(row["Views"]),
                            //IsList = DataHelper.intParse(row["IsList"])
                        });
                    }
                }

                return videos;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        #endregion Videos

        #region Video Search
        public VideoSearch Search(string keyword, int page)
        {
            VideoSearch result = new VideoSearch();

            try
            {
                if (keyword != "")
                    keyword = "%" + keyword + "%";

                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@keyword", Value = keyword },
                    new SqlParameter { ParameterName = "@page", Value = page },
                    new SqlParameter { ParameterName = "@LoginId", Value = clsWebSession.LoginId }
                };

                DataTableCollection dtbls = DatabaseObject.FetchFromSP("VideosSearch", param, ref response);
                if (!response.Error)
                {
                   
                    // Vidoes
                    foreach (DataRow row in dtbls[0].Rows)
                    {
                        result.Videos.Add(new VideoSearchVideos
                        {
                            Id = DataHelper.intParse(row["Id"]),
                            Title = row["Title"].ToString(),
                            UserName = row["UserName"].ToString(),
                            ImageURL = DataHelper.getVideoImageURL(row["ImageURL"].ToString()),
                            Likes = DataHelper.intParse(row["Likes"]),
                            Dislikes = DataHelper.intParse(row["DisLikes"]),
                            Views = DataHelper.intParse(row["Views"]),
                            CategoryId = DataHelper.intParse(row["CategoryId"]),
                            CategoryName = row["CategoryName"].ToString(),
                            IsList = DataHelper.intParse(row["IsList"]),
                            URL = "/watch?v=" + SBEncryption.Encrypt(DataHelper.stringParse(row["Id"])),
                            FormatNumber = DataHelper.FormatNumber(DataHelper.intParse(row["Views"])),
                            TimeAgo = DataHelper.TimeAgo(DataHelper.dateParse(row["AddedOn"])),
                            ProfileURL = DataHelper.FriendlyUrlParse(row["UserName"].ToString())
                        });
                    }

                    // Total Records 
                    result.TotalRecords = DataHelper.intParse(dtbls[1].Rows[0]["TotalRecords"]);

                    // RemainingRecords
                    result.RemainingRecords = DataHelper.intParse(dtbls[1].Rows[0]["RemainingRecords"]);
                    result.StartingRecord = DataHelper.intParse(dtbls[1].Rows[0]["ShowingRecords"]);

                    int perPageRecords = DataHelper.intParse(dtbls[1].Rows[0]["PerPageRecords"]);

                    if (result.TotalRecords > (result.StartingRecord - 1) + perPageRecords)
                        result.EndingRecord = (result.StartingRecord - 1) + perPageRecords;
                    else
                        result.EndingRecord = result.TotalRecords;
                    
                }
                else
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
            }

            return result;
        }

        #endregion Video Search

        public bool ClaimVideosStatus(int LoginId, int Id, int Status)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {

                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId },
                    new SqlParameter { ParameterName = "@Id", Value = Id },
                    new SqlParameter { ParameterName = "@Status", Value = Status }
                };

                DataTable dt = DatabaseObject.FetchTableFromSP("ClaimVideosStatus", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return false;
                }
                //fetch from sp email address and claim vides description

                DataRow row = dt.Rows[0];
                string CustomerName = row["CustomerName"].ToString();
                string CustomerEmail = row["CustomerEmail"].ToString();
                string VideoURL = row["VideoURL"].ToString();
                string Description = row["Description"].ToString();
                string StatusName = Status == 1 ? "Approved" : "Rejected";

                string admin_email = ConfigurationManager.AppSettings["admin_email"];

                StringBuilder sbHtml = new StringBuilder();
                var path = System.Web.HttpContext.Current.Server.MapPath("~/content/emails/dispute_video_status.html");
                sbHtml.AppendLine(System.IO.File.ReadAllText(path));

                sbHtml = sbHtml.Replace("{CUSTOMERNAME}", CustomerName);
                sbHtml = sbHtml.Replace("{VIDEOURL}", VideoURL);
                sbHtml = sbHtml.Replace("{DESCRIPTION}", Description);
                sbHtml = sbHtml.Replace("{STATUS}", StatusName);

                bool IsSend = Emails.SendMail(CustomerEmail, admin_email, "Net Five - Claim Video " + StatusName, sbHtml.ToString(), true);

                return IsSend;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return false;
            }
        }

        public Videos GetMyListLoadMore(int page)
        {
            Videos result = new Videos();

            try
            {

                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@page", Value = page },
                    new SqlParameter { ParameterName = "@LoginId", Value = clsWebSession.LoginId }
                };

                DataTableCollection dtbls = DatabaseObject.FetchFromSP("VideosGetMyList", param, ref response);
                if (!response.Error)
                {
                    // Vidoes
                    foreach (DataRow row in dtbls[0].Rows)
                    {
                        result.VideoLibrary.Add(new SmartEcommerce.Models.Product.Videos()
                        {
                            DT_RowId = DataHelper.intParse(row["Id"]),
                            Id = DataHelper.intParse(row["Id"]),
                            Title = row["Title"].ToString(),
                            Program = new Program
                            {
                                Id = DataHelper.intParse(row["ProgramId"]),
                                Title = row["ProgramName"].ToString()
                            },
                            Category = new Category
                            {
                                Id = DataHelper.intParse(row["CategoryId"]),
                                Title = row["CategoryName"].ToString()
                            },
                            ImageURL = DataHelper.getVideoImageURL(row["ImageURL"].ToString()),
                            CreatedOn = DataHelper.dateParse(row["AddedOn"]),
                            Likes = DataHelper.intParse(row["Likes"]),
                            Dislikes = DataHelper.intParse(row["DisLikes"]),
                            Views = DataHelper.intParse(row["Views"]),
                            IsList = DataHelper.intParse(row["IsList"]),
                            URL = "/watch?v=" + SBEncryption.Encrypt(DataHelper.stringParse(row["Id"])),
                            FormatNumber = DataHelper.FormatNumber(DataHelper.intParse(row["Views"])),
                            TimeAgo = DataHelper.TimeAgo(DataHelper.dateParse(row["AddedOn"])),
                            CreatedBy = row["UserName"].ToString(),
                            ProfileURL = DataHelper.FriendlyUrlParse(row["UserName"].ToString())
                        });
                    }

                    // Total Records 
                    result.TotalRecords = DataHelper.intParse(dtbls[1].Rows[0]["TotalRecords"]);

                    // RemainingRecords
                    result.RemainingRecords = DataHelper.intParse(dtbls[1].Rows[0]["RemainingRecords"]);
                    result.StartingRecord = DataHelper.intParse(dtbls[1].Rows[0]["ShowingRecords"]);

                    int perPageRecords = DataHelper.intParse(dtbls[1].Rows[0]["PerPageRecords"]);

                    if (result.TotalRecords > (result.StartingRecord - 1) + perPageRecords)
                        result.EndingRecord = (result.StartingRecord - 1) + perPageRecords;
                    else
                        result.EndingRecord = result.TotalRecords;

                }
                else
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
            }

            return result;
        }
    }
}
