using BusinessLogic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Web.Http;

namespace SmartEcommerce.Controllers
{
    public class MobileController : ApiController
    {
        [AllowAnonymous]
        [HttpPost]
        [Route("api/mobile/signup")]
        public HttpResponseMessage SignUp([FromBody]JObject data)
        {
            if (data["full_name"] == null || data["email"] == null || data["country_id"] == null ||
                data["password"] == null || data["referral"] == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotAcceptable, new { error = "Required parameters not supplied" });
            }

            string FullName = data["full_name"].ToString();
            string Email = data["email"].ToString();
            string Phone = ""; // data["phone"].ToString();
            int CountryId = DataHelper.intParse(data["country_id"]);
            int StateId = 0; // DataHelper.intParse(data["state_id"]);
            int CityId = 0; // DataHelper.intParse(data["city_id"]);
            string Password = data["password"].ToString();
            int Referral = DataHelper.intParse(data["referral"]);

            dynamic signup_validation = new BusinessLogic.MobileApi().ValidateSignUpEmailAndMobile(Email, Phone);
            if (signup_validation.ToString() == "False")
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = "Exception" });
            }
            else
            {
                bool EmailExists = DataHelper.boolParse(((System.Reflection.PropertyInfo)signup_validation.GetType().GetProperty("EmailExists")).GetValue(signup_validation, null));
                bool PhoneExists = DataHelper.boolParse(((System.Reflection.PropertyInfo)signup_validation.GetType().GetProperty("PhoneExists")).GetValue(signup_validation, null));
                StringBuilder sb = new StringBuilder();
                if (EmailExists)
                {
                    sb.AppendLine("Email alreadyy exists");
                }
                //if (PhoneExists)
                //{
                //    sb.AppendLine("Phone alreadyy exists");
                //}

                if (sb.Length > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Found, new { error = sb.ToString() });
                }
            }

            SmartEcommerce.Models.Admin.Other result = new BusinessLogic.Accounts().SignupUser(FullName, Email, Phone, CountryId, StateId, CityId, Password, Referral);
            if (result.ErrorCode == "000" || result.ErrorCode == "003")
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { LoginId = result.Key1, SubscriptionType = result.Key2, ErrorCode = result.ErrorCode });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = "Exception" });
            }
        }

        [Authorize(Roles = "customer")]
        [HttpPost]
        [Route("api/mobile/get-subscription-plan")]
        public HttpResponseMessage GetSubscriptionPlan([FromBody]JObject data)
        {

            var identity = (ClaimsIdentity)User.Identity;
            var roles = identity.Claims.
                Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value);


            int LoginId = DataHelper.intParse(identity.Claims.Where(r => r.Type == "loginid").FirstOrDefault().Value);

            object result = new BusinessLogic.MobileApi().GetSubscriptionPlan(LoginId);

            if (result.ToString() == "False")
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = "unknown exception" });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/mobile/validate-email-phone")]
        public HttpResponseMessage ValidateEmailPhone([FromBody]JObject data)
        {
            if (data["email"] == null || data["phone"] == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotAcceptable, new { error = "Required parameters not supplied" });
            }

            string Email = data["email"].ToString();
            string Phone = data["phone"].ToString();

            object result = new BusinessLogic.MobileApi().ValidateSignUpEmailAndMobile(Email, Phone);

            if (result.ToString() == "False")
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = "unknown exception" });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
        }


        [Authorize(Roles = "customer")]
        [HttpPost]
        [Route("api/mobile/categories")]
        public HttpResponseMessage GetCategoryList()
        {
            try
            {
                List<SmartEcommerce.Models.Product.Category> data = new BusinessLogic.Products().GetCategoryByStatus(1);

                var result = data.Where(r => r.Active == true).OrderBy(r => r.Title);

                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ae)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = ae.Message });
            }
        }


        [Authorize(Roles = "customer")]
        [HttpPost]
        [Route("api/mobile/library")]
        public HttpResponseMessage GetLibraryList([FromBody]JObject data)
        {
            try
            {
                if (data["categories"] == null || data["page"] == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotAcceptable, new { error = "Required parameters not supplied" });
                }


                var identity = (ClaimsIdentity)User.Identity;
                var roles = identity.Claims.
                    Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value);

                
                int LoginId = DataHelper.intParse(identity.Claims.Where(r => r.Type == "loginid").FirstOrDefault().Value);
                string categories = data["categories"].ToString();
                int page = DataHelper.intParse(data["page"]);


                var result = new BusinessLogic.MobileApi().GetLibrary(categories, LoginId, page);

                if (result == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = "Exception" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
                

                //List<SmartEcommerce.Models.Product.Category> data = new BusinessLogic.Products().GetCategoryByStatus(1);

                //var result = data.Where(r => r.Active == true).OrderBy(r => r.Title);

                
            }
            catch (Exception ae)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = ae.Message });
            }
        }


        [Authorize(Roles = "customer")]
        [HttpPost]
        [Route("api/mobile/librarydetail")]
        public HttpResponseMessage GetLibraryDetail([FromBody]JObject data)
        {
            try
            {
                if (data["video_id"] == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotAcceptable, new { error = "Required parameters not supplied" });
                }


                var identity = (ClaimsIdentity)User.Identity;
                var roles = identity.Claims.
                    Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value);

                int LoginId = DataHelper.intParse(identity.Claims.Where(r => r.Type == "loginid").FirstOrDefault().Value);

                if (new BusinessLogic.MobileApi().CheckForSubscription(LoginId))
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, new { error = "you have no rights" });
                }

                
                long VideoId = DataHelper.longParse(data["video_id"]);


                var result = new BusinessLogic.MobileApi().GetLibraryDetail(LoginId, VideoId);

                if (result == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = "Exception" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
            }
            catch (Exception ae)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = ae.Message });
            }
        }


        [Authorize(Roles = "customer")]
        [HttpPost]
        [Route("api/mobile/library-related")]
        public HttpResponseMessage GetLibraryRelated([FromBody]JObject data)
        {
            try
            {
                if (data["video_id"] == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotAcceptable, new { error = "Required parameters not supplied" });
                }


                var identity = (ClaimsIdentity)User.Identity;
                var roles = identity.Claims.
                    Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value);

                int LoginId = DataHelper.intParse(identity.Claims.Where(r => r.Type == "loginid").FirstOrDefault().Value);

               
                long VideoId = DataHelper.longParse(data["video_id"]);


                var result = new BusinessLogic.MobileApi().GetLibraryRelated(LoginId, VideoId);

                if (result == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = "Exception" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
            }
            catch (Exception ae)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = ae.Message });
            }
        }


        [Authorize(Roles = "customer")]
        [HttpPost]
        [Route("api/mobile/livestreaming")]
        public HttpResponseMessage GetLiveStreaming([FromBody]JObject data)
        {
            try
            {
                var identity = (ClaimsIdentity)User.Identity;
                var roles = identity.Claims.
                    Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value);


                int LoginId = DataHelper.intParse(identity.Claims.Where(r => r.Type == "loginid").FirstOrDefault().Value);

                var result = new BusinessLogic.MobileApi().GetLiveStreaming(LoginId);

                if (result == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = "Exception" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
            }
            catch (Exception ae)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = ae.Message });
            }
        }


        [Authorize(Roles = "customer")]
        [HttpPost]
        [Route("api/mobile/programs")]
        public HttpResponseMessage GetPrograms([FromBody]JObject data)
        {
            try
            {
                var identity = (ClaimsIdentity)User.Identity;
                var roles = identity.Claims.
                    Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value);


                int LoginId = DataHelper.intParse(identity.Claims.Where(r => r.Type == "loginid").FirstOrDefault().Value);

                var result = new BusinessLogic.MobileApi().GetPrograms();

                if (result == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = "Exception" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
            }
            catch (Exception ae)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = ae.Message });
            }
        }


        [Authorize(Roles = "customer")]
        [HttpPost]
        [Route("api/mobile/programs-new-episodes")]
        public HttpResponseMessage GetProgramsNewEpisodes([FromBody]JObject data)
        {
            try
            {
                var identity = (ClaimsIdentity)User.Identity;
                var roles = identity.Claims.
                    Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value);


                int LoginId = DataHelper.intParse(identity.Claims.Where(r => r.Type == "loginid").FirstOrDefault().Value);

                var result = new BusinessLogic.MobileApi().GetProgramsNewEpisodes();

                if (result == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = "Exception" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
            }
            catch (Exception ae)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = ae.Message });
            }
        }


        [Authorize(Roles = "customer")]
        [HttpPost]
        [Route("api/mobile/program-detail")]
        public HttpResponseMessage GetProgramDetail([FromBody]JObject data)
        {
            try
            {
                if (data["program_id"] == null || data["page"] == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotAcceptable, new { error = "Required parameters not supplied" });
                }

                var identity = (ClaimsIdentity)User.Identity;
                var roles = identity.Claims.
                    Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value);

                int LoginId = DataHelper.intParse(identity.Claims.Where(r => r.Type == "loginid").FirstOrDefault().Value);

                int ProgramId = DataHelper.intParse(data["program_id"]);
                int Page = DataHelper.intParse(data["page"]);


                var result = new BusinessLogic.MobileApi().GetProgramsDetail(ProgramId, Page);

                if (result == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = "Exception" });
                }
                else if (result.ToString() == "False")
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { error = "program not found" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
            }
            catch (Exception ae)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = ae.Message });
            }
        }


        [Authorize(Roles = "customer")]
        [HttpPost]
        [Route("api/mobile/search")]
        public HttpResponseMessage GetSearchResult([FromBody]JObject data)
        {
            try
            {
                if (data["keyword"] == null || data["page"] == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotAcceptable, new { error = "Required parameters not supplied" });
                }

                var identity = (ClaimsIdentity)User.Identity;
                var roles = identity.Claims.
                    Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value);

                int LoginId = DataHelper.intParse(identity.Claims.Where(r => r.Type == "loginid").FirstOrDefault().Value);

                string Keyword = data["keyword"].ToString();
                int Page = DataHelper.intParse(data["page"]);


                var result = new BusinessLogic.MobileApi().GetSearchResult(Keyword, Page);

                if (result == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = "Exception" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
            }
            catch (Exception ae)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = ae.Message });
            }
        }


        [Authorize(Roles = "customer")]
        [HttpPost]
        [Route("api/mobile/change-password")]
        public HttpResponseMessage ChangePassword([FromBody]JObject data)
        {
            try
            {
                if (data["current_password"] == null || data["new_password"] == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotAcceptable, new { error = "Required parameters not supplied" });
                }

                var identity = (ClaimsIdentity)User.Identity;
                var roles = identity.Claims.
                    Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value);
                

                int LoginId = DataHelper.intParse(identity.Claims.Where(r => r.Type == "loginid").FirstOrDefault().Value);
                string UserId = identity.Claims.Where(r => r.Type == "username").FirstOrDefault().Value.ToString();

                string CurrentPassword = data["current_password"].ToString().Trim();
                string NewPassword = data["new_password"].ToString().Trim();

                if (CurrentPassword == NewPassword)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = "old and new password cannot be same" });
                }

                var result = new BusinessLogic.MobileApi().ChangePassword(LoginId, UserId, CurrentPassword, NewPassword);

                if (result.ToString() == "False")
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = "Invalid old password" });
                }
                else if (result.ToString() == "True")
                {
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else 
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = "Exception" });
                }
            }
            catch (Exception ae)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = ae.Message });
            }
        }


        [Authorize(Roles = "customer")]
        [HttpPost]
        [Route("api/mobile/profile-info")]
        public HttpResponseMessage GetProfileInfo([FromBody]JObject data)
        {
            try
            {
                var identity = (ClaimsIdentity)User.Identity;
                var roles = identity.Claims.
                    Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value);


                int LoginId = DataHelper.intParse(identity.Claims.Where(r => r.Type == "loginid").FirstOrDefault().Value);

                var result = new BusinessLogic.MobileApi().GetProfileInfo(LoginId);

                if (result == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = "exception" });
                }
                else 
                {
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
            }
            catch (Exception ae)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = ae.Message });
            }
        }


        [Authorize(Roles = "customer")]
        [HttpPost]
        [Route("api/mobile/update-profile")]
        public HttpResponseMessage UpdateProfile()
        {
            try
            {
                var httpRequest = System.Web.HttpContext.Current.Request;

                if (httpRequest.Form["full_name"] == null || httpRequest.Form["phone"] == null || httpRequest.Form["dob"] == null 
                    || httpRequest.Form["country_id"] == null || httpRequest.Form["state_id"] == null || httpRequest.Form["city_id"] == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotAcceptable, new { error = "Required parameters not supplied" });
                }

                string UserName = httpRequest.Form["full_name"].ToString();
                string Phone = httpRequest.Form["phone"].ToString();
                DateTime DateOfBirth = DataHelper.dateParse(httpRequest.Form["dob"]);
                int CountryId = DataHelper.intParse(httpRequest.Form["country_id"]);
                int StateId = DataHelper.intParse(httpRequest.Form["state_id"]);
                int CityId = DataHelper.intParse(httpRequest.Form["city_id"]);


                var identity = (ClaimsIdentity)User.Identity;
                var roles = identity.Claims.
                    Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value);


                int LoginId = DataHelper.intParse(identity.Claims.Where(r => r.Type == "loginid").FirstOrDefault().Value);

                var result = new BusinessLogic.MobileApi().UpdateProfile(LoginId, UserName, "", Phone, DateOfBirth, CountryId, StateId, CityId,
                    httpRequest.Files);

                if (result == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = "exception" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { error = 200 });
                }
            }
            catch (Exception ae)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = ae.Message });
            }
        }


        [AllowAnonymous]
        [HttpPost]
        [Route("api/mobile/regions-countries")]
        public HttpResponseMessage GetRegionsCountries()
        {
            try
            {
                //var identity = (ClaimsIdentity)User.Identity;
                //var roles = identity.Claims.
                //    Where(c => c.Type == ClaimTypes.Role)
                //    .Select(c => c.Value);


                //int LoginId = DataHelper.intParse(identity.Claims.Where(r => r.Type == "loginid").FirstOrDefault().Value);

                var result = new BusinessLogic.MobileApi().GetRegionsCountryStates();

                if (result == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = "exception" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
            }
            catch (Exception ae)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = ae.Message });
            }
        }


        [AllowAnonymous]
        [HttpPost]
        [Route("api/mobile/regions-cities")]
        public HttpResponseMessage GetRegionsCities([FromBody]JObject data)
        {
            try
            {
                if (data["state_id"] == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotAcceptable, new { error = "Required parameters not supplied" });
                }

                int StateId = DataHelper.intParse(data["state_id"]);

                var result = new BusinessLogic.MobileApi().GetRegionsCitiesByState(StateId);

                if (result == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = "exception" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
            }
            catch (Exception ae)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = ae.Message });
            }
        }


        [Authorize(Roles = "customer")]
        [HttpPost]
        [Route("api/mobile/add-video-to-list")]
        public HttpResponseMessage AddVideoToMyList([FromBody]JObject data)
        {
            try
            {
                if (data["video_id"] == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotAcceptable, new { error = "Required parameters not supplied" });
                }

                var identity = (ClaimsIdentity)User.Identity;
                var roles = identity.Claims.
                    Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value);


                int LoginId = DataHelper.intParse(identity.Claims.Where(r => r.Type == "loginid").FirstOrDefault().Value);
                long VideoId = DataHelper.longParse(data["video_id"]);

                var result = new BusinessLogic.MobileApi().AddVideoToMyList(LoginId, VideoId);

                if (result == false)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = "exception" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
            }
            catch (Exception ae)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = ae.Message });
            }
        }


        [Authorize(Roles = "customer")]
        [HttpPost]
        [Route("api/mobile/remove-video-from-list")]
        public HttpResponseMessage RemoveVideoFromMyList([FromBody]JObject data)
        {
            try
            {
                if (data["video_id"] == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotAcceptable, new { error = "Required parameters not supplied" });
                }

                var identity = (ClaimsIdentity)User.Identity;
                var roles = identity.Claims.
                    Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value);


                int LoginId = DataHelper.intParse(identity.Claims.Where(r => r.Type == "loginid").FirstOrDefault().Value);
                long VideoId = DataHelper.longParse(data["video_id"]);

                var result = new BusinessLogic.MobileApi().RemoveVideoFromMyList(LoginId, VideoId);

                if (result == false)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = "exception" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
            }
            catch (Exception ae)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = ae.Message });
            }
        }


        [Authorize(Roles = "customer")]
        [HttpPost]
        [Route("api/mobile/my-list")]
        public HttpResponseMessage GetMyListLibrary([FromBody]JObject data)
        {
            try
            {
                if (data["page"] == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotAcceptable, new { error = "Required parameters not supplied" });
                }


                var identity = (ClaimsIdentity)User.Identity;
                var roles = identity.Claims.
                    Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value);


                int LoginId = DataHelper.intParse(identity.Claims.Where(r => r.Type == "loginid").FirstOrDefault().Value);
                int page = DataHelper.intParse(data["page"]);


                var result = new BusinessLogic.MobileApi().GetMyListLibrary(LoginId, page);

                if (result == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = "Exception" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
            }
            catch (Exception ae)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = ae.Message });
            }
        }


        [Authorize(Roles = "customer")]
        [HttpPost]
        [Route("api/mobile/upload-video")]
        public HttpResponseMessage UploadVideo()
        {
            try
            {
                var httpRequest = System.Web.HttpContext.Current.Request;

                if (httpRequest.Form["title"] == null || httpRequest.Form["program_id"] == null || httpRequest.Form["category_id"] == null
                    || httpRequest.Form["description"] == null || httpRequest.Files.Count == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.NotAcceptable, new { error = "Required parameters not supplied" });
                }

                string Title = httpRequest.Form["title"].ToString();
                int ProgramId = DataHelper.intParse(httpRequest.Form["program_id"]);
                int CategoryId = DataHelper.intParse(httpRequest.Form["category_id"]);
                string Description = httpRequest.Form["description"].ToString();


                var identity = (ClaimsIdentity)User.Identity;
                var roles = identity.Claims.
                    Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value);


                int LoginId = DataHelper.intParse(identity.Claims.Where(r => r.Type == "loginid").FirstOrDefault().Value);

                var result = new BusinessLogic.MobileApi().UploadVideo(LoginId, Title, ProgramId, CategoryId, Description, httpRequest.Files);

                if (result == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = "exception" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { error = 200 });
                }
            }
            catch (Exception ae)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = ae.Message });
            }
        }


        [Authorize(Roles = "customer")]
        [HttpPost]
        [Route("api/mobile/edit-video")]
        public HttpResponseMessage EditVideo()
        {
            try
            {
                var httpRequest = System.Web.HttpContext.Current.Request;

                if (httpRequest.Form["title"] == null || httpRequest.Form["program_id"] == null || httpRequest.Form["category_id"] == null
                    || httpRequest.Form["description"] == null || httpRequest.Files.Count == 0 || httpRequest.Form["video_id"] == null || httpRequest.Form["video_id"] == "")
                {
                    return Request.CreateResponse(HttpStatusCode.NotAcceptable, new { error = "Required parameters not supplied" });
                }

                int VideoId = DataHelper.intParse(httpRequest.Form["video_id"]);
                string Title = httpRequest.Form["title"].ToString();
                int ProgramId = DataHelper.intParse(httpRequest.Form["program_id"]);
                int CategoryId = DataHelper.intParse(httpRequest.Form["category_id"]);
                string Description = httpRequest.Form["description"].ToString();


                var identity = (ClaimsIdentity)User.Identity;
                var roles = identity.Claims.
                    Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value);


                int LoginId = DataHelper.intParse(identity.Claims.Where(r => r.Type == "loginid").FirstOrDefault().Value);

                var result = new BusinessLogic.MobileApi().EditVideo(LoginId, VideoId, Title, ProgramId, CategoryId, Description, httpRequest.Files);

                if (result == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = "exception" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { error = 200 });
                }
            }
            catch (Exception ae)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = ae.Message });
            }
        }

        [Authorize(Roles = "customer")]
        [HttpPost]
        [Route("api/mobile/my-videos")]
        public HttpResponseMessage GetMyVideos([FromBody]JObject data)
        {
            try
            {
                if (data["page"] == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotAcceptable, new { error = "Required parameters not supplied" });
                }


                var identity = (ClaimsIdentity)User.Identity;
                var roles = identity.Claims.
                    Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value);


                int LoginId = DataHelper.intParse(identity.Claims.Where(r => r.Type == "loginid").FirstOrDefault().Value);
                int page = DataHelper.intParse(data["page"]);


                var result = new BusinessLogic.MobileApi().GetMyVideos(LoginId, page);

                if (result == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = "Exception" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
            }
            catch (Exception ae)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = ae.Message });
            }
        }

        [Authorize(Roles = "customer")]
        [HttpPost]
        [Route("api/mobile/my-videos-detail")]
        public HttpResponseMessage GetMyVideosDetail([FromBody]JObject data)
        {
            try
            {
                if (data["video_id"] == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotAcceptable, new { error = "Required parameters not supplied" });
                }


                var identity = (ClaimsIdentity)User.Identity;
                var roles = identity.Claims.
                    Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value);

                int LoginId = DataHelper.intParse(identity.Claims.Where(r => r.Type == "loginid").FirstOrDefault().Value);

                long VideoId = DataHelper.longParse(data["video_id"]);

                var result = new BusinessLogic.MobileApi().GetMyVideosDetail(LoginId, VideoId);

                if (result == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = "Exception" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
            }
            catch (Exception ae)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = ae.Message });
            }
        }

        [Authorize(Roles = "customer")]
        [HttpPost]
        [Route("api/mobile/remove-my-video")]
        public HttpResponseMessage RemoveMyVideo([FromBody]JObject data)
        {
            try
            {
                if (data["video_id"] == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotAcceptable, new { error = "Required parameters not supplied" });
                }

                var identity = (ClaimsIdentity)User.Identity;
                var roles = identity.Claims.
                    Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value);


                int LoginId = DataHelper.intParse(identity.Claims.Where(r => r.Type == "loginid").FirstOrDefault().Value);
                long VideoId = DataHelper.longParse(data["video_id"]);

                var result = new BusinessLogic.MobileApi().RemoveMyVideo(LoginId, VideoId);

                if (result == false)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = "exception" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
            }
            catch (Exception ae)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = ae.Message });
            }
        }


        [Authorize(Roles = "customer")]
        [HttpPost]
        [Route("api/mobile/send-message-on-video")]
        public HttpResponseMessage SendUserMessage([FromBody]JObject data)
        {
            try
            {
                if (data["video_id"] == null || data["message"] == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotAcceptable, new { error = "Required parameters not supplied" });
                }

                var identity = (ClaimsIdentity)User.Identity;
                var roles = identity.Claims.
                    Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value);


                int LoginId = DataHelper.intParse(identity.Claims.Where(r => r.Type == "loginid").FirstOrDefault().Value);
                long VideoId = DataHelper.longParse(data["video_id"]);
                string Message = data["message"].ToString();

                var result = new BusinessLogic.MobileApi().SendUserMessage(LoginId, VideoId, Message);

                if (result == false)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = "exception" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
            }
            catch (Exception ae)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = ae.Message });
            }
        }


        [Authorize(Roles = "customer")]
        [HttpPost]
        [Route("api/mobile/programs-by-user")]
        public HttpResponseMessage GetProgramsByUser([FromBody]JObject data)
        {
            try
            {
                if (data["user_id"] == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotAcceptable, new { error = "Required parameters not supplied" });
                }

                var identity = (ClaimsIdentity)User.Identity;
                var roles = identity.Claims.
                    Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value);

                int UserId = DataHelper.intParse(data["user_id"]);
                int LoginId = DataHelper.intParse(identity.Claims.Where(r => r.Type == "loginid").FirstOrDefault().Value);

                var result = new BusinessLogic.MobileApi().GetProgramsByUser(UserId);

                if (result == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = "Exception" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
            }
            catch (Exception ae)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = ae.Message });
            }
        }


        [Authorize(Roles = "customer")]
        [HttpPost]
        [Route("api/mobile/videos-by-user")]
        public HttpResponseMessage GetVideosByUser([FromBody]JObject data)
        {
            try
            {
                if (data["page"] == null || data["user_id"] == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotAcceptable, new { error = "Required parameters not supplied" });
                }


                var identity = (ClaimsIdentity)User.Identity;
                var roles = identity.Claims.
                    Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value);


                int LoginId = DataHelper.intParse(identity.Claims.Where(r => r.Type == "loginid").FirstOrDefault().Value);
                int UserId = DataHelper.intParse(data["user_id"]);
                int page = DataHelper.intParse(data["page"]);


                var result = new BusinessLogic.MobileApi().GetVideosByUser(UserId, page);

                if (result == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = "Exception" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
            }
            catch (Exception ae)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = ae.Message });
            }
        }


        [AllowAnonymous]
        [HttpGet]
        [Route("api/mobile/referrals")]
        public HttpResponseMessage GetReferrals()
        {
            try
            {
                var result = new DatabaseContext().VU_Partners.ToList();
                
                if (result == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = "Exception" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
            }
            catch (Exception ae)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = ae.Message });
            }
        }


        [AllowAnonymous]
        [HttpPost]
        [Route("api/mobile/country-by-ip")]
        public HttpResponseMessage GetCountryByIp([FromBody]JObject data)
        {
            try
            {
                if (data["ip"] == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotAcceptable, new { error = "Required parameters not supplied" });
                }


                string ip_address = data["ip"].ToString();

                var result = new Common().GetCountryBySession(ip_address, true);

                if (result == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = "Exception" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
            }
            catch (Exception ae)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = ae.Message });
            }
        }


        [Authorize(Roles = "customer")]
        [HttpPost]
        [Route("api/mobile/like_dislike")]
        public HttpResponseMessage LikeDislikeVideo([FromBody]JObject data)
        {
            try
            {
                if (data["video_id"] == null || data["type"] == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotAcceptable, new { error = "Required parameters not supplied" });
                }

                long VideoId = DataHelper.longParse(data["video_id"]);
                int type = DataHelper.intParse(data["type"]);
                bool live_streaming = false;
                if (data["live_streaming"] != null)
                    live_streaming = DataHelper.boolParse(data["live_streaming"]);


                var identity = (ClaimsIdentity)User.Identity;
                var roles = identity.Claims.
                    Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value);


                int LoginId = DataHelper.intParse(identity.Claims.Where(r => r.Type == "loginid").FirstOrDefault().Value);

                var result = new Common().LikeVideo(VideoId, LoginId, type, false, live_streaming);

                if (result == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = "Exception" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, (object)result);
                }
            }
            catch (Exception ae)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = ae.Message });
            }
        }


        [Authorize(Roles = "customer")]
        [HttpPost]
        [Route("api/mobile/subscribe")]
        public HttpResponseMessage SubscribeChannel([FromBody]JObject data)
        {
            try
            {
                if (data["channel_id"] == null || data["subscribe"] == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotAcceptable, new { error = "Required parameters not supplied" });
                }

                int ChannelId = DataHelper.intParse(data["channel_id"]);
                int Subscribe = DataHelper.intParse(data["subscribe"]);

                var identity = (ClaimsIdentity)User.Identity;
                var roles = identity.Claims.
                    Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value);


                int LoginId = DataHelper.intParse(identity.Claims.Where(r => r.Type == "loginid").FirstOrDefault().Value);

                var result = new Common().SubscriberRequest(ChannelId, LoginId, Subscribe);

                if (result == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = "Exception" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { TotalSubscribers = (object)result });
                }
            }
            catch (Exception ae)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = ae.Message });
            }
        }


        [AllowAnonymous]
        [HttpPost]
        [Route("api/mobile/forgot-password")]
        public HttpResponseMessage ForgotPassword([FromBody]JObject data)
        {
            try
            {
                if (data["email"] == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotAcceptable, new { ErrorCode = "003", Message = "Required parameters not supplied" });
                }

                string email = data["email"].ToString();

                var result = new Accounts().ForgotPasswordToken(email);

                if (result == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { ErrorCode = "999", Message = "Exception" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, (object)result );
                }
            }
            catch (Exception ae)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { ErrorCode = "999", Message = ae.Message });
            }
        }


        [AllowAnonymous]
        [HttpPost]
        [Route("api/mobile/forgot-password-verify-token")]
        public HttpResponseMessage ForgotPasswordVerifyToken([FromBody]JObject data)
        {
            try
            {
                if (data["email"] == null || data["token"] == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotAcceptable, new { ErrorCode = "003", Message = "Required parameters not supplied" });
                }

                string email = data["email"].ToString();
                string token = data["token"].ToString();

                var result = new Accounts().ForgotPasswordTokenVerify(email, token);

                if (result == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { ErrorCode = "999", Message = "Exception" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, (object)result);
                }
            }
            catch (Exception ae)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { ErrorCode = "999", Message = ae.Message });
            }
        }


        [AllowAnonymous]
        [HttpPost]
        [Route("api/mobile/forgot-password-change-password")]
        public HttpResponseMessage ForgotPasswordChangePassword([FromBody]JObject data)
        {
            try
            {
                if (data["email"] == null || data["token"] == null || data["password"] == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotAcceptable, new { ErrorCode = "003", Message = "Required parameters not supplied" });
                }

                string email = data["email"].ToString();
                string token = data["token"].ToString();
                string password = data["password"].ToString();

                var result = new Accounts().ForgotPasswordChangePassword(email, token, password);

                if (result == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { ErrorCode = "999", Message = "Exception" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, (object)result);
                }
            }
            catch (Exception ae)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { ErrorCode = "999", Message = ae.Message });
            }
        }

        [Authorize(Roles = "customer")]
        [HttpPost]
        [Route("api/mobile/get-subscription-status")]
        public HttpResponseMessage GetSubscriptionStatus([FromBody]JObject data)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var roles = identity.Claims.
                Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value);

            int LoginId = DataHelper.intParse(identity.Claims.Where(r => r.Type == "loginid").FirstOrDefault().Value);

            object result = new BusinessLogic.MobileApi().GetSubscriptionStatus(LoginId);

            if (result.ToString() == "False")
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = "unknown exception" });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
        }


        [Authorize(Roles = "customer")]
        [HttpPost]
        [Route("api/mobile/my-subscription")]
        public HttpResponseMessage MySubscription([FromBody]JObject data)
        {
            try
            {

                var identity = (ClaimsIdentity)User.Identity;
                var roles = identity.Claims.
                    Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value);

                int LoginId = DataHelper.intParse(identity.Claims.Where(r => r.Type == "loginid").FirstOrDefault().Value);

                var result = new BusinessLogic.MobileApi().GetMySubscription(LoginId);

                if (result == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = "Exception" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
            }
            catch (Exception ae)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = ae.Message });
            }
        }


        [Authorize(Roles = "customer")]
        [HttpPost]
        [Route("api/mobile/change-subscription-status")]
        public HttpResponseMessage ChangeSubscriptionStatus([FromBody]JObject data)
        {
            try
            {

                if (data["status"] == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotAcceptable, new { error = "Required parameters not supplied" });
                }

                int Status = DataHelper.intParse(data["status"]);

                var identity = (ClaimsIdentity)User.Identity;
                var roles = identity.Claims.
                    Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value);

                int LoginId = DataHelper.intParse(identity.Claims.Where(r => r.Type == "loginid").FirstOrDefault().Value);

                var result = new BusinessLogic.MobileApi().ChangeSubscriptionStatus(LoginId, Status);

                if (result == false)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = "exception" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { error = 200 });
                }
            }
            catch (Exception ae)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = ae.Message });
            }
        }


        [Authorize(Roles = "customer")]
        [HttpPost]
        [Route("api/mobile/update-views-counts")]
        public HttpResponseMessage UpdateViewsCounts([FromBody]JObject data)
        {
            try
            {

                if (data["video_id"] == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotAcceptable, new { error = "Required parameters not supplied" });
                }

                long VideoId = DataHelper.longParse(data["video_id"]);
                
                bool live_streaming = false;
                if (data["live_streaming"] != null)
                    live_streaming = DataHelper.boolParse(data["live_streaming"]);

                var identity = (ClaimsIdentity)User.Identity;
                var roles = identity.Claims.
                    Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value);

                int LoginId = DataHelper.intParse(identity.Claims.Where(r => r.Type == "loginid").FirstOrDefault().Value);

                var result = new BusinessLogic.MobileApi().UpdateViewsCounts(VideoId, live_streaming);

                if (result == false)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = "exception" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { error = 200 });
                }
            }
            catch (Exception ae)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = ae.Message });
            }
        }


        [Authorize(Roles = "customer")]
        [HttpPost]
        [Route("api/mobile/get-report-reasons")]
        public HttpResponseMessage GetReportReasons()
        {
            try
            {
                List<SmartEcommerce.Models.Common.Topic> data = new BusinessLogic.Common().GetTopicByStatus(1);

                var result = data.Where(r => r.Active == true).OrderBy(r => r.Title);

                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ae)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = ae.Message });
            }
        }

        [Authorize(Roles = "customer")]
        [HttpPost]
        [Route("api/mobile/add-claim-video")]
        public HttpResponseMessage AddClaimVideo([FromBody]JObject data)
        {
            try
            {
                if (data["video_id"] == null || data["description"] == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotAcceptable, new { error = "Required parameters not supplied" });
                }

                var identity = (ClaimsIdentity)User.Identity;
                var roles = identity.Claims.
                    Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value);


                int LoginId = DataHelper.intParse(identity.Claims.Where(r => r.Type == "loginid").FirstOrDefault().Value);
                long VideoId = DataHelper.longParse(data["video_id"]);
                string Description = DataHelper.stringParse(data["description"]);

                var result = new BusinessLogic.MobileApi().AddClaimVideo(LoginId, VideoId, Description);

                if (result == false)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = "exception" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
            }
            catch (Exception ae)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = ae.Message });
            }
        }

        [Authorize(Roles = "customer")]
        [HttpPost]
        [Route("api/mobile/generate-customer-id")]
        public HttpResponseMessage GetStripeCustomerId([FromBody]JObject data)
        {
            try
            {
                if (data["amount"] == null || data["currency"] == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotAcceptable, new { error = "Required parameters not supplied" });
                }

                var identity = (ClaimsIdentity)User.Identity;
                var roles = identity.Claims.
                    Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value);


                int LoginId = DataHelper.intParse(identity.Claims.Where(r => r.Type == "loginid").FirstOrDefault().Value);
                decimal Amount = DataHelper.decimalParse(data["amount"]);
                string Currency = DataHelper.stringParse(data["currency"]);

                var result = new BusinessLogic.MobileApi().GetStripeCustomerId(LoginId, Amount, Currency);

                if (result == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = "exception" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
            }
            catch (Exception ae)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = ae.Message });
            }
        }

        [Authorize(Roles = "customer")]
        [HttpPost]
        [Route("api/mobile/save-payment-response")]
        public HttpResponseMessage SavePaymentResponse([FromBody]JObject data)
        {
            try
            {
                dynamic item = JsonConvert.DeserializeObject<dynamic>(data.ToString());

                if (item == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotAcceptable, new { error = "Required parameters not supplied" });
                }

                var identity = (ClaimsIdentity)User.Identity;
                var roles = identity.Claims.
                    Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value);


                int LoginId = DataHelper.intParse(identity.Claims.Where(r => r.Type == "loginid").FirstOrDefault().Value);


                string result = new BusinessLogic.MobileApi().SavePaymentResponse(LoginId, item);

                if (result == "000")
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { response = "success" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { response = "failed" });
                }
            }
            catch (Exception ae)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { error = ae.Message });
            }
        }

    }
}
