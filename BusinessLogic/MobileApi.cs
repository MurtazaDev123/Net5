using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public class MobileApi
    {
        public object GetLibrary(string categories, int LoginId, int Page)
        {
            try
            {
                DataTable categoriesTable = new DataTable();
                categoriesTable.Columns.Add("Id", typeof(int));

                string[] categoriesArray = categories.Split(',');
                foreach (string categoryItem in categoriesArray)
                {
                    categoriesTable.Rows.Add(DataHelper.intParse(categoryItem));
                }

                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@Categories", Value = categoriesTable, SqlDbType = SqlDbType.Structured, TypeName = "Ids" },
                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId },
                    new SqlParameter { ParameterName = "@Page", Value = Page }
                };
                DataTableCollection dtbls = DatabaseObject.FetchFromSP("MobileLibraryByFilters", param, ref response);

                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return null;
                }

                dynamic result = new List<object>();
                dynamic recent_watch = new List<object>();

                int i = 0;

                foreach (DataRow row in dtbls[0].Rows)
                {
                    result.Add(VideoParsing(row));

                    i++;
                }


                foreach (DataRow row in dtbls[2].Rows)
                {
                    recent_watch.Add(VideoParsing(row));
                }

                DataRow dr = dtbls[1].Rows[0];

                var obj = new
                {
                    Paging = new
                    {
                        TotalRecords = DataHelper.longParse(dr["TotalRecords"]),
                        ShowingRecords = DataHelper.longParse(dr["ShowingRecords"]),
                        RemainingRecords = DataHelper.longParse(dr["RemainingRecords"])
                    },
                    RecentWatch = recent_watch,
                    Data = result
                };

                return obj;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public object GetLibraryDetail(int LoginId, long VideoId)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId },
                    new SqlParameter { ParameterName = "@VideoId", Value = VideoId }
                };
                DataTable dt = DatabaseObject.FetchTableFromSP("MobileLibraryDetail", param, ref response);

                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return null;
                }

                if (!DataHelper.HasRows(dt))
                {
                    return null;
                }

                DataRow row = dt.Rows[0];

                dynamic result = VideoParsing(row, true);

                // Add Video to User Watch List
                AddVideoToWatchList(LoginId, VideoId);

                return result;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public object GetLibraryRelated(int LoginId, long VideoId)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId },
                    new SqlParameter { ParameterName = "@VideoId", Value = VideoId }
                };
                DataTable dt = DatabaseObject.FetchTableFromSP("MobileLibraryRelated", param, ref response);

                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return null;
                }

                dynamic result = new List<object>();

                foreach (DataRow row in dt.Rows)
                {
                    result.Add(VideoParsing(row));
                }

                var obj = new
                {
                    Data = result
                };

                return obj;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public object GetLiveStreaming(int LoginId)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param = { new SqlParameter { ParameterName = "@Loginid", Value = LoginId } };
                DataTable dt = DatabaseObject.FetchTableFromSP("MobileLiveStreaming", param, ref response);

                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return null;
                }

                dynamic result = new List<object>();

                foreach (DataRow row in dt.Rows)
                {
                    result.Add(new
                    {
                        Id = DataHelper.longParse(row["Id"]),
                        Title = row["Title"].ToString(),
                        VideoURL = row["videourl"].ToString(),
                        ImageURL = DataHelper.getLiveStreamingImageURL(row["ImageURL"].ToString()),
                        Description = row["Description"].ToString(),
                        AddedOn = DataHelper.TimeAgo(DataHelper.dateParse(row["AddedOn"])),
                        ChannelName = row["ChannelName"].ToString(),
                        Views = DataHelper.intParse(row["Views"]),
                        Subscribers = DataHelper.FormatNumber(DataHelper.longParse(row["TotalSubscribers"])),
                        Likes = DataHelper.FormatNumber(DataHelper.longParse(row["Likes"])),
                        DisLikes = DataHelper.FormatNumber(DataHelper.longParse(row["DisLikes"])),
                        IsSubscribe = DataHelper.boolParse(row["IsSubscribe"]),
                        IsLike = DataHelper.boolParse(row["IsLike"]),
                        IsDisLike = DataHelper.boolParse(row["IsDisLike"]),
                        URL = "live-tv/" + DataHelper.FriendlyUrlParse(row["Title"].ToString())
                    });
                }
                
                var obj = new
                {
                    Data = result
                };

                return obj;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public object GetPrograms()
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param = null;
                DataTableCollection dtbls = DatabaseObject.FetchFromSP("MobilePrograms", param, ref response);

                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return null;
                }

                dynamic new_arrival = new List<object>();
                dynamic result = new List<object>();
                dynamic featured = new List<object>();


                foreach (DataRow row in dtbls[0].Rows)
                {
                    new_arrival.Add(VideoParsing(row));
                }

                foreach (DataRow row in dtbls[1].Rows)
                {
                    result.Add(new
                    {
                        Id = DataHelper.longParse(row["Id"]),
                        Title = row["Title"].ToString(),
                        ImageURL = UrlEncode(DataHelper.getProgramImageURL(row["ImageURL"].ToString())),
                        Description = row["Description"].ToString()
                    });
                }

                foreach (DataRow row in dtbls[2].Rows)
                {
                    featured.Add(new
                    {
                        Id = DataHelper.longParse(row["Id"]),
                        Title = row["Title"].ToString(),
                        ImageURL = UrlEncode(DataHelper.getProgramImageURL(row["ImageURL"].ToString()))
                    });
                }

                var obj = new
                {
                    FeaturedPrograms = featured,
                    NewArrival = new_arrival,
                    Data = result
                };

                return obj;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public object GetProgramsNewEpisodes()
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param = null;
                DataTableCollection dtbls = DatabaseObject.FetchFromSP("MobileProgramsNewEpisodes", param, ref response);

                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return null;
                }

                dynamic result = new List<object>();


                foreach (DataRow row in dtbls[0].Rows)
                {
                    result.Add(VideoParsing(row));
                }

                var obj = new
                {
                    Data = result
                };

                return obj;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public object GetProgramsDetail(int ProgramId, int Page)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@ProgramId", Value = ProgramId },
                    new SqlParameter { ParameterName = "@Page", Value = Page }
                };
                DataTableCollection dtbls = DatabaseObject.FetchFromSP("MobileProgramDetail", param, ref response);

                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return null;
                }

                dynamic result = new List<object>();
                dynamic new_programs = new List<object>();

                if (!DataHelper.HasRows(dtbls[0]))
                {
                    return false;
                }


                DataRow mRow = dtbls[0].Rows[0];

                dynamic program_detail = new
                {
                    Id = DataHelper.intParse(mRow["Id"]),
                    Title = mRow["Title"].ToString(),
                    ImageURL = DataHelper.getProgramImageURL(mRow["ImageURL"].ToString()),
                    Description = mRow["Description"].ToString(),
                    UserId = DataHelper.intParse(mRow["UserId"]),
                    UserName = mRow["UserName"].ToString()
                };

                foreach (DataRow row in dtbls[1].Rows)
                {
                    result.Add(VideoParsing(row));
                }

                DataRow dr = dtbls[2].Rows[0];

                foreach (DataRow row in dtbls[3].Rows)
                {
                    new_programs = new
                    {
                        Id = DataHelper.intParse(row["Id"]),
                        Title = row["Title"].ToString(),
                        ImageURL = DataHelper.getProgramImageURL(row["ImageURL"].ToString())
                    };
                }


                var obj = new
                {
                    ProgramDetail = program_detail,
                    Episodes = result,
                    Paging = new
                    {
                        TotalRecords = DataHelper.longParse(dr["TotalRecords"]),
                        ShowingRecords = DataHelper.longParse(dr["ShowingRecords"]),
                        RemainingRecords = DataHelper.longParse(dr["RemainingRecords"])
                    },
                    NewPrograms = new_programs
                };

                return obj;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public object GetSearchResult(string Keyword, int Page)
        {
            try
            {
                if (Keyword.Trim().Length > 0)
                    Keyword = "%" + Keyword.Trim() + "%";

                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@Keyword", Value = Keyword },
                    new SqlParameter { ParameterName = "@Page", Value = Page }
                };
                DataTableCollection dtbls = DatabaseObject.FetchFromSP("MobileSearch", param, ref response);

                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return null;
                }

                dynamic result = new List<object>();

                foreach (DataRow row in dtbls[0].Rows)
                {
                    result.Add(VideoParsing(row));
                }

                DataRow dr = dtbls[1].Rows[0];

                var obj = new
                {
                    Paging = new
                    {
                        TotalRecords = DataHelper.longParse(dr["TotalRecords"]),
                        ShowingRecords = DataHelper.longParse(dr["ShowingRecords"]),
                        RemainingRecords = DataHelper.longParse(dr["RemainingRecords"])
                    },
                    Data = result,
                };

                return obj;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public object ChangePassword(int LoginId, string UserId, string CurrentPassword, string NewPassword)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId },
                    new SqlParameter { ParameterName = "@CurrentPassword", Value = SBEncryption.getMD5Password(UserId, CurrentPassword) },
                    new SqlParameter { ParameterName = "@NewPassword", Value = SBEncryption.getMD5Password(UserId, NewPassword) }
                };

                string Result = DatabaseObject.DLookupDB("MobileChangePassword", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return response.ErrorList[0].Message;
                }

                if (Result == "True")
                    return true;
                else
                    return false;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return ae.Message;
            }
        }

        public object GetProfileInfo(int LoginId)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId }
                };

                DataTable dt = DatabaseObject.FetchTableFromSP("MobileGetProfile", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return response.ErrorList[0].Message;
                }

                if (DataHelper.HasRows(dt))
                {
                    DataRow row = dt.Rows[0];

                    dynamic result = new
                    {
                        UserId = row["UserId"].ToString(),
                        UserName = row["UserName"].ToString(),
                        PhoneNo = row["PhoneNo"].ToString(),
                        //DateOfBirth = (string.IsNullOrEmpty(row["DateOfBirth"].ToString()) ? DataHelper.dateParse(row["DateOfBirth"]).ToString("yyyy-MM-dd") : ""),
                        DateOfBirth = DataHelper.dateParse(row["DateOfBirth"]).ToString("yyyy-MM-dd"),
                        CountryId = DataHelper.intParse(row["CountryId"]),
                        StateId = DataHelper.intParse(row["StateId"]),
                        CityId = DataHelper.intParse(row["CityId"]),
                        ProfilePicture = (row["ProfilePicture"].ToString() == "" ? "" : "content/uploads/users/" + UrlEncode(row["ProfilePicture"].ToString()))
                    };

                    return result;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public object UpdateProfile(int LoginId, string FullName, string Email, string Phone, DateTime DateOfBirth, int CountryId, int StateId, int CityId, 
            System.Web.HttpFileCollection files)
        {
            try
            {
                string ProfilePicture = "";
                if (files.Count > 0)
                {
                    ProfilePicture = DateTime.Now.Ticks + "_" + files[0].FileName;
                    files[0].SaveAs(System.Web.HttpContext.Current.Server.MapPath("~/content/uploads/users/" + ProfilePicture));
                }

                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@UserName", Value = FullName },
                    new SqlParameter { ParameterName = "@PhoneNo", Value = Phone },
                    new SqlParameter { ParameterName = "@DateOfBirth", Value = DateOfBirth },
                    new SqlParameter { ParameterName = "@CountryId", Value = CountryId },
                    new SqlParameter { ParameterName = "@StateId", Value = StateId },
                    new SqlParameter { ParameterName = "@CityId", Value = CityId },
                    new SqlParameter { ParameterName = "@ProfilePicture", Value = ProfilePicture },
                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId }
                };
                DatabaseObject.ExecuteSP("MobileUpdateProfile", param, ref response);

                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return null;
                }

                return true;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public object GetRegionsCountryStates()
        {
            try
            {
                ErrorResponse response = new BusinessLogic.ErrorResponse();
                SqlParameter[] param = null;
                DataTable dt = DatabaseObject.FetchTableFromSP("MobileGetRegions_Country_States", param, ref response);

                if (response.Error)
                {
                    return null;
                }

                dynamic states = new List<object>();
                dynamic countries = new List<object>();

                int pCountryId = 0;
                string pCountryName = "";

                foreach (DataRow row in dt.Rows)
                {
                    states.Add(new
                    {
                        Id = DataHelper.intParse(row["StateId"]),
                        Name = row["StateName"].ToString()
                    });

                    if (pCountryId != DataHelper.intParse(row["CountryId"]) && pCountryId > 0)
                    {
                        countries.Add(new
                        {
                            Id = pCountryId,
                            Name = pCountryName,
                            States = states
                        });

                        states = new List<object>();
                    }


                    pCountryId = DataHelper.intParse(row["CountryId"]);
                    pCountryName = row["CountryName"].ToString();
                }


                return countries;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public object GetRegionsCitiesByState(int StateId)
        {
            try
            {
                ErrorResponse response = new BusinessLogic.ErrorResponse();
                SqlParameter[] param = 
                {
                    new SqlParameter { ParameterName = "@StateId", Value = StateId }
                };
                DataTable dt = DatabaseObject.FetchTableFromSP("MobileGetRegions_Cities", param, ref response);

                if (response.Error)
                {
                    return null;
                }

                dynamic cities = new List<object>();

                foreach (DataRow row in dt.Rows)
                {
                    cities.Add(new
                    {
                        Id = DataHelper.intParse(row["CityId"]),
                        Name = row["CityName"].ToString()
                    });
                }


                return cities;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public object ValidateSignUpEmailAndMobile(string email, string phone)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@UserId", Value = email },
                    new SqlParameter { ParameterName = "@Phone", Value = phone }
                };

                DataTable dt = DatabaseObject.FetchTableFromSP("Mobile_ValidateEmailAndPhone", param, ref response);
                if (response.Error)
                {
                    return false;
                }
                else
                {
                    bool EmailExists = false, PhoneExists = false;

                    if (DataHelper.HasRows(dt))
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            if (row["UserId"].ToString() == email)
                            {
                                EmailExists = true;
                            }

                            //if (row["PhoneNo"].ToString() == phone)
                            //{
                            //    PhoneExists = true;
                            //}
                        }
                    }

                    return new { EmailExists = EmailExists, PhoneExists = PhoneExists };
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return false;
            }
        }

        public object GetSubscriptionPlan(int LoginId)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId }
                };

                DataTable dt = DatabaseObject.FetchTableFromSP("GetSubscriptionPlan", param, ref response);
                if (response.Error)
                {
                    return false;
                }
                else
                {
                    DataRow row = dt.Rows[0];
                    return new { MonthlyRate = DataHelper.intParse(row["MonthlyRate"]), YearlyRate = DataHelper.intParse(row["YearlyRate"]), Currency = row["Currency"].ToString(), SubscriptionType = row["SubType"].ToString(), SubscriptionStatus = row["SubscriptionStatus"].ToString() };
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return false;
            }
        }

        public bool CheckForSubscription(int LoginId)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter() { ParameterName = "@LoginId", Value = LoginId }
                };
                DataTable dt = DatabaseObject.FetchTableFromSP("Mobile_CheckForSubscription", param, ref response);
                if (response.Error)
                {
                    return false;
                }
                else
                {
                    return DataHelper.HasRows(dt);
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return false;
            }
        }

        public bool AddVideoToMyList(int LoginId, long VideoId)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId },
                    new SqlParameter { ParameterName = "@VideoId", Value = VideoId }
                };

                DatabaseObject.ExecuteSP("Mobile_AddToMyList", param, ref response);
                if (response.Error)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return false;
            }
        }

        public bool RemoveVideoFromMyList(int LoginId, long VideoId)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId },
                    new SqlParameter { ParameterName = "@VideoId", Value = VideoId }
                };

                DatabaseObject.ExecuteSP("Mobile_RemoveFromMyList", param, ref response);
                if (response.Error)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return false;
            }
        }

        public object GetMyListLibrary(int LoginId, int Page)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId },
                    new SqlParameter { ParameterName = "@Page", Value = Page }
                };
                DataTableCollection dtbls = DatabaseObject.FetchFromSP("MobileLibraryMyList", param, ref response);

                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return null;
                }

                dynamic result = new List<object>();

                int i = 0;

                foreach (DataRow row in dtbls[0].Rows)
                {
                    result.Add(VideoParsing(row));

                    i++;
                }

                DataRow dr = dtbls[1].Rows[0];

                var obj = new
                {
                    Paging = new
                    {
                        TotalRecords = DataHelper.longParse(dr["TotalRecords"]),
                        ShowingRecords = DataHelper.longParse(dr["ShowingRecords"]),
                        RemainingRecords = DataHelper.longParse(dr["RemainingRecords"])
                    },
                    Data = result
                };

                return obj;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public object UploadVideo(int LoginId, string Title, int ProgramId, int CategoryId, string Description, System.Web.HttpFileCollection files)
        {
            try
            {
                string ImageURL = "", VideoURL = "";

                if (files.Count > 0)
                {
                    VideoURL = DateTime.Now.Ticks + "_" + files[0].FileName;
                    files[0].SaveAs(System.Web.HttpContext.Current.Server.MapPath("~/content/uploads/videos/" + VideoURL));


                    if (files.Count > 1)
                    {
                        ImageURL = DateTime.Now.Ticks + "_" + files[1].FileName;
                        files[1].SaveAs(System.Web.HttpContext.Current.Server.MapPath("~/content/uploads/videos/thumb/" + ImageURL));
                    }
                }

                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId },
                    new SqlParameter { ParameterName = "@Id", Value = 0 },
                    new SqlParameter { ParameterName = "@Title", Value = Title },
                    new SqlParameter { ParameterName = "@ProgramId", Value = ProgramId },
                    new SqlParameter { ParameterName = "@CategoryId", Value = CategoryId },
                    new SqlParameter { ParameterName = "@ImageURL", Value = ImageURL },
                    new SqlParameter { ParameterName = "@VideoURL", Value = VideoURL },
                    new SqlParameter { ParameterName = "@Description", Value = Description }
                };
                DatabaseObject.ExecuteSP("Mobile_UploadVideo", param, ref response);

                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return null;
                }

                return true;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public object EditVideo(int LoginId, int VideoId, string Title, int ProgramId, int CategoryId, string Description, System.Web.HttpFileCollection files)
        {
            try
            {
                string ImageURL = "", VideoURL = "";

                if (files.Count > 0)
                {
                    if (files[0].FileName != "")
                    {
                        ImageURL = DateTime.Now.Ticks + "_" + files[0].FileName;
                        files[0].SaveAs(System.Web.HttpContext.Current.Server.MapPath("~/content/uploads/videos/thumb/" + ImageURL));
                    }
                }

                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId },
                    new SqlParameter { ParameterName = "@Id", Value = VideoId },
                    new SqlParameter { ParameterName = "@Title", Value = Title },
                    new SqlParameter { ParameterName = "@ProgramId", Value = ProgramId },
                    new SqlParameter { ParameterName = "@CategoryId", Value = CategoryId },
                    new SqlParameter { ParameterName = "@ImageURL", Value = ImageURL },
                    new SqlParameter { ParameterName = "@VideoURL", Value = VideoURL },
                    new SqlParameter { ParameterName = "@Description", Value = Description }
                };
                DatabaseObject.ExecuteSP("Mobile_UploadVideo", param, ref response);

                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return null;
                }

                return true;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public object GetMyVideos(int LoginId, int Page)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId },
                    new SqlParameter { ParameterName = "@Page", Value = Page }
                };
                DataTableCollection dtbls = DatabaseObject.FetchFromSP("Mobile_MyVideos", param, ref response);

                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return null;
                }

                dynamic result = new List<object>();

                int i = 0;

                foreach (DataRow row in dtbls[0].Rows)
                {
                    result.Add(new
                    {
                        Id = DataHelper.longParse(row["Id"]),
                        Title = row["Title"].ToString(),
                        UserName = row["UserName"].ToString(),
                        Views = DataHelper.intParse(row["Views"]),
                        Likes = DataHelper.intParse(row["Likes"]),
                        DisLikes = DataHelper.intParse(row["DisLikes"]),
                        AddedOn = DataHelper.TimeAgo(DataHelper.dateParse(row["AddedOn"])),
                        ImageURL = DataHelper.getVideoImageURLMobile(row["ImageURL"].ToString()),
                        Status = row["CategoryName"].ToString()
                    });

                    i++;
                }

                DataRow dr = dtbls[1].Rows[0];

                var obj = new
                {
                    Paging = new
                    {
                        TotalRecords = DataHelper.longParse(dr["TotalRecords"]),
                        ShowingRecords = DataHelper.longParse(dr["ShowingRecords"]),
                        RemainingRecords = DataHelper.longParse(dr["RemainingRecords"])
                    },
                    Data = result
                };

                return obj;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public object GetMyVideosDetail(int LoginId, long VideoId)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId },
                    new SqlParameter { ParameterName = "@VideoId", Value = VideoId }
                };
                DataTable dt = DatabaseObject.FetchTableFromSP("MobileMyVideoDetail", param, ref response);

                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return null;
                }

                if (!DataHelper.HasRows(dt))
                {
                    return null;
                }

                DataRow row = dt.Rows[0];

                return new
                {
                    Id = DataHelper.longParse(row["Id"]),
                    Title = row["Title"].ToString(),
                    UserName = row["UserName"].ToString(),
                    Views = DataHelper.intParse(row["Views"]),
                    Likes = DataHelper.intParse(row["Likes"]),
                    DisLikes = DataHelper.intParse(row["DisLikes"]),
                    AddedOn = DataHelper.TimeAgo(DataHelper.dateParse(row["AddedOn"])),
                    ImageURL = DataHelper.getVideoImageURLMobile(row["ImageURL"].ToString()),
                    Description = row["Description"].ToString(),
                    VideoURL = "content/uploads/videos/" + UrlEncode(row["VideoURL"].ToString()),
                    CategoryId = DataHelper.intParse(row["CategoryId"]),
                    CategoryName = row["CategoryName"].ToString(),
                    ProgramId = DataHelper.intParse(row["ProgramId"]),
                    ProgramName = row["ProgramName"].ToString(),
                    Status = row["Status"].ToString(),
                    Notes = row["Notes"].ToString()
                };
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public bool RemoveMyVideo(int LoginId, long VideoId)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId },
                    new SqlParameter { ParameterName = "@VideoId", Value = VideoId }
                };

                DatabaseObject.ExecuteSP("Mobile_DeleteMyVideo", param, ref response);
                if (response.Error)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return false;
            }
        }

        public bool SendUserMessage(int LoginId, long VideoId, string Message)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId },
                    new SqlParameter { ParameterName = "@VideoId", Value = VideoId },
                    new SqlParameter { ParameterName = "@Message", Value = Message }
                };

                DatabaseObject.ExecuteSP("Mobile_SendMessage", param, ref response);
                if (response.Error)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return false;
            }
        }

        public void AddVideoToWatchList(int LoginId, long VideoId)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId },
                    new SqlParameter { ParameterName = "@VideoId", Value = VideoId }
                };

                DatabaseObject.ExecuteSP("AddVideoToWatch", param, ref response);
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
            }
        }

        public object GetProgramsByUser(int LoginId)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@UserId", Value = LoginId },
                };
                DataTableCollection dtbls = DatabaseObject.FetchFromSP("MobileProgramsByUser", param, ref response);

                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return null;
                }

                dynamic result = new List<object>();

                foreach (DataRow row in dtbls[0].Rows)
                {
                    result.Add(new
                    {
                        Id = DataHelper.longParse(row["Id"]),
                        Title = row["Title"].ToString(),
                        ImageURL = UrlEncode(DataHelper.getProgramImageURL(row["ImageURL"].ToString())),
                        Description = row["Description"].ToString(),
                    });
                }

                var obj = new
                {
                    Data = result
                };

                return obj;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public object GetVideosByUser(int LoginId, int Page)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId },
                    new SqlParameter { ParameterName = "@Page", Value = Page }
                };
                DataTableCollection dtbls = DatabaseObject.FetchFromSP("MobileVideosByUser", param, ref response);

                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return null;
                }

                dynamic result = new List<object>();

                int i = 0;

                foreach (DataRow row in dtbls[0].Rows)
                {
                    result.Add(VideoParsing(row));
                    i++;
                }
                

                DataRow dr = dtbls[1].Rows[0];

                var obj = new
                {
                    Paging = new
                    {
                        TotalRecords = DataHelper.longParse(dr["TotalRecords"]),
                        ShowingRecords = DataHelper.longParse(dr["ShowingRecords"]),
                        RemainingRecords = DataHelper.longParse(dr["RemainingRecords"])
                    },
                    Data = result
                };

                return obj;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }


        private string UrlEncode(string url)
        {
            return System.Web.HttpUtility.UrlPathEncode(url);
        }

        private object VideoParsing(DataRow row, bool IsDetail = false)
        {
            
            int UserId = 0;
            string Subscribers = "0";
            bool IsSubscribe = false, IsLike = false, IsDisLike = false;


            if (row.Table.Columns.Contains("UserId"))
                UserId = DataHelper.intParse(row["UserId"]);

            if (row.Table.Columns.Contains("IsSubscribe"))
                IsSubscribe = DataHelper.boolParse(row["IsSubscribe"]);

            if (row.Table.Columns.Contains("IsLike"))
                IsLike = DataHelper.boolParse(row["IsLike"]);

            if (row.Table.Columns.Contains("IsDisLike"))
                IsDisLike = DataHelper.boolParse(row["IsDisLike"]);

            if (row.Table.Columns.Contains("TotalSubscribers"))
                Subscribers = DataHelper.FormatNumber(DataHelper.longParse(row["TotalSubscribers"]));

            if (IsDetail)
            {
                return new
                {
                    Id = DataHelper.longParse(row["Id"]),
                    Title = row["Title"].ToString(),
                    UserId = UserId,
                    UserName = row["UserName"].ToString(),
                    Views = DataHelper.intParse(row["Views"]),
                    Likes = DataHelper.intParse(row["Likes"]),
                    DisLikes = DataHelper.intParse(row["DisLikes"]),
                    AddedOn = DataHelper.TimeAgo(DataHelper.dateParse(row["AddedOn"])),
                    ImageURL = DataHelper.getVideoImageURLMobile(row["ImageURL"].ToString()),
                    Description = row["Description"].ToString(),
                    VideoURL = "content/uploads/videos/" + UrlEncode(row["VideoURL"].ToString()),
                    CategoryId = DataHelper.intParse(row["CategoryId"]),
                    CategoryName = row["CategoryName"].ToString(),
                    ProgramId = DataHelper.intParse(row["ProgramId"]),
                    ProgramName = row["ProgramName"].ToString(),
                    IsAddToList = DataHelper.intParse(row["IsAddToList"]),
                    Subscribers = Subscribers,
                    IsSubscribe = IsSubscribe,
                    IsLike = IsLike,
                    IdDisLike = IsDisLike,
                    URL = "watch?v=" + SBEncryption.Encrypt(DataHelper.stringParse(row["Id"]))
                };
            }
            else
            {
                if (row["Description"] == null)
                {
                    int programId = 0;
                    string programName = "";


                    if (row["ProgramId"] != null)
                        programId = DataHelper.intParse(row["ProgramId"]);

                    if (row["ProgramName"] != null)
                        programName = row["ProgramName"].ToString();

                    return new
                    {
                        Id = DataHelper.longParse(row["Id"]),
                        Title = row["Title"].ToString(),
                        UserId = UserId,
                        UserName = row["UserName"].ToString(),
                        Views = DataHelper.intParse(row["Views"]),
                        Likes = DataHelper.intParse(row["Likes"]),
                        DisLikes = DataHelper.intParse(row["DisLikes"]),
                        AddedOn = DataHelper.TimeAgo(DataHelper.dateParse(row["AddedOn"])),
                        ImageURL = DataHelper.getVideoImageURLMobile(row["ImageURL"].ToString()),
                        CategoryId = DataHelper.intParse(row["CategoryId"]),
                        CategoryName = row["CategoryName"].ToString(),
                        ProgramId = programId,
                        ProgramName = programName,
                        Subscribers = Subscribers,
                        IsSubscribe = IsSubscribe,
                        IsLike = IsLike,
                        IdDisLike = IsDisLike,
                        URL = "watch?v=" + SBEncryption.Encrypt(DataHelper.stringParse(row["Id"]))
                    };
                }
                else
                {
                    return new
                    {
                        Id = DataHelper.longParse(row["Id"]),
                        Title = row["Title"].ToString(),
                        UserId = UserId,
                        UserName = row["UserName"].ToString(),
                        Views = DataHelper.intParse(row["Views"]),
                        Likes = DataHelper.intParse(row["Likes"]),
                        DisLikes = DataHelper.intParse(row["DisLikes"]),
                        AddedOn = DataHelper.TimeAgo(DataHelper.dateParse(row["AddedOn"])),
                        ImageURL = DataHelper.getVideoImageURLMobile(row["ImageURL"].ToString()),
                        Description = row["Description"].ToString(),
                        VideoURL = "content/uploads/videos/" + UrlEncode(row["VideoURL"].ToString()),
                        CategoryId = DataHelper.intParse(row["CategoryId"]),
                        CategoryName = row["CategoryName"].ToString(),
                        ProgramId = DataHelper.intParse(row["ProgramId"]),
                        ProgramName = row["ProgramName"].ToString(),
                        Subscribers = Subscribers,
                        IsSubscribe = IsSubscribe,
                        IsLike = IsLike,
                        IdDisLike = IsDisLike,
                        URL = "watch?v=" + SBEncryption.Encrypt(DataHelper.stringParse(row["Id"]))
                    };
                }
            }
        }


        public object GetSubscriptionStatus(int login_id)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@LoginId", Value = login_id }
                };

                DataTable dt = DatabaseObject.FetchTableFromSP("GetSubscriptionStatus", param, ref response);
                if (response.Error)
                {
                    return false;
                }
                else
                {
                    DataRow row = dt.Rows[0];
                    return new { SubscriptionStatus = row["SubscriptionStatus"].ToString(), SubscriptionType = row["SubscriptionType"].ToString() };
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return false;
            }
        }

        public object GetMySubscription(int login_id)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@LoginId", Value = login_id }
                };

                DataTable dt = DatabaseObject.FetchTableFromSP("GetSubscriptionPlanByCustomer", param, ref response);
                if (response.Error)
                {
                    return false;
                }
                else
                {
                    DataRow row = dt.Rows[0];
                    return new { SubscriptionType = row["SubscriptionType"].ToString(), SubscriptionStatus = row["SubscriptionStatus"].ToString(),
                                 SubscriptionPlan = row["SubscriptionPlan"].ToString(), StartDate = DataHelper.dateParse(row["SubscriptionStart"]).ToString("yyyy-MM-dd"),
                                 ExpireOn = DataHelper.dateParse(row["SubscriptionEnd"]).ToString("yyyy-MM-dd"), DaysLeft = DataHelper.intParse(row["DaysLeft"]),
                                 SubscriptionAmount = DataHelper.decimalParse(row["SubscriptionAmount"]), SubscriptionCurrency = row["SubscriptionCurrency"].ToString()
                    };
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return false;
            }
        }


        public bool ChangeSubscriptionStatus(int LoginId, int Status)
        {
            try
            {

                string storedProcedure = "SubscriptionPlanCancel";
                if (Status == 1)
                    storedProcedure = "SubscriptionPlanReActive";

                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@CustomerId", Value = LoginId }
                };

                DatabaseObject.ExecuteSP(storedProcedure, param, ref response);
                if (response.Error)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return false;
            }
        }

        public bool UpdateViewsCounts(long VideoId, bool IsLiveStreaming)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@VideoId", Value = VideoId },
                    new SqlParameter { ParameterName = "@IsLiveStreaming", Value = IsLiveStreaming == true ? 1 : 0 }
                };

                DatabaseObject.ExecuteSP("MobileUpdateViewsCount", param, ref response);
                if (response.Error)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return false;
            }
        }

        public bool AddClaimVideo(int LoginId, long VideoId, string Description)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId },
                    new SqlParameter { ParameterName = "@VideoId", Value = VideoId },
                    new SqlParameter { ParameterName = "@Description", Value = Description }
                };

                DatabaseObject.ExecuteSP("Mobile_AddToMyList", param, ref response);
                if (response.Error)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return false;
            }
        }

        public object GetStripeCustomerId(int login_id, decimal amount, string currency)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@LoginId", Value = login_id }
                };

                DataTable dt = DatabaseObject.FetchTableFromSP("GetSubscriptionPlanByCustomer", param, ref response);
                if (response.Error)
                {
                    return false;
                }
                else
                {
                    DataRow row = dt.Rows[0];
                    return new
                    {
                        SubscriptionType = row["SubscriptionType"].ToString(),
                        SubscriptionStatus = row["SubscriptionStatus"].ToString(),
                        SubscriptionPlan = row["SubscriptionPlan"].ToString(),
                        StartDate = DataHelper.dateParse(row["SubscriptionStart"]).ToString("yyyy-MM-dd"),
                        ExpireOn = DataHelper.dateParse(row["SubscriptionEnd"]).ToString("yyyy-MM-dd"),
                        DaysLeft = DataHelper.intParse(row["DaysLeft"]),
                        SubscriptionAmount = DataHelper.decimalParse(row["SubscriptionAmount"]),
                        SubscriptionCurrency = row["SubscriptionCurrency"].ToString()
                    };
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return false;
            }
        }


        public object SavePaymentResponse(int login_id, dynamic json)
        {
            try
            {

                decimal stripe_amount = DataHelper.decimalParse(json["amount"]);
                string customer_id = json["customerId"];
                string subscription_plan = json["subscriptionPlan"];
                string client_secret = json["clientSecret"];
                string stripe_currency = json["currency"];
                string payment_intent_id = json["id"];
                string payment_method = "";
                string payment_status = json["status"];

                string error_code = "";
                string error_message = "";
                string decline_code = "";
                string charge = "";

                payment_status = payment_status.ToLower();

                if (payment_status != "succeeded")
                {
                    error_code = json["lastPaymentError"]["code"];
                    error_message = json["lastPaymentError"]["message"];
                    decline_code = json["lastPaymentError"]["type"];
                    payment_method = json["lastPaymentError"]["paymentMethod"]["id"];
                }

                //stripe_amount = (stripe_amount / 100); // for stripe functionality

                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@StripeCustomerId", Value = customer_id },
                    new SqlParameter { ParameterName = "@StripeAmount", Value = stripe_amount },
                    new SqlParameter { ParameterName = "@ClientSecret", Value = client_secret },
                    new SqlParameter { ParameterName = "@StripeCurrency", Value = stripe_currency },
                    new SqlParameter { ParameterName = "@PaymentIntentId", Value = payment_intent_id },
                    new SqlParameter { ParameterName = "@PaymentMethod", Value = payment_method },
                    new SqlParameter { ParameterName = "@PaymentStatus", Value = payment_status },
                    new SqlParameter { ParameterName = "@ErrorCode", Value = error_code },
                    new SqlParameter { ParameterName = "@ErrorMessage", Value = error_message },
                    new SqlParameter { ParameterName = "@DeclineCode", Value = decline_code },
                    new SqlParameter { ParameterName = "@Charge", Value = charge },
                    new SqlParameter { ParameterName = "@LoginId", Value = login_id },
                    new SqlParameter { ParameterName = "@SubscriptionPlan", Value = subscription_plan },
                    new SqlParameter { ParameterName = "@StartDate", Value = DBNull.Value }
                };

                DataTable dt = DatabaseObject.FetchTableFromSP("PaymentSubscription", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return "999";
                }
                else
                {
                    string ErrorCode = dt.Rows[0]["ErrorCode"].ToString();

                    if (ErrorCode == "000")
                    {
                        long PaymentId = DataHelper.longParse(dt.Rows[0]["PaymentId"]);
                        string receipt_no = PaymentId.ToString("000000");

                        StringBuilder sbHtml = new StringBuilder();
                        var path = System.Web.HttpContext.Current.Server.MapPath("~/content/emails/payment-success.html");
                        sbHtml.AppendLine(System.IO.File.ReadAllText(path));

                        sbHtml = sbHtml.Replace("{NAME}", clsWebSession.UserName);
                        sbHtml = sbHtml.Replace("{CURRENCY}", stripe_currency.ToUpper());
                        sbHtml = sbHtml.Replace("{AMOUNT}", stripe_amount.ToString("#,##0.00"));
                        sbHtml = sbHtml.Replace("{RECEIPT_NO}", receipt_no);

                        bool IsSend = Emails.SendMail(clsWebSession.UserId, "NetFive - Payment Receipt!", sbHtml.ToString(), true);
                    }
                    else
                    {
                        StringBuilder sbHtml = new StringBuilder();
                        var path = System.Web.HttpContext.Current.Server.MapPath("~/content/emails/payment-failed.html");
                        sbHtml.AppendLine(System.IO.File.ReadAllText(path));

                        bool IsSend = Emails.SendMail(clsWebSession.UserId, "NetFive - Payment failed!", sbHtml.ToString(), true);
                    }

                    return dt.Rows[0]["ErrorCode"].ToString();
                }


            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return "999";
            }
        }
    }
}
