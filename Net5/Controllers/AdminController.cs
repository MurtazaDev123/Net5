using BusinessLogic;
using SmartEcommerce.Controllers.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartEcommerce.Controllers
{
    [AdminAuthorization(Roles = "admin", Exemption = new string[] { "login", "loginaccount", "index", "statesbycountry", "citiesbystate" })]
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            if (BusinessLogic.clsCookie.Exists)
            {
                if (clsSession.LoginType == 1)
                    return RedirectToRoute("admin-dashboard");
                else if (clsSession.LoginType == 2)
                    return RedirectToRoute("partner-dashboard");
                else
                    return View();
            }
            else
            {
                return RedirectToRoute("admin-login");
            }
                
        }

        // GET: Dashboard
        public ActionResult Dashboard()
        {
            //if (clsSession.LoginType == 1)
            //    return RedirectToRoute("admin-dashboard");
            //else if (clsSession.LoginType == 2)
            //    return RedirectToRoute("partner-dashboard");
            //else

            return View();
        }

        public JsonResult DashboardData()
        {
            var result = new Accounts().GetDashboardData();
            if (result == null)
            {
                return Json(new { success = false });
            }
            else
            {
                return Json(new { success = true, data = result });
            }
        }

        // GET: Account
        public ActionResult Login()
        {
            if (BusinessLogic.clsCookie.Exists)
            {
                if (clsSession.LoginType == 1)
                    return RedirectToRoute("admin-dashboard");
                else if (clsSession.LoginType == 2)
                    return RedirectToRoute("partner-dashboard");
                else
                    return RedirectToRoute("Default");
            }

            return View();
        }

        [HttpPost]
        public JsonResult LoginAccount()
        {
            string UserId = Request.Form["user_id"].ToString();
            string Password = Request.Form["password"].ToString();

            SmartEcommerce.Models.Admin.Logins user = new BusinessLogic.Accounts().ValidateUser(UserId, Password);

            if (user != null)
            {
                if (BusinessLogic.clsSession.useSession)
                {
                    Session[BusinessLogic.Sessions.LoginId] = user.LoginId;
                    Session[BusinessLogic.Sessions.UserId] = user.UserId;
                    Session[BusinessLogic.Sessions.UserName] = user.UserName;
                    Session[BusinessLogic.Sessions.LoginType] = user.LoginType;

                    return Json(new { Success = 1, Info = user });
                }
                else
                {
                    HttpCookie userCookie = new HttpCookie(BusinessLogic.UserInfoCookie.scookiename);
                    userCookie.Expires = DateTime.Now.AddDays(3);
                    userCookie[BusinessLogic.UserInfoCookie.sLoginId] = BusinessLogic.SBEncryption.Encrypt(user.LoginId.ToString());
                    userCookie[BusinessLogic.UserInfoCookie.sUserId] = BusinessLogic.SBEncryption.Encrypt(user.UserId);
                    userCookie[BusinessLogic.UserInfoCookie.sUserName] = BusinessLogic.SBEncryption.Encrypt(user.UserName);
                    userCookie[BusinessLogic.UserInfoCookie.sloginType] = BusinessLogic.SBEncryption.Encrypt(user.LoginType.ToString());
                    userCookie[BusinessLogic.UserInfoCookie.sPartnerCategoryId] = BusinessLogic.SBEncryption.Encrypt(user.PartnerCategoryId.ToString());
                    //Response.Cookies.Add(userCookie);
                    System.Web.HttpContext.Current.Response.Cookies.Add(userCookie);

                    return Json(new { Success = 1, Info = user });
                }
            }
            else
            {
                return Json(new { Success = 0 });
            }
        }

        public ActionResult Logout()
        {
            BusinessLogic.clsCookie.Logout();

            if (BusinessLogic.clsSession.LoginType == 1)
                return RedirectToRoute("admin-login");
            else
                return RedirectToRoute("partner-login");
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        public JsonResult ChangePasswordRequest()
        {
            string OldPassword = Request.Form["old_password"].ToString();
            string NewPassword = Request.Form["new_password"].ToString();

            var result = new Accounts().ChangeAdminPassword(BusinessLogic.clsSession.LoginId, BusinessLogic.clsSession.UserId, OldPassword, NewPassword);

            return Json(result);
        }
        
        #region Country

        public ActionResult Country()
        {
            return View();
        }

        public JsonResult CurrentCountries()
        {
            List<SmartEcommerce.Models.Common.Country> countries = new Common().GetCountryByStatus(Status.Current);
            return Json(new { data = countries });
        }

        public JsonResult ArchiveCountry()
        {
            List<SmartEcommerce.Models.Common.Country> countries = new Common().GetCountryByStatus(Status.Archive);
            return Json(new { data = countries });
        }

        public JsonResult SaveCountry()
        {
            try
            {
                int Id = DataHelper.intParse(Request.Form["Id"]);
                string Code = Request.Form["Code"].ToString();
                string FullName = Request.Form["FullName"].ToString();
                string Masking = Request.Form["Masking"].ToString();
                string Currency = Request.Form["Currency"].ToString();
                bool Active = DataHelper.boolParse(Request.Form["active"]);
                int entryLevel = 0;

                if (new Common().SaveCountry(clsSession.LoginId, Id, Code, FullName, Masking, Currency, Active, out entryLevel))
                {
                    return Json(new { ErrorCode = "000", EntryLevel = entryLevel });
                }
                else
                {
                    return Json(new { ErrorCode = "999" });
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return Json(new { ErrorCode = "999" });
            }
        }

        public JsonResult MoveToArchiveCountry()
        {
            try
            {
                int CountryId = DataHelper.intParse(Request.Form["Id"]);
                int Status = DataHelper.intParse(Request.Form["Status"]);

                if (new Common().ArchiveCountry(clsSession.LoginId, CountryId, Status))
                {
                    if (Status == 2)
                        Session["country_save_msg"] = "Record move to archive successfully!";
                    else if (Status == 1)
                        Session["country_save_msg"] = "Record restore successfully!";

                    return Json(new { ErrorCode = "000" });
                }
                else
                {
                    return Json(new { ErrorCode = "999" });
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return Json(new { ErrorCode = "999" });
            }
        }

        public JsonResult CountryById()
        {
            int Id = DataHelper.intParse(Request.Form["Id"]);

            SmartEcommerce.Models.Common.Country country = new Common().GetCountryById(Id);
            return Json(country);
        }

        public JsonResult SubscriptionCurrency()
        {
            try
            {
                int Id = DataHelper.intParse(Request.Form["Id"].ToString());

                SmartEcommerce.Models.Common.Country currency = new Common().GetCountryCurrency(Id);
                return Json(currency);

            }
            catch (Exception ae)
            {
                return Json(new { ErrorCode = "999", ErrorDescription = ae.Message });
            }
        }

        #endregion Country

        #region State

        public ActionResult State()
        {
            return View();
        }

        public JsonResult CurrentStates()
        {
            List<SmartEcommerce.Models.Common.State> states = new Common().GetStateByStatus(Status.Current);
            return Json(new { data = states });
        }

        public JsonResult CurrentStatesByCountryId(int CountryId)
        {
            List<SmartEcommerce.Models.Common.State> states = new Common().GetStateByCountryId(CountryId);
            return Json(new { data = states });
        }

        public JsonResult ArchiveState()
        {
            List<SmartEcommerce.Models.Common.State> states = new Common().GetStateByStatus(Status.Archive);
            return Json(new { data = states });
        }

        public JsonResult SaveState()
        {
            try
            {
                int Id = DataHelper.intParse(Request.Form["Id"]);
                string Code = Request.Form["Code"].ToString();
                string FullName = Request.Form["FullName"].ToString();
                int CountryId = DataHelper.intParse(Request.Form["CountryId"]);
                bool Active = DataHelper.boolParse(Request.Form["active"]);
                int entryLevel = 0;

                if (new Common().SaveState(clsSession.LoginId, Id, Code, FullName, CountryId, Active, out entryLevel))
                {
                    return Json(new { ErrorCode = "000", EntryLevel = entryLevel });
                }
                else
                {
                    return Json(new { ErrorCode = "999" });
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return Json(new { ErrorCode = "999" });
            }
        }

        public JsonResult MoveToArchiveState()
        {
            try
            {
                int StateId = DataHelper.intParse(Request.Form["Id"]);
                int Status = DataHelper.intParse(Request.Form["Status"]);

                if (new Common().ArchiveState(clsSession.LoginId, StateId, Status))
                {
                    if (Status == 2)
                        Session["store_save_msg"] = "Record move to archive successfully!";
                    else if (Status == 1)
                        Session["store_save_msg"] = "Record restore successfully!";

                    return Json(new { ErrorCode = "000" });
                }
                else
                {
                    return Json(new { ErrorCode = "999" });
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return Json(new { ErrorCode = "999" });
            }
        }

        public JsonResult StateById()
        {
            int Id = DataHelper.intParse(Request.Form["Id"]);

            SmartEcommerce.Models.Common.State state = new Common().GetStateById(Id);
            return Json(state);
        }

        public JsonResult StatesByCountry(int parent_id)
        {
            List<SmartEcommerce.Models.Common.State> states = new Common().GetStateByCountry(parent_id);
            return Json(new { data = states });
        }

        #endregion State

        #region City

        public ActionResult City()
        {
            return View();
        }

        public JsonResult CurrentCities()
        {
            List<SmartEcommerce.Models.Common.City> cities = new Common().GetCityByStatus(Status.Current);
            return Json(new { data = cities });
        }

        public JsonResult CurrentCitiesByCountryStateId(int CountryId, int StateId)
        {
            List<SmartEcommerce.Models.Common.City> cities = new Common().GetCityByCountryStateId(CountryId, StateId);
            return Json(new { data = cities });
        }

        public JsonResult ArchiveCity()
        {
            List<SmartEcommerce.Models.Common.City> cities = new Common().GetCityByStatus(Status.Archive);
            return Json(new { data = cities });
        }

        public JsonResult SaveCity()
        {
            try
            {
                int Id = DataHelper.intParse(Request.Form["Id"]);
                string Code = Request.Form["Code"].ToString();
                string FullName = Request.Form["FullName"].ToString();
                int CountryId = DataHelper.intParse(Request.Form["CountryId"]);
                int StateId = DataHelper.intParse(Request.Form["StateId"]);
                bool Active = DataHelper.boolParse(Request.Form["active"]);
                int entryLevel = 0;

                if (new Common().SaveCity(clsSession.LoginId, Id, Code, FullName, CountryId, StateId, Active, out entryLevel))
                {
                    return Json(new { ErrorCode = "000", EntryLevel = entryLevel });
                }
                else
                {
                    return Json(new { ErrorCode = "999" });
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return Json(new { ErrorCode = "999" });
            }
        }

        public JsonResult MoveToArchiveCity()
        {
            try
            {
                int CityId = DataHelper.intParse(Request.Form["Id"]);
                int Status = DataHelper.intParse(Request.Form["Status"]);

                if (new Common().ArchiveCity(clsSession.LoginId, CityId, Status))
                {
                    if (Status == 2)
                        Session["store_save_msg"] = "Record move to archive successfully!";
                    else if (Status == 1)
                        Session["store_save_msg"] = "Record restore successfully!";

                    return Json(new { ErrorCode = "000" });
                }
                else
                {
                    return Json(new { ErrorCode = "999" });
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return Json(new { ErrorCode = "999" });
            }
        }

        public JsonResult CityById()
        {
            int Id = DataHelper.intParse(Request.Form["Id"]);

            SmartEcommerce.Models.Common.City city = new Common().GetCityById(Id);
            return Json(city);
        }

        public JsonResult CitiesByState(int parent_id)
        {
            List<SmartEcommerce.Models.Common.City> cities = new Common().GetCitiesByState(parent_id);
            return Json(new { data = cities });
        }

        #endregion City

        #region Category

        public ActionResult Categories()
        {
            return View();
        }

        public JsonResult CurrentCategories()
        {
            List<SmartEcommerce.Models.Product.Category> categories = new Products().GetCategoryByStatus(Status.Current);

            return Json(new { data = categories });
        }

        public JsonResult ArchiveCategories()
        {
            List<SmartEcommerce.Models.Product.Category> categories = new Products().GetCategoryByStatus(Status.Archive);
            return Json(new { data = categories });
        }

        public JsonResult SaveCategory()
        {
            try
            {
                int Id = DataHelper.intParse(Request.Form["Id"]);
                string Title = Request.Form["Title"].ToString();
                int Priority = DataHelper.intParse(Request.Form["Priority"]);
                bool Featured = DataHelper.boolParse(Request.Form["Featured"]);
                bool Active = DataHelper.boolParse(Request.Form["active"]);
                string ImageURL = Request.Form["image_name"].ToString();

                int entryLevel = 0;

                if (new Products().SaveCategory(clsSession.LoginId, Id, Title, Featured, Active, ImageURL, Priority, out entryLevel))
                {
                    return Json(new { ErrorCode = "000", EntryLevel = entryLevel });
                }
                else
                {
                    return Json(new { ErrorCode = "999" });
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return Json(new { ErrorCode = "999" });
            }
        }

        public JsonResult MoveToArchiveCategory()
        {
            try
            {
                int CategoryId = DataHelper.intParse(Request.Form["Id"]);
                int Status = DataHelper.intParse(Request.Form["Status"]);

                if (new Products().ArchiveCategory(clsSession.LoginId, CategoryId, Status))
                {
                    if (Status == 2)
                        Session["brand_save_msg"] = "Record move to archive successfully!";
                    else if (Status == 1)
                        Session["brand_save_msg"] = "Record restore successfully!";

                    return Json(new { ErrorCode = "000" });
                }
                else
                {
                    return Json(new { ErrorCode = "999" });
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return Json(new { ErrorCode = "999" });
            }
        }

        public JsonResult CategoryById()
        {
            int Id = DataHelper.intParse(Request.Form["Id"]);

            SmartEcommerce.Models.Product.Category category = new Products().GetCategoryById(Id);
            return Json(category);
        }

        public JsonResult UploadCategoryImage()
        {
            try
            {
                string[] valid_extensions = { "jpeg", "jpg", "png", "gif" };
                string path = Server.MapPath("~/content/uploads/categories/");

                if (Request.Files["image"] != null)
                {
                    string img = Request.Files["image"].FileName.ToString();
                    string extension = img.Substring(img.LastIndexOf('.') + 1);

                    if (!Array.Exists(valid_extensions, e => e.ToLower().Equals(extension)))
                    {
                        // not a valid extension
                        return Json(new { ErrorCode = "002" });
                    }
                    else
                    {
                        string final_image = DateTime.Now.Ticks.ToString() + img.ToLower();
                        path += final_image;

                        Request.Files["image"].SaveAs(path);

                        // Successfully upload file
                        return Json(new { ErrorCode = "000", FileName = final_image });
                    }
                }
                else
                {
                    // File not found
                    return Json(new { ErrorCode = "001" });
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                // Exception 
                return Json(new { ErrorCode = "999" });
            }
        }

        #endregion Category

        #region Live Streaming

        public ActionResult LiveStreamings()
        {
            return View();
        }

        public JsonResult CurrentLiveStreamings()
        {
            List<SmartEcommerce.Models.Product.LiveStreaming> streaming = new Products().GetLiveStreamingByStatus(Status.Current);

            return Json(new { data = streaming });
        }

        public JsonResult ArchiveStreamings()
        {
            List<SmartEcommerce.Models.Product.LiveStreaming> streaming = new Products().GetLiveStreamingByStatus(Status.Archive);
            return Json(new { data = streaming });
        }

        public JsonResult SaveLiveStreaming()
        {
            try
            {
                SmartEcommerce.Models.Product.LiveStreaming streaming = new Models.Product.LiveStreaming();

                streaming.Id = DataHelper.intParse(Request.Form["id"]);
                streaming.Title = Request.Form["title"].ToString();
                streaming.Description = Request.Form["description"].ToString();
                streaming.ImageURL = Request.Form["image_name"].ToString();
                streaming.VideoURL = Request.Form["video_url"].ToString();
                streaming.Active = DataHelper.boolParse(Request.Form["active"]);
                streaming.AddedBy = DataHelper.intParse(Request.Form["added_by"]);
                int entryLevel = 0;

                //if admin upload live tv then select user for added by else save current user loginid
                int AddedBy = clsSession.LoginType == 1 ? streaming.AddedBy : clsSession.LoginId;

                if (new Products().SaveLiveStreaming(clsSession.LoginId, AddedBy, streaming, out entryLevel))
                {
                    return Json(new { ErrorCode = "000", EntryLevel = entryLevel });
                }
                else
                {
                    return Json(new { ErrorCode = "999" });
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return Json(new { ErrorCode = "999" });
            }
        }

        public JsonResult MoveToArchiveLiveStreaming()
        {
            try
            {
                int Id = DataHelper.intParse(Request.Form["Id"]);
                int Status = DataHelper.intParse(Request.Form["Status"]);

                if (new Products().ArchiveLiveStreaming(clsSession.LoginId, Id, Status))
                {
                    if (Status == 2)
                        Session["livestreaming_save_msg"] = "Record move to archive successfully!";
                    else if (Status == 1)
                        Session["livestreaming_save_msg"] = "Record restore successfully!";

                    return Json(new { ErrorCode = "000" });
                }
                else
                {
                    return Json(new { ErrorCode = "999" });
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return Json(new { ErrorCode = "999" });
            }
        }

        public JsonResult LiveStreamingById()
        {
            int Id = DataHelper.intParse(Request.Form["Id"]);

            SmartEcommerce.Models.Product.LiveStreaming streaming = new Products().GetLiveStreamingById(Id);
            return Json(streaming);
        }

        public JsonResult UploadLiveStreamingImage()
        {
            try
            {
                string[] valid_extensions = { "jpeg", "jpg", "png", "gif" };
                string path = Server.MapPath("~/content/uploads/livestreaming/");

                if (Request.Files["image"] != null)
                {
                    string img = Request.Files["image"].FileName.ToString();
                    string extension = img.Substring(img.LastIndexOf('.') + 1);

                    if (!Array.Exists(valid_extensions, e => e.ToLower().Equals(extension)))
                    {
                        // not a valid extension
                        return Json(new { ErrorCode = "002" });
                    }
                    else
                    {
                        string final_image = DateTime.Now.Ticks.ToString() + img.ToLower();
                        path += final_image;

                        Request.Files["image"].SaveAs(path);

                        // Successfully upload file
                        return Json(new { ErrorCode = "000", FileName = final_image });
                    }
                }
                else
                {
                    // File not found
                    return Json(new { ErrorCode = "001" });
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                // Exception 
                return Json(new { ErrorCode = "999" });
            }
        }

        public ActionResult LiveStreamingPending()
        {
            return View();
        }

        public JsonResult PendingLiveStreaming()
        {
            List<SmartEcommerce.Models.Product.LiveStreaming> streaming = new Products().GetLiveStreamingByStatus(Status.Pending);
            return Json(new { data = streaming });
        }

        public JsonResult RejectLiveStreaming()
        {
            List<SmartEcommerce.Models.Product.LiveStreaming> streaming = new Products().GetLiveStreamingByStatus(Status.Reject);
            return Json(new { data = streaming });
        }

        public JsonResult StatusLiveStreaming()
        {
            try
            {
                int Id = DataHelper.intParse(Request.Form["Id"]);
                int Status = DataHelper.intParse(Request.Form["Status"]);

                if (new Products().StatusLiveStreaming(clsSession.LoginId, Id, Status))
                {
                    if (Status == 3)
                        Session["livestreaming_save_msg"] = "Live TV rejected successfully!";
                    else if (Status == 1)
                        Session["livestreaming_save_msg"] = "Live TV approved successfully!";

                    return Json(new { ErrorCode = "000" });
                }
                else
                {
                    return Json(new { ErrorCode = "999" });
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return Json(new { ErrorCode = "999" });
            }
        }

        #endregion Live Streaming

        #region Live Videos

        public ActionResult LiveVideos()
        {
            return View();
        }

        public JsonResult CurrentLiveVideos()
        {
            List<SmartEcommerce.Models.Product.LiveVideos> livevideos = new Products().GetLiveVideosByStatus(Status.Current);

            return Json(new { data = livevideos });
        }

        public JsonResult ArchiveLiveVideos()
        {
            List<SmartEcommerce.Models.Product.LiveVideos> livevideos = new Products().GetLiveVideosByStatus(Status.Archive);
            return Json(new { data = livevideos });
        }

        public JsonResult SaveLiveVideos()
        {
            try
            {
                SmartEcommerce.Models.Product.LiveVideos livevideos = new Models.Product.LiveVideos();

                livevideos.Id = DataHelper.intParse(Request.Form["id"]);
                livevideos.Title = Request.Form["title"].ToString();
                livevideos.Description = Request.Form["description"].ToString();
                livevideos.ImageURL = Request.Form["image_name"].ToString();
                livevideos.VideoURL = Request.Form["video_url"].ToString();
                livevideos.Active = DataHelper.boolParse(Request.Form["active"]);
                livevideos.AddedBy = DataHelper.intParse(Request.Form["added_by"]);

                livevideos.EventDate = DataHelper.dateParse(Request.Form["event_date"]);
                livevideos.FromTime = DataHelper.dateParse(Request.Form["from_time"]);
                livevideos.ToTime = DataHelper.dateParse(Request.Form["to_time"]);

                int entryLevel = 0;

                //if admin upload live tv then select user for added by else save current user loginid
                int AddedBy = clsSession.LoginType == 1 ? livevideos.AddedBy : clsSession.LoginId;

                if (new Products().SaveLiveVideos(clsSession.LoginId, AddedBy, livevideos, out entryLevel))
                {
                    return Json(new { ErrorCode = "000", EntryLevel = entryLevel });
                }
                else
                {
                    return Json(new { ErrorCode = "999" });
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return Json(new { ErrorCode = "999" });
            }
        }

        public JsonResult MoveToArchiveLiveVideos()
        {
            try
            {
                int Id = DataHelper.intParse(Request.Form["Id"]);
                int Status = DataHelper.intParse(Request.Form["Status"]);

                if (new Products().ArchiveLiveVideos(clsSession.LoginId, Id, Status))
                {
                    if (Status == 2)
                        Session["livesvideos_save_msg"] = "Record move to archive successfully!";
                    else if (Status == 1)
                        Session["livesvideos_save_msg"] = "Record restore successfully!";

                    return Json(new { ErrorCode = "000" });
                }
                else
                {
                    return Json(new { ErrorCode = "999" });
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return Json(new { ErrorCode = "999" });
            }
        }

        public JsonResult LiveVideosById()
        {
            int Id = DataHelper.intParse(Request.Form["Id"]);

            SmartEcommerce.Models.Product.LiveVideos livevideos = new Products().GetLiveVideosById(Id);
            return Json(livevideos);
        }

        public JsonResult UploadLiveVideosImage()
        {
            try
            {
                string[] valid_extensions = { "jpeg", "jpg", "png", "gif" };
                string path = Server.MapPath("~/content/uploads/livevideos/");

                if (Request.Files["image"] != null)
                {
                    string img = Request.Files["image"].FileName.ToString();
                    string extension = img.Substring(img.LastIndexOf('.') + 1);

                    if (!Array.Exists(valid_extensions, e => e.ToLower().Equals(extension)))
                    {
                        // not a valid extension
                        return Json(new { ErrorCode = "002" });
                    }
                    else
                    {
                        string final_image = DateTime.Now.Ticks.ToString() + img.ToLower();
                        path += final_image;

                        Request.Files["image"].SaveAs(path);

                        // Successfully upload file
                        return Json(new { ErrorCode = "000", FileName = final_image });
                    }
                }
                else
                {
                    // File not found
                    return Json(new { ErrorCode = "001" });
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                // Exception 
                return Json(new { ErrorCode = "999" });
            }
        }

        public ActionResult LiveVideosPending()
        {
            return View();
        }

        public JsonResult PendingLiveVideos()
        {
            List<SmartEcommerce.Models.Product.LiveVideos> livevideos = new Products().GetLiveVideosByStatus(Status.Pending);
            return Json(new { data = livevideos });
        }

        public JsonResult RejectLiveVideos()
        {
            List<SmartEcommerce.Models.Product.LiveVideos> livevideos = new Products().GetLiveVideosByStatus(Status.Reject);
            return Json(new { data = livevideos });
        }

        public JsonResult StatusLiveVideos()
        {
            try
            {
                int Id = DataHelper.intParse(Request.Form["Id"]);
                int Status = DataHelper.intParse(Request.Form["Status"]);

                if (new Products().StatusLiveVideos(clsSession.LoginId, Id, Status))
                {
                    if (Status == 3)
                        Session["livevideos_save_msg"] = "Live Videos rejected successfully!";
                    else if (Status == 1)
                        Session["livevideos_save_msg"] = "Live Videos approved successfully!";

                    return Json(new { ErrorCode = "000" });
                }
                else
                {
                    return Json(new { ErrorCode = "999" });
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return Json(new { ErrorCode = "999" });
            }
        }

        #endregion Live Videos

        #region Programs

        public ActionResult Programs()
        {
            return View();
        }

        public JsonResult CurrentPrograms()
        {
            List<SmartEcommerce.Models.Product.Program> programs = new Products().GetProgramsByStatus(Status.Current);
            return Json(new { data = programs });
        }

        public JsonResult ArchivePrograms()
        {
            List<SmartEcommerce.Models.Product.Program> programs = new Products().GetProgramsByStatus(Status.Archive);
            return Json(new { data = programs });
        }

        public JsonResult SaveProgram()
        {
            try
            {
                SmartEcommerce.Models.Product.Program program = new Models.Product.Program();

                program.Id = DataHelper.intParse(Request.Form["id"]);
                program.Title = Request.Form["title"].ToString();
                program.Description = Request.Form["description"].ToString();
                program.ImageURL = Request.Form["image_name"].ToString();
                program.Active = DataHelper.boolParse(Request.Form["active"]);
                program.Featured = DataHelper.boolParse(Request.Form["Featured"]);
                program.AddedBy = DataHelper.intParse(Request.Form["added_by"]);

                int entryLevel = 0;
                int programId = 0;

                //if admin upload video then select user for added by else save current user loginid
                int AddedBy = clsSession.LoginType == 1 ? program.AddedBy : clsSession.LoginId;

                if (new Products().SaveProgram(clsSession.LoginId, program, AddedBy, out entryLevel, out programId))
                {
                    return Json(new { ErrorCode = "000", EntryLevel = entryLevel, ProgramId = programId });
                }
                else
                {
                    return Json(new { ErrorCode = "999" });
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return Json(new { ErrorCode = "999" });
            }
        }

        public JsonResult MoveToArchivePrograms()
        {
            try
            {
                int Id = DataHelper.intParse(Request.Form["Id"]);
                int Status = DataHelper.intParse(Request.Form["Status"]);

                if (new Products().ArchiveProgram(clsSession.LoginId, Id, Status))
                {
                    if (Status == 2)
                        Session["programs_save_msg"] = "Record move to archive successfully!";
                    else if (Status == 1)
                        Session["programs_save_msg"] = "Record restore successfully!";

                    return Json(new { ErrorCode = "000" });
                }
                else
                {
                    return Json(new { ErrorCode = "999" });
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return Json(new { ErrorCode = "999" });
            }
        }

        public JsonResult ProgramById()
        {
            int Id = DataHelper.intParse(Request.Form["Id"]);

            SmartEcommerce.Models.Product.Program program = new Products().GetProgramById(Id);
            return Json(program);
        }

        public JsonResult UploadProgramImage()
        {
            try
            {
                string[] valid_extensions = { "jpeg", "jpg", "png", "gif" };
                string path = Server.MapPath("~/content/uploads/programs/");

                if (Request.Files["image"] != null)
                {
                    string img = Request.Files["image"].FileName.ToString();
                    string extension = img.Substring(img.LastIndexOf('.') + 1);

                    if (!Array.Exists(valid_extensions, e => e.ToLower().Equals(extension)))
                    {
                        // not a valid extension
                        return Json(new { ErrorCode = "002" });
                    }
                    else
                    {
                        string final_image = DateTime.Now.Ticks.ToString() + img.ToLower();
                        path += final_image;

                        Request.Files["image"].SaveAs(path);

                        // Successfully upload file
                        return Json(new { ErrorCode = "000", FileName = final_image });
                    }
                }
                else
                {
                    // File not found
                    return Json(new { ErrorCode = "001" });
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                // Exception 
                return Json(new { ErrorCode = "999" });
            }
        }

        #endregion Programs

        #region Videos

        public ActionResult Videos()
        {
            return View();
        }

        public JsonResult CurrentVideos()
        {
            List<SmartEcommerce.Models.Product.Videos> videos = new Products().GetVideosByStatus(Status.Current);
            return Json(new { data = videos });
        }

        public JsonResult ArchiveVideos()
        {
            List<SmartEcommerce.Models.Product.Videos> videos = new Products().GetVideosByStatus(Status.Archive);
            return Json(new { data = videos });
        }

        public JsonResult SaveVideo()
        {
            try
            {
                SmartEcommerce.Models.Product.Videos video = new Models.Product.Videos();

                video.Id = DataHelper.intParse(Request.Form["id"]);
                video.Title = Request.Form["title"].ToString();
                video.Program.Id = DataHelper.intParse(Request.Form["program_id"]);
                video.Category.Id = DataHelper.intParse(Request.Form["category_id"]);
                video.Description = Request.Form["description"].ToString();
                video.ImageURL = Request.Form["image_name"].ToString();
                video.VideoURL = Request.Form["video_url"].ToString();
                video.Active = DataHelper.boolParse(Request.Form["active"]);
                video.AddedBy = DataHelper.intParse(Request.Form["added_by"]);
                int entryLevel = 0;

                //if admin upload video then select user for added by else save current user loginid
                int AddedBy = clsSession.LoginType == 1 ? video.AddedBy : clsSession.LoginId; 

                if (new Products().SaveVideos(clsSession.LoginId, AddedBy, video, out entryLevel))
                {
                    return Json(new { ErrorCode = "000", EntryLevel = entryLevel });
                }
                else
                {
                    return Json(new { ErrorCode = "999" });
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return Json(new { ErrorCode = "999" });
            }
        }

        public JsonResult MoveToArchiveVideos()
        {
            try
            {
                int Id = DataHelper.intParse(Request.Form["Id"]);
                int Status = DataHelper.intParse(Request.Form["Status"]);

                string Response = new Products().ArchiveVideos(clsSession.LoginId, Id, Status);
                return Json(new { ErrorCode = Response });

                //if (new Products().ArchiveVideos(clsSession.LoginId, Id, Status))
                //{
                //    if (Status == 2)
                //        Session["videos_save_msg"] = "Record move to archive successfully!";
                //    else if (Status == 1)
                //        Session["videos_save_msg"] = "Record restore successfully!";

                //    return Json(new { ErrorCode = "000" });
                //}
                //else
                //{
                //    return Json(new { ErrorCode = "999" });
                //}
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return Json(new { ErrorCode = "999" });
            }
        }

        public JsonResult StatusVideos()
        {
            try
            {
                int Id = DataHelper.intParse(Request.Form["Id"]);
                int Status = DataHelper.intParse(Request.Form["Status"]);

                if (new Products().StatusVideos(clsSession.LoginId, Id, Status))
                {
                    if (Status == 3)
                        Session["videos_save_msg"] = "Video rejected successfully!";
                    else if (Status == 1)
                        Session["videos_save_msg"] = "Video approved successfully!";

                    return Json(new { ErrorCode = "000" });
                }
                else
                {
                    return Json(new { ErrorCode = "999" });
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return Json(new { ErrorCode = "999" });
            }
        }

        public JsonResult VideoById()
        {
            int Id = DataHelper.intParse(Request.Form["Id"]);

            SmartEcommerce.Models.Product.Videos video = new Products().GetVideoById(Id);
            return Json(video);
        }

        public JsonResult UploadVideoImage()
        {
            try
            {
                string[] valid_extensions = { "jpeg", "jpg", "png", "gif" };
                string path = Server.MapPath("~/content/uploads/videos/thumb/");

                if (Request.Files["image"] != null)
                {
                    string img = Request.Files["image"].FileName.ToString();
                    string extension = img.Substring(img.LastIndexOf('.') + 1);

                    if (!Array.Exists(valid_extensions, e => e.ToLower().Equals(extension)))
                    {
                        // not a valid extension
                        return Json(new { ErrorCode = "002" });
                    }
                    else
                    {
                        string final_image = DateTime.Now.Ticks.ToString() + img.ToLower();
                        path += final_image;

                        Request.Files["image"].SaveAs(path);

                        // Successfully upload file
                        return Json(new { ErrorCode = "000", FileName = final_image });
                    }
                }
                else
                {
                    // File not found
                    return Json(new { ErrorCode = "001" });
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                // Exception 
                return Json(new { ErrorCode = "999" });
            }
        }

        public JsonResult UploadVideoFile()
        {
            try
            {
                string[] valid_extensions = { "avi", "mp4", "mkv", "flv" };
                string path = Server.MapPath("~/content/uploads/videos/");

                if (Request.Files["file_video"] != null)
                {
                    string img = Request.Files["file_video"].FileName.ToString();
                    string extension = img.Substring(img.LastIndexOf('.') + 1);

                    if (!Array.Exists(valid_extensions, e => e.ToLower().Equals(extension)))
                    {
                        // not a valid extension
                        return Json(new { ErrorCode = "002" });
                    }
                    else
                    {
                        string final_image = DateTime.Now.Ticks.ToString() + img.ToLower();
                        path += final_image;

                        System.Threading.Thread.Sleep(2000);

                        Request.Files["file_video"].SaveAs(path);

                        //string path = HttpContext.Server.MapPath("/Content/ffmepg/");
                        //string mpg = HttpContext.Server.MapPath("/Content/thumail") + "//video%3d.png";
                        //string video = HttpContext.Server.MapPath(path);

                        string image_name = "VideoThumb_" + DateTime.Now.Ticks.ToString() + ".jpg";

                        string video = path; //Server.MapPath("~/content/uploads/videos/");

                        Process ffmpeg = new Process();

                        var ffMpeg = new NReco.VideoConverter.FFMpegConverter();
                        string thumbnailJPEGpath = Server.MapPath("~/content/uploads/videos/thumb/" + image_name);

                        ffMpeg.GetVideoThumbnail(video, thumbnailJPEGpath, 10);


                        // Successfully upload file
                        return Json(new { ErrorCode = "000", FileName = final_image, DisplayName = img.Replace(extension, ""), ImageName = image_name });
                    }
                }
                else
                {
                    // File not found
                    return Json(new { ErrorCode = "001" });
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                // Exception 
                return Json(new { ErrorCode = "999" });
            }
        }

        public JsonResult CurrentVideoType()
        {
            List<object> list = new List<object>
            {
                new { Id = 1,  Name = "Solo" },
                new { Id = 2,  Name = "Episode" }
            };

            return Json(new { data = list });
        }

        public ActionResult VideosPending()
        {
            return View();
        }

        public JsonResult PendingVideos()
        {
            List<SmartEcommerce.Models.Product.Videos> videos = new Products().GetVideosByStatus(Status.Pending);
            return Json(new { data = videos });
        }

        public JsonResult RejectVideos()
        {
            List<SmartEcommerce.Models.Product.Videos> videos = new Products().GetVideosByStatus(Status.Reject);
            return Json(new { data = videos });
        }

        #endregion Videos

        #region Sliders

        public ActionResult Sliders()
        {
            return View();
        }

        public JsonResult CurrentSliders()
        {
            List<SmartEcommerce.Models.Common.Sliders> sliders = new Common().GetSliderByStatus(Status.Current);
            return Json(new { data = sliders });
        }

        public JsonResult ArchiveSliders()
        {
            List<SmartEcommerce.Models.Common.Sliders> sliders = new Common().GetSliderByStatus(Status.Archive);
            return Json(new { data = sliders });
        }

        public JsonResult UploadSliderImage()
        {
            try
            {
                string[] valid_extensions = { "jpeg", "jpg", "png", "gif" };
                string path = Server.MapPath("~/content/uploads/slider/");

                if (Request.Files["image"] != null)
                {
                    string img = Request.Files["image"].FileName.ToString();
                    string extension = img.Substring(img.LastIndexOf('.') + 1);

                    if (!Array.Exists(valid_extensions, e => e.ToLower().Equals(extension)))
                    {
                        // not a valid extension
                        return Json(new { ErrorCode = "002" });
                    }
                    else
                    {
                        string final_image = DateTime.Now.Ticks.ToString() + img.ToLower();
                        path += final_image;

                        Request.Files["image"].SaveAs(path);

                        // Successfully upload file
                        return Json(new { ErrorCode = "000", FileName = final_image });
                    }
                }
                else
                {
                    // File not found
                    return Json(new { ErrorCode = "001" });
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                // Exception 
                return Json(new { ErrorCode = "999" });
            }
        }

        [ValidateInput(false)]
        public JsonResult SaveSlider()
        {
            try
            {
                int Id = DataHelper.intParse(Request.Form["Id"]);
                string Title = Request.Form["Title"].ToString();
                string PageName = Request.Form["PageName"].ToString();
                string ImageURL = Request.Form["ImageURL"].ToString();
                string RedirectionURL = Request.Form["RedirectionURL"].ToString();
                int Sequence = DataHelper.intParse(Request.Form["Sequence"].ToString());
                bool Active = DataHelper.boolParse(Request.Form["active"]);
                int entryLevel = 0;

                if (new Common().SaveSlider(clsSession.LoginId, Id, Title, PageName, ImageURL, RedirectionURL, Sequence, Active, out entryLevel))
                {
                    return Json(new { ErrorCode = "000", EntryLevel = entryLevel });
                }
                else
                {
                    return Json(new { ErrorCode = "999" });
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return Json(new { ErrorCode = "999" });
            }
        }

        public JsonResult MoveToArchiveSlider()
        {
            try
            {
                int SliderId = DataHelper.intParse(Request.Form["Id"]);
                int Status = DataHelper.intParse(Request.Form["Status"]);

                if (new Common().ArchiveSlider(clsSession.LoginId, SliderId, Status))
                {
                    if (Status == 2)
                        Session["slider_save_msg"] = "Record move to archive successfully!";
                    else if (Status == 1)
                        Session["slider_save_msg"] = "Record restore successfully!";

                    return Json(new { ErrorCode = "000" });
                }
                else
                {
                    return Json(new { ErrorCode = "999" });
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return Json(new { ErrorCode = "999" });
            }
        }

        public JsonResult SliderById()
        {
            int Id = DataHelper.intParse(Request.Form["Id"]);

            SmartEcommerce.Models.Common.Sliders slider = new Common().GetSliderById(Id);
            return Json(slider);
        }

        #endregion

        #region Topics

        public ActionResult Topics()
        {
            return View();
        }

        public JsonResult CurrentTopics()
        {
            List<SmartEcommerce.Models.Common.Topic> topics = new Common().GetTopicByStatus(Status.Current);
            return Json(new { data = topics });
        }

        public JsonResult ArchiveTopics()
        {
            List<SmartEcommerce.Models.Common.Topic> topics = new Common().GetTopicByStatus(Status.Archive);
            return Json(new { data = topics });
        }

        public JsonResult SaveTopic()
        {
            try
            {
                int Id = DataHelper.intParse(Request.Form["Id"]);
                string Title = Request.Form["Title"].ToString();
                bool Active = DataHelper.boolParse(Request.Form["active"]);
                int entryLevel = 0;

                if (new Common().SaveTopic(clsSession.LoginId, Id, Title, Active, out entryLevel))
                {
                    return Json(new { ErrorCode = "000", EntryLevel = entryLevel });
                }
                else
                {
                    return Json(new { ErrorCode = "999" });
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return Json(new { ErrorCode = "999" });
            }
        }

        public JsonResult MoveToArchiveTopic()
        {
            try
            {
                int TopicId = DataHelper.intParse(Request.Form["Id"]);
                int Status = DataHelper.intParse(Request.Form["Status"]);

                if (new Common().ArchiveTopic(clsSession.LoginId, TopicId, Status))
                {
                    if (Status == 2)
                        Session["slider_save_msg"] = "Record move to archive successfully!";
                    else if (Status == 1)
                        Session["slider_save_msg"] = "Record restore successfully!";

                    return Json(new { ErrorCode = "000" });
                }
                else
                {
                    return Json(new { ErrorCode = "999" });
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return Json(new { ErrorCode = "999" });
            }
        }

        public JsonResult TopicById()
        {
            int Id = DataHelper.intParse(Request.Form["Id"]);

            SmartEcommerce.Models.Common.Topic topic = new Common().GetTopicById(Id);
            return Json(topic);
        }

        #endregion Topics

        #region Partners/Users

        public JsonResult UploadUserImage()
        {
            try
            {
                string[] valid_extensions = { "jpeg", "jpg", "png", "gif" };
                string path = Server.MapPath("~/content/uploads/users/");

                if (Request.Files["image"] != null)
                {
                    string img = Request.Files["image"].FileName.ToString();
                    string extension = img.Substring(img.LastIndexOf('.') + 1);

                    if (!Array.Exists(valid_extensions, e => e.ToLower().Equals(extension)))
                    {
                        // not a valid extension
                        return Json(new { ErrorCode = "002" });
                    }
                    else
                    {
                        string final_image = DateTime.Now.Ticks.ToString() + img.ToLower();
                        path += final_image;

                        Request.Files["image"].SaveAs(path);

                        // Successfully upload file
                        return Json(new { ErrorCode = "000", FileName = final_image });
                    }
                }
                else
                {
                    // File not found
                    return Json(new { ErrorCode = "001" });
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                // Exception 
                return Json(new { ErrorCode = "999" });
            }
        }

        public ActionResult Users()
        {
            return View();
        }

        public JsonResult CurrentUsers()
        {
            List<SmartEcommerce.Models.Common.User> users = new Common().GetUserByStatus(Status.Current, LoginType.User);
            return Json(new { data = users });
        }

        public JsonResult ArchiveUsers()
        {
            List<SmartEcommerce.Models.Common.User> users = new Common().GetUserByStatus(Status.Archive, LoginType.User);
            return Json(new { data = users });
        }

        public JsonResult SaveUser()
        {
            try
            {

                SmartEcommerce.Models.Common.Partner partner = new Models.Common.Partner();

                partner.Id = DataHelper.intParse(Request.Form["Id"]);
                partner.FullName = Request.Form["UserName"].ToString();
                partner.PartnerType.Id = DataHelper.intParse(Request.Form["Type"].ToString());
                partner.PartnerCategory.Id = DataHelper.intParse(Request.Form["Category"]);
                partner.ContactPerson = Request.Form["ContactPerson"].ToString();
                partner.Telephone = Request.Form["Telephone"].ToString();
                partner.MobileNo = Request.Form["MobileNo"].ToString();
                partner.EmailAddress = Request.Form["UserId"].ToString();
                partner.Password = Request.Form["Password"].ToString();
                partner.Country.Id = DataHelper.intParse(Request.Form["Country"]);
                partner.State.Id = DataHelper.intParse(Request.Form["State"]);
                partner.City.Id = DataHelper.intParse(Request.Form["City"]);
                partner.Address = Request.Form["Address"].ToString();
                partner.PartnerContentType.Id = DataHelper.intParse(Request.Form["TypeContent"]);
                //partner.PartnerContentTypeUpload.Id = DataHelper.intParse(Request.Form["TypeContentUpload"]);
                partner.Active = DataHelper.boolParse(Request.Form["Active"]);
                partner.Approval = DataHelper.boolParse(Request.Form["Approval"]);
                partner.IsPassChange = DataHelper.boolParse(Request.Form["IsPassChange"]);
                partner.ProfilePicture = Request.Form["image_name"].ToString();
                partner.Monetization = DataHelper.intParse(Request.Form["Monetization"]);

                string content_data = Request.Form["TypeContentUpload"].ToString();
                string[] content_array = content_data.Split(',');

                foreach (string row in content_array)
                {
                    partner.PartnerContentTypeUpload.Add(new SmartEcommerce.Models.Product.Category
                    {
                        ContentTypeUploadId = row.ToString()
                    });
                }

                int entryLevel = 0;

                if (new Common().SaveUser(clsSession.LoginId, partner, out entryLevel))
                {
                    return Json(new { ErrorCode = "000", EntryLevel = entryLevel });
                }
                else
                {
                    return Json(new { ErrorCode = "999" });
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return Json(new { ErrorCode = "999" });
            }
        }

        public JsonResult MoveToArchiveUser()
        {
            try
            {
                int UserId = DataHelper.intParse(Request.Form["Id"]);
                int Status = DataHelper.intParse(Request.Form["Status"]);

                if (new Common().ArchiveUser(clsSession.LoginId, UserId, Status))
                {
                    if (Status == 2)
                        Session["slider_save_msg"] = "Record move to archive successfully!";
                    else if (Status == 1)
                        Session["slider_save_msg"] = "Record restore successfully!";

                    return Json(new { ErrorCode = "000" });
                }
                else
                {
                    return Json(new { ErrorCode = "999" });
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return Json(new { ErrorCode = "999" });
            }
        }

        public JsonResult UserById()
        {
            int Id = DataHelper.intParse(Request.Form["Id"]);

            SmartEcommerce.Models.Common.User user = new Common().GetUserById(Id);
            return Json(user);
        }

        public ActionResult PendingPartners()
        {
            return View();
        }

        public JsonResult PendingPartner()
        {
            List<SmartEcommerce.Models.Common.Partner> partners = new Common().GetPartnersByStatus(Status.Pending);
            return Json(new { data = partners });
        }

        public JsonResult RejectPartner()
        {
            List<SmartEcommerce.Models.Common.Partner> partners = new Common().GetPartnersByStatus(Status.Reject);
            return Json(new { data = partners });
        }

        public JsonResult StatusPartners()
        {
            try
            {
                int Id = DataHelper.intParse(Request.Form["Id"]);
                int Status = DataHelper.intParse(Request.Form["Status"]);

                if (new Common().StatusPartners(clsSession.LoginId, Id, Status))
                {
                    if (Status == 3)
                        Session["videos_save_msg"] = "Partner rejected successfully!";
                    else if (Status == 1)
                        Session["videos_save_msg"] = "Partner approved successfully!";

                    return Json(new { ErrorCode = "000" });
                }
                else
                {
                    return Json(new { ErrorCode = "999" });
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return Json(new { ErrorCode = "999" });
            }
        }

        public JsonResult PartnerById()
        {
            int Id = DataHelper.intParse(Request.Form["id"]);

            SmartEcommerce.Models.Common.Partner partner = new Common().GetPartnerById(Id);
            return Json(partner);
        }

        public JsonResult CurrentPartnerType()
        {
            List<SmartEcommerce.Models.Common.PartnerType> partners = new Common().GetPartnerTypeByStatus(Status.Current);
            return Json(new { data = partners });
        }

        public JsonResult CurrentPartnerCategory()
        {
            List<SmartEcommerce.Models.Common.PartnerCategory> partners = new Common().GetPartnerCategoryByStatus(Status.Current);
            return Json(new { data = partners });
        }

        public JsonResult CurrentPartnerContentype()
        {
            List<SmartEcommerce.Models.Common.PartnerContentType> partners = new Common().GetPartnerContentTypeByStatus(Status.Current);
            return Json(new { data = partners });
        }

        public JsonResult CurrentPartnerContentypeUpload()
        {
            List<SmartEcommerce.Models.Product.Category> partners = new Products().GetCategoryByStatus(Status.Current);
            return Json(new { data = partners });
        }

        public JsonResult CurrentPartners()
        {
            List<SmartEcommerce.Models.Common.Partner> partners = new Common().GetPartnersByStatus(Status.Current);
            return Json(new { data = partners });
        }

        public JsonResult ArchivePartners()
        {
            List<SmartEcommerce.Models.Common.Partner> partners = new Common().GetPartnersByStatus(Status.Archive);
            return Json(new { data = partners });
        }

        #endregion Partners/Users

        #region Customers

        public ActionResult Customers()
        {
            return View();
        }

        public JsonResult CurrentCustomers()
        {
            List<SmartEcommerce.Models.Common.User> customers = new Common().GetCustomer();
            return Json(new { data = customers });
        }

        #endregion

        #region Disputed Videos

        public ActionResult ClaimVideos()
        {
            return View();
        }

        public JsonResult CurrentClaimVideos()
        {

            string url = Request.RawUrl;
            string[] prms = Request.RawUrl.Split('/');
            int status = 0;

            if (prms.Count() == 4)
            {
                status = DataHelper.intParse(prms[3]);
            }

            List<SmartEcommerce.Models.Common.DisputeVideo> claims = new Common().GetClaimVideos(status);
            return Json(new { data = claims });
        }
        
        public ActionResult ReportVideos()
        {
            return View();
        }

        public JsonResult CurrentReportVideos()
        {
            List<SmartEcommerce.Models.Common.DisputeVideo> reports = new Common().GetReportVideos();
            return Json(new { data = reports });
        }

        public JsonResult ClaimDescription()
        {
            try
            {
                int Id = DataHelper.intParse(Request.Form["Id"].ToString());

                SmartEcommerce.Models.Common.DisputeVideo claims = new Common().GetClaimDescription(Id);
                return Json(claims);

            }
            catch (Exception ae)
            {
                return Json(new { ErrorCode = "999", ErrorDescription = ae.Message });
            }
        }

        public JsonResult ClaimVideosStatusUpdate()
        {
            try
            {
                int Id = DataHelper.intParse(Request.Form["Id"]);
                int Status = DataHelper.intParse(Request.Form["Status"]);

                if (new Products().ClaimVideosStatus(clsSession.LoginId, Id, Status))
                {
                    if (Status == 2)
                        Session["videos_save_msg"] = "Claim video reject successfully!";
                    else if (Status == 1)
                        Session["videos_save_msg"] = "Claim video approved successfully!";

                    return Json(new { ErrorCode = "000" });
                }
                else
                {
                    return Json(new { ErrorCode = "999" });
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return Json(new { ErrorCode = "999" });
            }
        }

        #endregion

        #region Subscription Setup

        public ActionResult Subscription()
        {
            return View();
        }

        public JsonResult CurrentSubscription()
        {
            List<SmartEcommerce.Models.Common.Subscription> subscriptions = new Common().GetSubscriptionByStatus(Status.Current);
            return Json(new { data = subscriptions });
        }

        public JsonResult ArchiveSubscription()
        {
            List<SmartEcommerce.Models.Common.Subscription> subscriptions = new Common().GetSubscriptionByStatus(Status.Archive);
            return Json(new { data = subscriptions });
        }

        public JsonResult SaveSubscription()
        {
            try
            {
                int Id = DataHelper.intParse(Request.Form["Id"]);
                string SubType = Request.Form["SubType"].ToString();
                int CountryId = DataHelper.intParse(Request.Form["CountryId"]);
                string Currency = Request.Form["Currency"].ToString();
                decimal YearlyRate = DataHelper.decimalParse(Request.Form["YearlyRate"]);
                decimal MonthlyRate = DataHelper.decimalParse(Request.Form["MonthlyRate"]);
                bool Active = DataHelper.boolParse(Request.Form["active"]);

                int entryLevel = 0;

                string ErrorCode = new Common().SaveSubscription(clsSession.LoginId, Id, SubType, CountryId, Currency, YearlyRate, MonthlyRate, Active, out entryLevel);

                return Json(new { ErrorCode = ErrorCode, EntryLevel = entryLevel });
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return Json(new { ErrorCode = "999" });
            }
        }

        public JsonResult MoveToArchiveSubscription()
        {
            try
            {
                int SubscriptionId = DataHelper.intParse(Request.Form["Id"]);
                int Status = DataHelper.intParse(Request.Form["Status"]);

                if (new Common().ArchiveSubscription(clsSession.LoginId, SubscriptionId, Status))
                {
                    if (Status == 2)
                        Session["store_save_msg"] = "Record move to archive successfully!";
                    else if (Status == 1)
                        Session["store_save_msg"] = "Record restore successfully!";

                    return Json(new { ErrorCode = "000" });
                }
                else
                {
                    return Json(new { ErrorCode = "999" });
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return Json(new { ErrorCode = "999" });
            }
        }

        public JsonResult SubscriptionById()
        {
            int Id = DataHelper.intParse(Request.Form["Id"]);

            SmartEcommerce.Models.Common.Subscription subscription = new Common().GetSubscriptionById(Id);
            return Json(subscription);
        }

        public JsonResult CurrenSubType()
        {
            List<object> list = new List<object>
            {
                new { Name = "Free" },
                new { Name = "Paid" }
            };

            return Json(new { data = list });
        }

        public JsonResult GetGlobalRates()
        {
            try
            {
                SmartEcommerce.Models.Common.Subscription subscription = new Common().GetGlobalRates();
                return Json(subscription);

            }
            catch (Exception ae)
            {
                return Json(new { ErrorCode = "999", ErrorDescription = ae.Message });
            }
        }

        public JsonResult UpdateGlobalRates()
        {
            try
            {

                string ParameterName = Request.Form["ParameterName"].ToString();
                string ParameterValue = Request.Form["ParameterValue"].ToString();
                string ParameterName2 = Request.Form["ParameterName2"].ToString();
                string ParameterValue2 = Request.Form["ParameterValue2"].ToString();

                if (new Common().SaveParameterValue(ParameterName, ParameterValue))
                {
                    if (new Common().SaveParameterValue(ParameterName2, ParameterValue2))
                    {
                        return Json(new { ErrorCode = "000" });
                    }
                    else
                    {
                        return Json(new { ErrorCode = "999" });
                    }
                }
                else
                {
                    return Json(new { ErrorCode = "999" });
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return Json(new { ErrorCode = "999" });
            }
        }

        #endregion

        #region Sysytem Configuration

        public ActionResult Settings()
        {
            return View();
        }

        public JsonResult GetSystemConfiguration()
        {
            try
            {
                string ParameterName = Request.Form["ParameterName"].ToString();

                SmartEcommerce.Models.Common.SystemConfiguration configuration = new Common().GetParameterValue(ParameterName);
                return Json(configuration);

            }
            catch (Exception ae)
            {
                return Json(new { ErrorCode = "999", ErrorDescription = ae.Message });
            }
        }

        public JsonResult UpdateSystemConfiguration()
        {
            try
            {

                string ParameterName = Request.Form["ParameterName"].ToString();
                string ParameterValue = Request.Form["ParameterValue"].ToString();

                if (new Common().SaveParameterValue(ParameterName,ParameterValue))
                {
                    return Json(new { ErrorCode = "000" });
                }
                else
                {
                    return Json(new { ErrorCode = "999" });
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return Json(new { ErrorCode = "999" });
            }
        }

        public JsonResult GetSettings()
        {
            try
            {
                SmartEcommerce.Models.Common.Settings settings = new Common().GetSettings();
                return Json(settings);

            }
            catch (Exception ae)
            {
                return Json(new { ErrorCode = "999", ErrorDescription = ae.Message });
            }
        }

        public JsonResult SaveSettings()
        {
            try
            {

                string MonthlyRate = Request.Form["txtMonthlyRate"].ToString();
                string YearlyRate = Request.Form["txtYearlyRate"].ToString();
                string IsTrial = Request.Form["IsTrial"].ToString();
                string TrialDays = Request.Form["txtTrialDays"].ToString();

                if (new Common().SaveSettings(MonthlyRate, YearlyRate, IsTrial, TrialDays))
                {
                    return Json(new { ErrorCode = "000" });
                }
                else
                {
                    return Json(new { ErrorCode = "999" });
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return Json(new { ErrorCode = "999" });
            }
        }

        #endregion

        #region Payments

        public ActionResult Payments()
        {
            return View();
        }

        public JsonResult CurrentPayments()
        {
            List<SmartEcommerce.Models.Common.Payments> payments = new Common().GetPayments();
            return Json(new { data = payments });
        }

        #endregion

    }
}