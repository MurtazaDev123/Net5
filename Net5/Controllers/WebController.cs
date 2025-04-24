using BusinessLogic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartEcommerce.Controllers.Helper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Stripe.Checkout;
using Stripe;

namespace SmartEcommerce.Controllers
{
    public class WebController : Controller
    {
        #region Home Page

        public ActionResult Index()
        {
            SmartEcommerce.Models.Web.HomePage model = new BusinessLogic.WebClass().GetDefault();
            return View(model);
        }

        #endregion

        #region Live Streaming

        public ActionResult LiveStreaming()
        {
            List<SmartEcommerce.Models.Product.LiveStreaming> list = new BusinessLogic.Products().GetLiveStreamingWeb(Status.Current);
            ViewBag.PageTitle = "Live TV";
            return View(list);
        }

        public ActionResult LiveStreamDetail()
        {

            if (clsWebSession.HasLogin)
            {
                SmartEcommerce.Models.Common.Subscription subscription = new BusinessLogic.Common().GetSubscriptionStatus(clsWebSession.LoginId);

                if (subscription.SubStatus == "Active" || subscription.SubStatus == "Cancelled" || BusinessLogic.clsWebSession.LoginType == 2)
                {
                    string url = Request.RawUrl;
                    string[] prms = Request.RawUrl.Split('/');
                    string tv_name = "";

                    if (prms.Count() == 3)
                    {
                        tv_name = prms[2];
                    }
                    else
                    {
                        return RedirectToRoute("live-tv");
                    }

                    int LiveTvId = DataHelper.GetTvIdByName(tv_name);

                    if (LiveTvId == 0)
                    {
                        return RedirectToRoute("404");
                    }

                    SmartEcommerce.Models.Product.LiveStreaming data = new BusinessLogic.Products().GetLiveStreamingByIdWithRelated(LiveTvId);
                    ViewBag.PageTitle = data.Title;
                    return View(data);

                }
                else
                {
                    SBSession.CreateSession("SessionCustomerId", clsWebSession.LoginId);
                    SBSession.CreateSession("SessionCountryId", subscription.Country.Id);

                    return RedirectToAction("SubscriptionPlan");
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        #endregion

        #region Live Videos

        public ActionResult LiveVideos()
        {
            List<SmartEcommerce.Models.Product.LiveVideos> list = new BusinessLogic.Products().GetLiveVideosWeb(Status.Current);
            ViewBag.PageTitle = "Live Videos";
            return View(list);
        }

        public ActionResult LiveVideosDetail()
        {

            if (clsWebSession.HasLogin)
            {
                SmartEcommerce.Models.Common.Subscription subscription = new BusinessLogic.Common().GetSubscriptionStatus(clsWebSession.LoginId);

                //subscription.SubType == "Free" || subscription.SubType == "Trial" || subscription.SubStatus == "Active" || subscription.SubStatus == "Cancelled"
                if (subscription.SubStatus == "Active" || subscription.SubStatus == "Cancelled" || BusinessLogic.clsWebSession.LoginType == 2)
                {
                    string url = Request.RawUrl;
                    string[] prms = Request.RawUrl.Split('/');
                    string tv_name = "";

                    if (prms.Count() == 3)
                    {
                        tv_name = prms[2];
                    }
                    else
                    {
                        return RedirectToRoute("live-videos");
                    }

                    int LiveTvId = DataHelper.GetLiveVideosIdByName(tv_name);

                    if (LiveTvId == 0)
                    {
                        return RedirectToRoute("404");
                    }

                    SmartEcommerce.Models.Product.LiveVideos data = new BusinessLogic.Products().GetLiveVideosByIdWithRelated(LiveTvId);
                    ViewBag.PageTitle = data.Title;
                    return View(data);

                }
                else
                {
                    SBSession.CreateSession("SessionCustomerId", clsWebSession.LoginId);
                    SBSession.CreateSession("SessionCountryId", subscription.Country.Id);

                    return RedirectToAction("SubscriptionPlan");
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        #endregion

        #region Programs
        public ActionResult Programs()
        {
            SmartEcommerce.Models.Product.Program model = new BusinessLogic.Products().GetProgramsWithReatedData(Status.Current);
            ViewBag.PageTitle = "Programs";
            return View(model);
        }

        public ActionResult ProgramDetail()
        {
            string url = Request.RawUrl;
            string[] prms = Request.RawUrl.Split('/');
            string program_name = "";

            if (prms.Count() == 3)
            {
                program_name = prms[2];
            }
            else
            {
                return RedirectToRoute("programs");
            }


            int ProgramId = DataHelper.GetProgramIdByName(program_name);

            if (ProgramId == 0)
            {
                return RedirectToRoute("404");
            }

            SmartEcommerce.Models.Product.Program data = new BusinessLogic.Products().GetProgramDetailWithEpisodes(ProgramId);
            ViewBag.PageTitle = data.Title;

            if (data.ErrorCode == "999")
            {
                return RedirectToRoute("error");
            }

            return View(data);
        }
        #endregion

        #region Library

        public ActionResult Library()
        {
            //List<SmartEcommerce.Models.Product.Videos> data = new BusinessLogic.Products().GetVideosByStatus(Status.Current);
            //return View(data);
            ViewBag.PageTitle = "Library";
            return View();
        }

        public JsonResult LibraryLoadMore()
        {
            try
            {
                int page = DataHelper.intParse(Request.Form["page"]);

                SmartEcommerce.Models.Product.Videos search_result = new BusinessLogic.Products().GetVideosByStatusLoadMore(Status.Current, page);

                if (search_result != null)
                    return Json(search_result);
                else
                    return Json("999");
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return Json("999");
            }
        }

        public ActionResult LibraryDetail()
        {

            if (clsWebSession.HasLogin)
            {

                SmartEcommerce.Models.Common.Subscription subscription = new BusinessLogic.Common().GetSubscriptionStatus(clsWebSession.LoginId);

                if (subscription.SubStatus == "Active" || subscription.SubStatus == "Cancelled" || BusinessLogic.clsWebSession.LoginType == 2)
                {
                    if (Request.QueryString["v"] == null)
                    {
                        return RedirectToRoute("library");
                    }

                    int VideoId = DataHelper.intParse(SBEncryption.Decrypt(Request.QueryString["v"].ToString()));

                    SmartEcommerce.Models.Product.Videos data = new BusinessLogic.Products().GetVideoByIdWithRelated(VideoId);
                    ViewBag.PageTitle = data.Title;
                    return View(data);
                }
                else
                {
                    SBSession.CreateSession("SessionCustomerId", clsWebSession.LoginId);
                    SBSession.CreateSession("SessionCountryId", subscription.Country.Id);

                    return RedirectToAction("SubscriptionPlan");
                }
            }
            else
            {
                return RedirectToAction("Login");
            }

            //if (Request.QueryString["v"] == null)
            //{
            //    return RedirectToRoute("library");
            //}

            //int VideoId = DataHelper.intParse(SBEncryption.Decrypt(Request.QueryString["v"].ToString()));

            //SmartEcommerce.Models.Product.Videos data = new BusinessLogic.Products().GetVideoByIdWithRelated(VideoId);
            //ViewBag.PageTitle = data.Title;
            //return View(data);

        }

        public ActionResult RelatedVideosAll(int Id)
        {
            List<SmartEcommerce.Models.Product.Videos> data = new BusinessLogic.Products().GetAllRelatedVideos(Id);
            return View(data);
        }

        public JsonResult VideoAddDispute()
        {
            try
            {
                int LoginId = DataHelper.intParse(Request.Form["LoginId"].ToString());
                int VideoId = DataHelper.intParse(Request.Form["VideoId"].ToString());
                string VideoURL = Request.Form["VideoURL"].ToString();
                string Description = Request.Form["Description"].ToString();
                int TopicId = DataHelper.intParse(Request.Form["TopicId"].ToString());
                string TopicName = Request.Form["TopicName"].ToString();
                int Type = DataHelper.intParse(Request.Form["Type"].ToString());

                var result = new BusinessLogic.Accounts().VideoAddDispute(LoginId, VideoId, VideoURL, Description, TopicId, TopicName, Type);
                return Json(result);

            }
            catch (Exception ae)
            {
                return Json(new { ErrorCode = "999", ErrorDescription = ae.Message });
            }
        }

        public JsonResult LoadMoreRelatedVideos()
        {

            int Id = DataHelper.intParse(Request.Form["id"]);
            int StartIndex = DataHelper.intParse(Request.Form["startIndex"]);
            int Offset = DataHelper.intParse(Request.Form["offset"]);

            List<SmartEcommerce.Models.Product.Videos> data = new BusinessLogic.Products().LoadMoreRelatedVideos(Id, StartIndex, Offset);
            return Json(data);
        }

        public ActionResult RecentWatch()
        {
            ViewBag.PageTitle = "Recent Watch";
            return View();
        }

        public JsonResult RecentWatchLoadMore()
        {
            try
            {
                int page = DataHelper.intParse(Request.Form["page"]);

                SmartEcommerce.Models.Product.Videos search_result = new BusinessLogic.Products().GetVideosByRecentWatchLoadMore(page);

                if (search_result != null)
                    return Json(search_result);
                else
                    return Json("999");
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return Json("999");
            }
        }

        public ActionResult LibraryByUser()
        {
            string url = Request.RawUrl;
            string[] prms = Request.RawUrl.Split('/');
            string user_name = "";

            if (prms.Count() == 2)
            {
                user_name = prms[1];
            }
            else
            {
                return RedirectToRoute("library");
            }

            int user_id = DataHelper.GetUserIdByName(user_name);
            string full_name = DataHelper.GetFullNameById(user_id);

            ViewBag.UserName = user_name;
            ViewBag.FullName = full_name;
            ViewBag.UserId = user_id;

            return View();
        }

        public JsonResult LibraryByUserLoadMore()
        {
            try
            {
                int page = DataHelper.intParse(Request.Form["page"]);
                string user_name = DataHelper.stringParse(Request.Form["user_name"]);

                int UserId = DataHelper.GetUserIdByName(user_name);

                SmartEcommerce.Models.Product.Videos search_result = new BusinessLogic.Products().GetVideosByUserLoadMore(UserId, page);

                if (search_result != null)
                    return Json(search_result);
                else
                    return Json("999");
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return Json("999");
            }
        }

        #endregion

        #region Accounts

        public ActionResult Login()
        {
            return View();
        }

        public JsonResult LoginRequest()
        {
            try
            {
                string user_id = Request.Form["user_name"].ToString();
                string password = Request.Form["password"].ToString();

                var result = new BusinessLogic.Accounts().ValidateCustomer(user_id, password);

                if (result != null)
                {
                    if (BusinessLogic.clsWebSession.useSession)
                    {
                        Session[Sessions.LoginId] = result.LoginId;
                        Session[Sessions.UserId] = user_id;
                        Session[Sessions.UserName] = result.UserName;
                        Session[Sessions.LoginType] = result.LoginType;
                        Session[Sessions.ProfileImageURL] = BusinessLogic.DataHelper.getProfileImageURL(result.ProfilePicture);
                        Session[Sessions.SubscriptionStatus] = result.SubscriptionStatus;
                        Session[Sessions.SubscriptionType] = result.SubscriptionType;

                        return Json(new { Success = 1, Info = result });
                    }
                    else
                    {
                        HttpCookie userCookie = new HttpCookie(BusinessLogic.UserWebInfoCookie.scookiename);
                        userCookie.Expires = DateTime.Now.AddYears(1);
                        userCookie[BusinessLogic.UserWebInfoCookie.sLoginId] = BusinessLogic.SBEncryption.Encrypt(result.LoginId.ToString());
                        userCookie[BusinessLogic.UserWebInfoCookie.sUserId] = BusinessLogic.SBEncryption.Encrypt(result.UserId);
                        userCookie[BusinessLogic.UserWebInfoCookie.sUserName] = BusinessLogic.SBEncryption.Encrypt(result.UserName);
                        userCookie[BusinessLogic.UserWebInfoCookie.sloginType] = BusinessLogic.SBEncryption.Encrypt(result.LoginType.ToString());
                        userCookie[BusinessLogic.UserWebInfoCookie.simageurl] = BusinessLogic.SBEncryption.Encrypt(BusinessLogic.DataHelper.getProfileImageURL(result.ProfilePicture));
                        userCookie[BusinessLogic.UserWebInfoCookie.sSubscriptionStatus] = BusinessLogic.SBEncryption.Encrypt(result.SubscriptionStatus);
                        userCookie[BusinessLogic.UserWebInfoCookie.sSubscriptionType] = BusinessLogic.SBEncryption.Encrypt(result.SubscriptionType);

                        //Response.Cookies.Add(userCookie);
                        System.Web.HttpContext.Current.Response.Cookies.Add(userCookie);

                        return Json(new { Success = 1, Info = result });
                    }
                }
                else
                {
                    return Json(new { Success = 0 });
                }
                //return Json(new { Success = 1, Info = result });
            }
            catch (Exception ae)
            {
                return Json(new { Success = 0, ErrorDescription = ae.Message });
            }
        }

        public ActionResult SignUp()
        {

            //var client = new RestClient("http://api.ipapi.com/2.57.168.0?access_key=02295f2ef58f3be3e75fc198a50b0b74&format=1");
            //client.Timeout = -1;
            //var request = new RestRequest(Method.GET);
            //IRestResponse responses = client.Execute(request);

            //string json = responses.Content;

            //var data = (JObject)JsonConvert.DeserializeObject(json);
            //string country_code = data["country_code"].Value<string>();

            //string EncPass = SBEncryption.getMD5Password("shaheryar.Naqvi@gmail.com", "user1234");

            DatabaseContext db = new DatabaseContext();
            List<Partner> partners = db.VU_Partners.ToList();


            return View(partners);
        }

        public JsonResult SignUpRequest()
        {
            try
            {
                string fullName = Request.Form["full_name"].ToString();
                string email = Request.Form["email"].ToString().Trim().ToLower();
                string phone = Request.Form["phone"].ToString();
                int country_id = DataHelper.intParse(Request.Form["country_id"]);
                int state_id = DataHelper.intParse(Request.Form["state_id"]);
                int city_id = DataHelper.intParse(Request.Form["city_id"]);
                int partner_id = DataHelper.intParse(Request.Form["referral"]);
                string password = Request.Form["password"].ToString();
                string login_type = "3";

                var result = new BusinessLogic.Accounts().SignupUser(fullName, email, phone, country_id, state_id, city_id, password, partner_id, true);

                //Session[Sessions.LoginId] = result.Key1;
                //Session[Sessions.UserId] = email;
                //Session[Sessions.UserName] = fullName;
                //Session[Sessions.ProfileImageURL] = BusinessLogic.DataHelper.getProfileImageURL(result.ProfilePicture);

                if (result.ErrorCode == "000")
                {
                    HttpCookie userCookie = new HttpCookie(BusinessLogic.UserWebInfoCookie.scookiename);
                    userCookie.Expires = DateTime.Now.AddYears(1);
                    userCookie[BusinessLogic.UserWebInfoCookie.sLoginId] = BusinessLogic.SBEncryption.Encrypt(result.Key1.ToString());
                    userCookie[BusinessLogic.UserWebInfoCookie.sUserId] = BusinessLogic.SBEncryption.Encrypt(email);
                    userCookie[BusinessLogic.UserWebInfoCookie.sUserName] = BusinessLogic.SBEncryption.Encrypt(fullName);
                    userCookie[BusinessLogic.UserWebInfoCookie.sloginType] = BusinessLogic.SBEncryption.Encrypt(login_type);
                    userCookie[BusinessLogic.UserWebInfoCookie.simageurl] = BusinessLogic.SBEncryption.Encrypt(BusinessLogic.DataHelper.getProfileImageURL(result.ProfilePicture));
                    userCookie[BusinessLogic.UserWebInfoCookie.sSubscriptionType] = BusinessLogic.SBEncryption.Encrypt(result.Key2);
                    userCookie[BusinessLogic.UserWebInfoCookie.sSubscriptionStatus] = "InActive";

                    //Response.Cookies.Add(userCookie);
                    System.Web.HttpContext.Current.Response.Cookies.Add(userCookie);
                }

                return Json(result);
            }
            catch (Exception ae)
            {
                return Json(new SmartEcommerce.Models.Admin.Other { ErrorCode = "999", ErrorDescription = ae.Message });
            }
        }

        public ActionResult SignupComplete()
        {
            BusinessLogic.SBSession.ClearSession("SessionCountryId");
            BusinessLogic.SBSession.ClearSession("SessionPlanType");
            return View();
        }

        public ActionResult SignOut()
        {
            clsWebCookie.Logout();
            clsWebSession.Logout();

            return RedirectToAction("Index");
        }


        public ActionResult ForgetPassword()
        {
            return View();
        }

        public JsonResult ForgetPasswordRequest()
        {
            try
            {
                string user_id = Request.Form["user_name"].ToString();
                var result = new BusinessLogic.Accounts().ForgetPassword(user_id);
                return Json(result);

            }
            catch (Exception ae)
            {
                return Json(new { ErrorCode = "999", ErrorDescription = ae.Message });
            }
        }

        public JsonResult VideoAddToList()
        {
            try
            {
                int LoginId = DataHelper.intParse(Request.Form["LoginId"].ToString());
                int VideoId = DataHelper.intParse(Request.Form["VideoId"].ToString());
                int IsList = DataHelper.intParse(Request.Form["IsList"].ToString());

                var result = new BusinessLogic.Accounts().VideoAddToList(LoginId, VideoId, IsList);
                return Json(result);

            }
            catch (Exception ae)
            {
                return Json(new { ErrorCode = "999", ErrorDescription = ae.Message });
            }
        }

        #endregion Accounts

        #region Category
        public ActionResult Category()
        {
            List<SmartEcommerce.Models.Product.Category> data = new BusinessLogic.Products().GetCategoryByStatus(Status.Current);
            var result = data.Where(r => r.Active == true).ToList();

            ViewBag.PageTitle = "Category";
            return View(result);
        }

        public ActionResult LibraryByCategory()
        {

            string url = Request.RawUrl;
            string[] prms = Request.RawUrl.Split('/');
            string category_name = "";

            if (prms.Count() == 3)
            {
                category_name = prms[2];
            }
            else
            {
                return RedirectToRoute("category");
            }
            ViewBag.CategoryName = category_name;
            return View();
        }

        public JsonResult LibraryByCategoryLoadMore()
        {
            try
            {
                int page = DataHelper.intParse(Request.Form["page"]);
                string category_name = DataHelper.stringParse(Request.Form["category_name"]);

                int CategoryId = DataHelper.GetCategoryIdByName(category_name);

                SmartEcommerce.Models.Product.Category search_result = new BusinessLogic.Products().GetVideosByCategoryLoadMore(CategoryId, page);
                
                if (search_result != null)
                    return Json(search_result);
                else
                    return Json("999");
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return Json("999");
            }
        }

        #endregion

        #region MyAccount

        public ActionResult EditProfile()
        {

            if (clsWebSession.HasLogin)
            {
                SmartEcommerce.Models.Common.User user = new Common().GetUserById(clsWebSession.LoginId);
                return View(user);
            }
            else
            {
                return RedirectToAction("Index");
            }
            
        }

        public JsonResult UpdateProfile()
        {
            try
            {

                if (clsWebSession.HasLogin)
                {
                    SmartEcommerce.Models.Common.User user = new Models.Common.User();

                    user.UserName = DataHelper.stringParse(Request.Form["txtName"]);
                    user.DateOfBirth = DataHelper.dateParse(Request.Form["txtDOB"]).ToString("yyyy-MM-dd");
                    user.Gender = DataHelper.stringParse(Request.Form["ddlGender"]);
                    user.StateId = DataHelper.intParse(Request.Form["ddlState"]);
                    user.CityId = DataHelper.intParse(Request.Form["ddlCity"]);
                    user.Address = DataHelper.stringParse(Request.Form["txtAddress"]);
                    user.ProfilePicture = "";

                    if (Request.Files["uploadFile"].ContentLength > 0)
                    {
                        string path = Server.MapPath("~/content/uploads/users/");
                        string img = Request.Files["uploadFile"].FileName.ToString();
                        string final_image = DateTime.Now.Ticks.ToString() + img.ToLower();
                        path += final_image;

                        Request.Files["uploadFile"].SaveAs(path);

                        user.ProfilePicture = final_image;
                        Session[Sessions.ProfileImageURL] = BusinessLogic.DataHelper.getProfileImageURL(final_image);
                    }

                    if (new Common().UpdateProfile(clsWebSession.LoginId, user))
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
                    return Json(new { ErrorCode = "888" });
                }

            }
            catch (Exception)
            {
                return Json(new { ErrorCode = "999" });
                throw;
            }
        }

        public ActionResult MyList()
        {
            if (clsWebSession.HasLogin)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }

        }

        public JsonResult MyListLoadMore()
        {
            try
            {
                int page = DataHelper.intParse(Request.Form["page"]);

                SmartEcommerce.Models.Product.Videos search_result = new BusinessLogic.Products().GetMyListLoadMore(page);

                if (search_result != null)
                    return Json(search_result);
                else
                    return Json("999");
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return Json("999");
            }
        }

        public ActionResult ChangePassword()
        {
            if (clsWebSession.HasLogin)
            {
                SmartEcommerce.Models.Common.User user = new Common().GetUserById(clsWebSession.LoginId);
                return View(user);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public JsonResult ChangePasswordRequest()
        {
            try
            {
                if (clsWebSession.HasLogin)
                {
                    int LoginId = clsWebSession.LoginId;
                    string UserId = clsWebSession.UserId;

                    string CurrentPassword = Request.Form["current_password"].ToString();
                    string NewPassword = Request.Form["new_password"].ToString();

                    var result = new BusinessLogic.Accounts().ChangePassword(LoginId, UserId, CurrentPassword, NewPassword);
                    return Json(result);
                }
                else
                {
                    return Json(new { ErrorCode = "888" });
                }
            }
            catch (Exception ae)
            {
                return Json(new SmartEcommerce.Models.Admin.Other { ErrorCode = "999", ErrorDescription = ae.Message });
            }
        }

        public ActionResult SubscriptionMy()
        {
            if (clsWebSession.HasLogin)
            {
                SmartEcommerce.Models.Common.Subscription data = new BusinessLogic.Common().GetSubscriptionPlanCustomer();

                if (data.SubType == "Free")
                {
                    return RedirectToAction("Index");
                }

                return View(data);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        #endregion

        #region Search

        public ActionResult Search()
        {
            ViewBag.PageTitle = "Search Result";
            string keyword = "";

            if (Request.QueryString["search_query"] != null)
                keyword = Request.QueryString["search_query"].ToString();


            ViewBag.PageTitle = keyword;
            return View();
        }

        public JsonResult SearchResult()
        {
            try
            {
                string keyword = Request.Form["keyword"].ToString();
                int page = DataHelper.intParse(Request.Form["page"]);

                SmartEcommerce.Models.Product.VideoSearch search_result = new BusinessLogic.Products().Search(keyword, page);

                if (search_result != null)
                    return Json(search_result);
                else
                    return Json("999");
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return Json("999");
            }
        }

        #endregion

        #region Content Pages

        public ActionResult ContactUs()
        {
            return View();
        }

        public JsonResult ContactUsRequest()
        {
            try
            {
                string Name = Request.Form["Name"].ToString();
                string Email = Request.Form["Email"].ToString();
                string PhoneNo = Request.Form["PhoneNo"].ToString();
                string Subject = Request.Form["Subject"].ToString();
                string Message = Request.Form["Message"].ToString();

                var result = new BusinessLogic.Accounts().ContactUs(Name, Email, PhoneNo, Subject, Message);
                return Json(result);

            }
            catch (Exception ae)
            {
                return Json(new { ErrorCode = "999", ErrorDescription = ae.Message });
            }
        }

        public ActionResult AboutUs()
        {
            return View();
        }

        public ActionResult Payment()
        {
            return View();
        }

        public ActionResult PrivacyAndPolicy()
        {
            return View();
        }

        public ActionResult TermsAndConditions()
        {
            return View();
        }

        public ActionResult ReturnPolicy()
        {
            return View();
        }

        public ActionResult Career()
        {
            return View();
        }

        public ActionResult FAQS()
        {
            return View();
        }

        public ActionResult NotFound()
        {
            return View();
        }

        public ActionResult ErrorPage()
        {
            if (Session["error"] == null)
            {
                return RedirectToAction("Index");
            }

            ViewData["error"] = Session["error"].ToString();
            //Session.Remove("error");

            return View();
        }

        public ActionResult Pricing()
        {
            return View();
        }

        #endregion

        #region Subscription

        //https://www.youtube.com/watch?v=G587LDnuRto
        //https://stripe.com/docs/payments/accept-a-payment-charges

        public ActionResult SubscriptionTest()
        {
            StripeConfiguration.ApiKey = "sk_test_7siwp2qdpcqWr8Ls24rs2Zr700mxVD0BIO";

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> {
                "card",
            },
                LineItems = new List<SessionLineItemOptions>
            {
                // products
                new SessionLineItemOptions {
                    Name = "Monthly Subscription",
                    //Description = "Netfive Monthly Customer Subscription",
                    Amount = 500,
                    Currency = "usd",
                    Quantity = 1,
                    //Price = "price_1H3lHOCbMNnL3FxJIJDEBLje",
                    // Product Images
                    Images = new List<string>
                    {
                        HttpUtility.UrlPathEncode("https://upload.wikimedia.org/wikipedia/commons/thumb/e/ee/.NET_Core_Logo.svg/1200px-.NET_Core_Logo.svg.png"),
                        HttpUtility.UrlPathEncode("https://miro.medium.com/max/2728/1*MfOHvI5b1XZKYTXIAKY7PQ.png")
                    }
                }
            },
                SuccessUrl = "http://localhost:53065/update-payment", // Your website, Stripe will redirect to this URL
                CancelUrl = "http://localhost:53065/payment-fail", // Your websute, If user cancel payment, Stripe will redirect to this URL
                // Your configurations
                PaymentIntentData = new SessionPaymentIntentDataOptions
                {
                    // Maybe is Order ID, Customer ID, Descriptions,...
                    Metadata = new Dictionary<string, string>
                    {
                        // For example: Order ID, Description
                        { "Order_ID", "112233" }, // Order ID in your database
                        { "Description", "Netfive Monthly Noman Subscription" } //
                    }
                }
            };

            var service = new SessionService();
            Session session = service.Create(options);

            return View(session);
        }

        public ActionResult PaymentSuccess()
        {
            return View();
        }

        public ActionResult PaymentFailed()
        {
            if (clsWebSession.HasLogin && SBSession.SessionExists("SessionErrorCode"))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult UpdatePaymentStatus()
        {
            try
            {
                StripeConfiguration.ApiKey = "sk_test_7siwp2qdpcqWr8Ls24rs2Zr700mxVD0BIO";
                Stream req = Request.InputStream;
                req.Seek(0, System.IO.SeekOrigin.Begin);
                string json = new StreamReader(req).ReadToEnd();
                //log.Info("Stripe live callback :" + json);

                // Get all Stripe events.
                var stripeEvent = EventUtility.ParseEvent(json);
                string stripeJson = stripeEvent.Data.RawObject + string.Empty;
                var childData = Charge.FromJson(stripeJson);
                var metadata = childData.Metadata;

                int orderID = -1;
                string strOrderID = string.Empty;
                if (metadata.TryGetValue("Order_ID", out strOrderID))
                {
                    int.TryParse(strOrderID, out orderID);
                    // Find your order from database.
                    // For example:
                    // Order order = db.Order.FirstOrDefault(x=>x.ID == orderID);

                }

                switch (stripeEvent.Type)
                {
                    case Events.ChargeCaptured:
                    case Events.ChargeFailed:
                    case Events.ChargePending:
                    case Events.ChargeRefunded:
                    case Events.ChargeSucceeded:
                    case Events.ChargeUpdated:
                        var charge = Charge.FromJson(stripeJson);
                        string amountBuyer = ((double)childData.Amount / 100.0).ToString();
                        if (childData.BalanceTransactionId != null)
                        {
                            long transactionAmount = 0;
                            long transactionFee = 0;
                            if (childData.BalanceTransactionId != null)
                            {
                                // Get transaction fee.
                                var balanceService = new BalanceTransactionService();
                                BalanceTransaction transaction = balanceService.Get(childData.BalanceTransactionId);
                                transactionAmount = transaction.Amount;
                                transactionFee = transaction.Fee;
                            }

                            // My status, it is defined in my system.
                            int status = 0; // Wait

                            double transactionRefund = 0;

                            // Set order status.
                            if (stripeEvent.Type == Events.ChargeFailed)
                                status = -1; // Failed
                            if (stripeEvent.Type == Events.ChargePending)
                                status = -2; // Pending
                            if (stripeEvent.Type == Events.ChargeRefunded)
                            {
                                status = -3; // Refunded
                                transactionRefund = ((double)childData.AmountRefunded / 100.0);
                            }
                            if (stripeEvent.Type == Events.ChargeSucceeded)
                                status = 1; // Completed

                            // Update data
                            // For example: database
                            // order.Status = status;
                            // db.SaveChanges();
                        }
                        break;
                    default:
                        //log.Warn("");
                        break;
                }
                return Json(new
                {
                    Code = -1,
                    Message = "Update failed."
                });
            }
            catch (Exception e)
            {
                //log.Error("UpdatePaymentStatus: " + e.Message);
                return Json(new
                {
                    Code = -100,
                    Message = "Error."
                });
            }
            
        }

        public ActionResult SubscriptionPlan()
        {
            if (clsWebSession.HasLogin)
            {
                int CountryId = 0;
                if (!SBSession.SessionExists("SessionCountryId"))
                {
                    CountryId = new BusinessLogic.Common().GetCustomerCountry();
                    SBSession.CreateSession("SessionCountryId", CountryId);
                }
                else
                {
                    CountryId = DataHelper.intParse(BusinessLogic.SBSession.GetSessionValue("SessionCountryId"));
                }

                SmartEcommerce.Models.Common.Subscription data_sub_type = new BusinessLogic.Common().GetSubscriptionPlanCustomer();

                if (data_sub_type.SubType == "Free")
                {
                    return RedirectToAction("Index");
                }

                SmartEcommerce.Models.Common.Subscription data = new BusinessLogic.Common().GetSubscriptionPlan(CountryId);

                SBSession.ClearSession("SessionErrorCode");
                SBSession.ClearSession("SessionErrorMessage");
                SBSession.ClearSession("SessionDeclineCode");

                SBSession.CreateSession("SessionPlanType", "Monthly");
                SBSession.CreateSession("SessionAmount", data.MonthlyRate);
                SBSession.CreateSession("SessionCurrency", data.Country.Currency);

                return View(data);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public JsonResult SubscriptionPlanChange()
        {
            try
            {
                string SubscriptionPlan = Request.Form["SubscriptionPlan"].ToString();
                int CountryId = DataHelper.intParse(BusinessLogic.SBSession.GetSessionValue("SessionCountryId"));

                SmartEcommerce.Models.Common.Subscription data = new BusinessLogic.Common().GetSubscriptionPlan(CountryId);

                SBSession.CreateSession("SessionPlanType", SubscriptionPlan);

                if (SubscriptionPlan == "Monthly")
                    SBSession.CreateSession("SessionAmount", data.MonthlyRate);
                else
                    SBSession.CreateSession("SessionAmount", data.YearlyRate);


                SBSession.CreateSession("SessionCurrency", data.Country.Currency);

                if (data != null)
                    return Json(data);
                else
                    return Json("999");
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return Json("999");
            }
        }

        public JsonResult SubscriptionPlanActive()
        {
            try
            {
                int CustomerId = 0;
                if (!SBSession.SessionExists("SessionCustomerId"))
                {
                    CustomerId = clsWebSession.LoginId;
                    SBSession.CreateSession("SessionCustomerId", CustomerId);
                }
                else
                {
                    CustomerId = DataHelper.intParse(BusinessLogic.SBSession.GetSessionValue("SessionCustomerId"));
                }

                string SubscriptionPlan = Request.Form["SubscriptionPlan"].ToString();
                decimal SubscriptionPrice = DataHelper.decimalParse(Request.Form["SubscriptionPrice"]);

                string ErrorCode = new Common().ActiveSubscriptionPlan(CustomerId, SubscriptionPlan, SubscriptionPrice);

                return Json(new { ErrorCode = ErrorCode });
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return Json("999");
            }
        }

        public JsonResult SubscriptionPlanCancel()
        {
            try
            {
                int CustomerId = clsWebSession.LoginId;

                string ErrorCode = new Common().CancelSubscriptionPlan(CustomerId);

                if (ErrorCode == "000")
                {
                    if (BusinessLogic.clsWebSession.useSession)
                    {
                        Session[Sessions.SubscriptionStatus] = "Cancelled";
                    }
                    else
                    {
                        HttpCookie userCookie = new HttpCookie(BusinessLogic.UserWebInfoCookie.scookiename);
                        userCookie.Expires = DateTime.Now.AddDays(3);

                        userCookie[BusinessLogic.UserWebInfoCookie.sLoginId] = BusinessLogic.SBEncryption.Encrypt(clsWebCookie.LoginId.ToString());
                        userCookie[BusinessLogic.UserWebInfoCookie.sUserId] = BusinessLogic.SBEncryption.Encrypt(clsWebCookie.UserId);
                        userCookie[BusinessLogic.UserWebInfoCookie.sUserName] = BusinessLogic.SBEncryption.Encrypt(clsWebCookie.UserName);
                        userCookie[BusinessLogic.UserWebInfoCookie.sloginType] = BusinessLogic.SBEncryption.Encrypt(clsWebCookie.LoginType.ToString());
                        userCookie[BusinessLogic.UserWebInfoCookie.simageurl] = BusinessLogic.SBEncryption.Encrypt(BusinessLogic.DataHelper.getProfileImageURL(clsWebCookie.ProfileImageURL));
                        userCookie[BusinessLogic.UserWebInfoCookie.sSubscriptionStatus] = BusinessLogic.SBEncryption.Encrypt("Cancelled");
                        userCookie[BusinessLogic.UserWebInfoCookie.sSubscriptionType] = BusinessLogic.SBEncryption.Encrypt(clsWebCookie.SubscriptionType);
                        
                        System.Web.HttpContext.Current.Response.Cookies.Add(userCookie);
                    }
                }

                return Json(new { ErrorCode = ErrorCode });
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return Json("999");
            }
        }

        public JsonResult SubscriptionPlanReActive()
        {
            try
            {
                int CustomerId = clsWebSession.LoginId;

                string ErrorCode = new Common().ReActiveSubscriptionPlan(CustomerId);

                if (ErrorCode == "000")
                {
                    if (BusinessLogic.clsWebSession.useSession)
                    {
                        Session[Sessions.SubscriptionStatus] = "Active";
                    }
                    else
                    {
                        HttpCookie userCookie = new HttpCookie(BusinessLogic.UserWebInfoCookie.scookiename);
                        userCookie.Expires = DateTime.Now.AddDays(3);

                        userCookie[BusinessLogic.UserWebInfoCookie.sLoginId] = BusinessLogic.SBEncryption.Encrypt(clsWebCookie.LoginId.ToString());
                        userCookie[BusinessLogic.UserWebInfoCookie.sUserId] = BusinessLogic.SBEncryption.Encrypt(clsWebCookie.UserId);
                        userCookie[BusinessLogic.UserWebInfoCookie.sUserName] = BusinessLogic.SBEncryption.Encrypt(clsWebCookie.UserName);
                        userCookie[BusinessLogic.UserWebInfoCookie.sloginType] = BusinessLogic.SBEncryption.Encrypt(clsWebCookie.LoginType.ToString());
                        userCookie[BusinessLogic.UserWebInfoCookie.simageurl] = BusinessLogic.SBEncryption.Encrypt(BusinessLogic.DataHelper.getProfileImageURL(clsWebCookie.ProfileImageURL));
                        userCookie[BusinessLogic.UserWebInfoCookie.sSubscriptionStatus] = BusinessLogic.SBEncryption.Encrypt("Active");
                        userCookie[BusinessLogic.UserWebInfoCookie.sSubscriptionType] = BusinessLogic.SBEncryption.Encrypt(clsWebCookie.SubscriptionType);

                        System.Web.HttpContext.Current.Response.Cookies.Add(userCookie);
                    }
                }

                return Json(new { ErrorCode = ErrorCode });
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return Json("999");
            }
        }

        public ActionResult SubscriptionPayment()
        {
            return View();
        }

        public ActionResult checkout()
        {

            if (clsWebSession.HasLogin)
            {
                if (SBSession.SessionExists("SessionPlanType") && SBSession.GetSessionValue("SessionPlanType").ToString() != "")
                {

                    SmartEcommerce.Models.Common.Subscription data_sub_type = new BusinessLogic.Common().GetSubscriptionPlanCustomer();

                    if (data_sub_type.SubType == "Free")
                    {
                        return RedirectToAction("Index");
                    }

                    return View();
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public JsonResult SavePaymentSubscription()
        {
            try
            {
                string error_code = "", error_message = "", decline_code = "", charge = "";

                string customer_id = Request.Form["customer_id"].ToString();
                decimal stripe_amount = DataHelper.decimalParse(Request.Form["stripe_amount"]);
                string client_secret = Request.Form["client_secret"].ToString();
                string stripe_currency = Request.Form["stripe_currency"].ToString();
                string payment_intent_id = Request.Form["payment_intent_id"].ToString();
                string payment_method = Request.Form["payment_method"].ToString();
                string payment_status = Request.Form["payment_status"].ToString();

                if (payment_status != "succeeded")
                {
                    error_code = Request.Form["error_code"].ToString();
                    error_message = Request.Form["error_message"].ToString();
                    decline_code = ""; //Request.Form["decline_code"].ToString();
                    charge = Request.Form["charge"].ToString();

                    SBSession.CreateSession("SessionErrorCode", error_code);
                    SBSession.CreateSession("SessionErrorMessage", error_message);
                    SBSession.CreateSession("SessionDeclineCode", decline_code);

                    payment_status = "failed";
                }

                string ErrorCode = new Common().PaymentSubscription(customer_id, stripe_amount, client_secret, stripe_currency, payment_intent_id, payment_method, payment_status,
                                                                    error_code, error_message, decline_code, charge);




                return Json(new { ErrorCode = ErrorCode });
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return Json("999");
            }
        }

        #endregion

        #region Subscriber

        public JsonResult SubscribeRequest()
        {
            try
            {
                int SubscriberId = DataHelper.intParse(Request.Form["subscriber_id"]);
                int LoginId = DataHelper.intParse(Request.Form["login_id"]);
                int Subscribe = DataHelper.intParse(Request.Form["subscribe"]);

                dynamic result = new Common().SubscriberRequest(SubscriberId, LoginId, Subscribe);

                if (result == null)
                {
                    return Json(new { ErrorCode = "999" });
                }
                else
                {
                    return Json(new { ErrorCode = "000", TotalSubscribers = result });
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return Json(new { ErrorCode = "999" });
            }
        }

        public JsonResult SubscribeNotification()
        {
            try
            {
                int SubscriberId = DataHelper.intParse(Request.Form["subscriber_id"]);
                bool notification = DataHelper.boolParse(Request.Form["notification"]);

                bool result = new Common().SubscriberNotification(SubscriberId, clsWebSession.LoginId, notification);

                if (result == false)
                {
                    return Json(new { ErrorCode = "999" });
                }
                else
                {
                    return Json(new { ErrorCode = "000" });
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return Json(new { ErrorCode = "999" });
            }
        }

        #endregion

        #region Partner

        public ActionResult PartnerRequest()
        {
            return View();
        }

        public JsonResult SavePartnerRequest()
        {
            try
            {
                SmartEcommerce.Models.Common.Partner partner = new Models.Common.Partner();

                partner.FullName = Request.Form["txtfullname"].ToString();
                partner.PartnerType.Id = DataHelper.intParse(Request.Form["ddltype"].ToString());
                partner.PartnerCategory.Id = DataHelper.intParse(Request.Form["ddlcategory"]);
                partner.ContactPerson = Request.Form["txtcontactperson"].ToString();
                partner.Telephone = Request.Form["txttelephone"].ToString();
                partner.MobileNo = Request.Form["txtmobileno"].ToString();
                partner.EmailAddress = Request.Form["txtemailaddress"].ToString();
                partner.Country.Id = DataHelper.intParse(Request.Form["ddlcountry"]);
                partner.State.Id = DataHelper.intParse(Request.Form["ddlstate"]);
                partner.City.Id = DataHelper.intParse(Request.Form["ddlcity"]);
                partner.Address = Request.Form["txtaddress"].ToString();
                partner.PartnerContentType.Id = DataHelper.intParse(Request.Form["ddlcontentype"]);
                //partner.PartnerContentTypeUpload.Id = DataHelper.intParse(Request.Form["ddlcontentypeupload"]);


                string content_data = Request.Form["ddlcontentypeupload"].ToString();
                string[] content_array = content_data.Split(',');

                foreach (string row in content_array)
                {
                    partner.PartnerContentTypeUpload.Add(new SmartEcommerce.Models.Product.Category
                    {
                        ContentTypeUploadId = row.ToString()
                    });
                }

                string ErrorCode = new Common().SavePartner(partner);

                return Json(new { ErrorCode = ErrorCode });
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return Json(new { ErrorCode = "999" });
            }
        }

        public ActionResult PartnerRequestComplete()
        {
            return View();
        }

        #endregion

        #region Like Dislike

        public JsonResult LikeVideo()
        {
            long VideoId = DataHelper.longParse(Request.Form["video_id"]);
            int Type = DataHelper.intParse(Request.Form["type"]);
            bool IsAdd = DataHelper.boolParse(Request.Form["is_add"]);
            bool IsLiveStreaming = false;
            if (Request.Form["is_live_streaming"] != null)
                IsLiveStreaming = DataHelper.boolParse(Request.Form["is_live_streaming"]);


            var result = new Common().LikeVideo(VideoId, clsWebSession.LoginId, Type, IsAdd, IsLiveStreaming);
            if (result == null)
            {
                return Json(new { success = false });
            }
            else
            {
                return Json(new { success = true, data = result });
            }
        }

        #endregion Like Dislike

        #region Country

        public JsonResult GetCountryByIp()
        {
            string ip_address = Request.Form["ip"].ToString();

            var result = new Common().GetCountryBySession(ip_address);

            return Json(new { data = result, country_id = BusinessLogic.SBSession.GetSessionValue("SessionCountryId") });
        }

        #endregion Country

        #region Service

        [HttpGet]
        public JsonResult ExpireTrialUsers()
        {
            new Common().ExpireTrialUsers();

            return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult ChargeCustomers()
        {
            Stripe.StripePaymentIntent.ChargeSubscriptionPayment();

            //StripeConfiguration.ApiKey = BusinessLogic.clsWebSession.StripeSecretKey;
            //var service = new CustomerService();
            //var result = service.Get("cus_HlyrZhw5GOZ0Pc");

            return Json(new { success = 1, }, JsonRequestBehavior.AllowGet);
        }

        #endregion Service

    }
}