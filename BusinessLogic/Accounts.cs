using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;

namespace BusinessLogic
{
    public class Accounts
    {
        public SmartEcommerce.Models.Admin.Logins ValidateUser(string UserId, string Password)
        {
            try
            {
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@UserId", Value = UserId },
                    new SqlParameter { ParameterName = "@Password", Value = SBEncryption.getMD5Password(UserId, Password) }
                };
                ErrorResponse response = new ErrorResponse();

                DataTable dt = DatabaseObject.FetchTableFromSP("ValidateLogin", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return null;
                }
                else
                {
                    if (DataHelper.HasRows(dt))
                    {
                        DataRow row = dt.Rows[0];
                        if (DataHelper.intParse(row["Success"]) == 1)
                        {
                            SmartEcommerce.Models.Admin.Logins account = new SmartEcommerce.Models.Admin.Logins()
                            {
                                LoginId = DataHelper.intParse(row["LoginId"]),
                                UserId = UserId,
                                UserName = row["UserName"].ToString(),
                                LoginType = DataHelper.intParse(row["LoginType"]),
                                PartnerCategoryId = DataHelper.intParse(row["PartnerCategory"])
                            };

                            return account;
                        }
                    }
                }

                return null;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public SmartEcommerce.Models.Admin.Logins ValidateCustomer(string UserId, string Password)
        {
            try
            {
                string enc_password = SBEncryption.getMD5Password(UserId, Password);
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@UserId", Value = UserId },
                    new SqlParameter { ParameterName = "@Password", Value = enc_password }
                };

                ErrorResponse response = new ErrorResponse();
                DataTable dt = DatabaseObject.FetchTableFromSP("ValidateCustomerLogin", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return null;
                }
                else
                {
                    if (DataHelper.HasRows(dt))
                    {
                        DataRow row = dt.Rows[0];
                        if (DataHelper.intParse(row["Success"]) == 1)
                        {
                            SmartEcommerce.Models.Admin.Logins account = new SmartEcommerce.Models.Admin.Logins()
                            {
                                LoginId = DataHelper.intParse(row["LoginId"]),
                                UserId = UserId,
                                UserName = row["UserName"].ToString(),
                                LoginType = DataHelper.intParse(row["LoginType"]),
                                ProfilePicture = row["ProfilePicture"].ToString(),
                                SubscriptionStatus = row["SubscriptionStatus"].ToString(),
                                SubscriptionType = row["SubscriptionType"].ToString()
                            };

                            return account;
                        }
                    }
                }

                return null;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public SmartEcommerce.Models.Admin.Other SignupUser(string FullName, string Email, string PhoneNo, int CountryId, int StateId, int CityId, string Password,
            int PartnerId = 0, bool IsWeb = false)
        {
            try
            {

                string EncPass = SBEncryption.getMD5Password(Email, Password);
                string ProfilePicture = DataHelper.GenerateProfilePicture(FullName);

                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@FullName", Value = FullName },
                    new SqlParameter { ParameterName = "@EmailAddress", Value = Email },
                    new SqlParameter { ParameterName = "@PhoneNo", Value = PhoneNo },
                    new SqlParameter { ParameterName = "@CountryId", Value = CountryId },
                    new SqlParameter { ParameterName = "@StateId", Value = StateId },
                    new SqlParameter { ParameterName = "@CityId", Value = CityId },
                    new SqlParameter { ParameterName = "@Password", Value = EncPass },
                    new SqlParameter { ParameterName = "@ProfilePicture", Value = ProfilePicture },
                    new SqlParameter { ParameterName = "@PartnerId", Value = PartnerId }
                };
                ErrorResponse response = new BusinessLogic.ErrorResponse();
                DataTable result = DatabaseObject.FetchTableFromSP("Web_SignupCustomer", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return new SmartEcommerce.Models.Admin.Other { ErrorCode = "999", ErrorDescription = response.ErrorList[0].Message };
                }

                DataRow row = result.Rows[0];

                Logs.WriteError("Custom", string.Join(",", row.ItemArray));

                if (row["ErrorCode"].ToString() == "000")
                {

                    StringBuilder sbHtml = new StringBuilder();
                    var path = System.Web.HttpContext.Current.Server.MapPath("~/content/emails/signup.html");
                    sbHtml.AppendLine(System.IO.File.ReadAllText(path));

                    sbHtml = sbHtml.Replace("{NAME}", FullName);
                    sbHtml = sbHtml.Replace("{EMAIL}", Email);
                    sbHtml = sbHtml.Replace("{PASSWORD}", Password);


                    if (IsWeb)
                    {
                        SBSession.CreateSession("SessionCustomerId", DataHelper.longParse(row["CustomerId"]));
                    }

                    bool IsSend = Emails.SendMail(Email, "admin@netfive.tv", "NetFive - Welcome to NetFive!", sbHtml.ToString(), true);
                    return new SmartEcommerce.Models.Admin.Other
                    {
                        ErrorCode = row["ErrorCode"].ToString(),
                        ErrorDescription = "Success",
                        Key1 = DataHelper.longParse(row["CustomerId"]),
                        Key2 = row["SubscriptionType"].ToString(),
                        SubscriptionTrial = DataHelper.boolParse(row["SubscriptionTrial"]),
                        SubscriptionEnd = DataHelper.dateParse(row["SubscriptionEnd"]),
                        ProfilePicture = ProfilePicture
                    };
                }
                else
                {
                    return new SmartEcommerce.Models.Admin.Other { ErrorCode = row["ErrorCode"].ToString() };
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);

                return new SmartEcommerce.Models.Admin.Other { ErrorCode = "999", ErrorDescription = ae.Message };
            }
        }

        public SmartEcommerce.Models.Admin.Other ForgetPassword(string Email)
        {
            try
            {
                string Password = PasswordGenerator.Generate();
                string EncPass = SBEncryption.getMD5Password(Email, Password);

                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@EmailAddress", Value = Email },
                    new SqlParameter { ParameterName = "@Password", Value = EncPass }
                };
                ErrorResponse response = new BusinessLogic.ErrorResponse();
                DataTable result = DatabaseObject.FetchTableFromSP("Web_ForgetPassword", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return new SmartEcommerce.Models.Admin.Other { ErrorCode = "999", ErrorDescription = response.ErrorList[0].Message };
                }

                DataRow row = result.Rows[0];
                if (row["ErrorCode"].ToString() == "000")
                {
                    StringBuilder sbHtml = new StringBuilder();
                    var path = System.Web.HttpContext.Current.Server.MapPath("~/content/emails/forget_password.html");
                    sbHtml.AppendLine(System.IO.File.ReadAllText(path));
                    sbHtml = sbHtml.Replace("{EMAIL}", Email);
                    sbHtml = sbHtml.Replace("{PASSWORD}", Password);

                    Emails.SendMail(Email, "Net Five - Forget Password", sbHtml.ToString(), true);
                    return new SmartEcommerce.Models.Admin.Other { ErrorCode = "000", ErrorDescription = "Success" };
                }
                else
                {
                    return new SmartEcommerce.Models.Admin.Other { ErrorCode = row["ErrorCode"].ToString() };
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);

                return new SmartEcommerce.Models.Admin.Other { ErrorCode = "999", ErrorDescription = ae.Message };
            }
        }

        public SmartEcommerce.Models.Admin.Other ChangePassword(int LoginId, string UserId, string CurrentPassword, string NewPassword)
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
                
                DataTable result = DatabaseObject.FetchTableFromSP("ChangePassword", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return new SmartEcommerce.Models.Admin.Other { ErrorCode = "999", ErrorDescription = response.ErrorList[0].Message };
                }

                DataRow row = result.Rows[0];
                return new SmartEcommerce.Models.Admin.Other { ErrorCode = row["ErrorCode"].ToString() };
                
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);

                return new SmartEcommerce.Models.Admin.Other { ErrorCode = "999", ErrorDescription = ae.Message };
            }
        }

        public SmartEcommerce.Models.Admin.Other ChangeAdminPassword(int LoginId, string UserId, string CurrentPassword, string NewPassword)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId },
                    new SqlParameter { ParameterName = "@OldPassword", Value = SBEncryption.getMD5Password(UserId, CurrentPassword) },
                    new SqlParameter { ParameterName = "@NewPassword", Value = SBEncryption.getMD5Password(UserId, NewPassword) }
                };

                DataTable result = DatabaseObject.FetchTableFromSP("admin_changepassword", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return new SmartEcommerce.Models.Admin.Other { ErrorCode = "999", ErrorDescription = response.ErrorList[0].Message };
                }

                DataRow row = result.Rows[0];

                string ErrorCode = row["ErrorCode"].ToString();

                if (ErrorCode == "000")
                {
                    

                    if (BusinessLogic.clsSession.LoginType != 1)
                    {
                        StringBuilder sbHtml = new StringBuilder();
                        var path = System.Web.HttpContext.Current.Server.MapPath("~/content/emails/change-password.html");
                        sbHtml.AppendLine(System.IO.File.ReadAllText(path));

                        sbHtml = sbHtml.Replace("USER_ID", clsSession.UserId);

                        Emails.SendMail(clsSession.UserId, "Net Five password changed.", sbHtml.ToString(), true);
                    }
                }

                return new SmartEcommerce.Models.Admin.Other { ErrorCode = row["ErrorCode"].ToString() };

            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);

                return new SmartEcommerce.Models.Admin.Other { ErrorCode = "999", ErrorDescription = ae.Message };
            }
        }

        public SmartEcommerce.Models.Admin.Other VideoAddToList(int LoginId, int VideoId, int IsList)
        {
            try
            {

                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId },
                    new SqlParameter { ParameterName = "@VideoId", Value = VideoId },
                    new SqlParameter { ParameterName = "@IsList", Value = IsList }
                };
                ErrorResponse response = new BusinessLogic.ErrorResponse();
                DataTable result = DatabaseObject.FetchTableFromSP("VideoAddToList", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return new SmartEcommerce.Models.Admin.Other { ErrorCode = "999", ErrorDescription = response.ErrorList[0].Message };
                }

                DataRow row = result.Rows[0];
                if (row["ErrorCode"].ToString() == "000")
                {
                    return new SmartEcommerce.Models.Admin.Other { ErrorCode = "000", ErrorDescription = "Success" };
                }
                else
                {
                    return new SmartEcommerce.Models.Admin.Other { ErrorCode = row["ErrorCode"].ToString() };
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);

                return new SmartEcommerce.Models.Admin.Other { ErrorCode = "999", ErrorDescription = ae.Message };
            }
        }

        public SmartEcommerce.Models.Admin.Other VideoAddDispute(int LoginId, int VideoId, string VideoURL, string Description, int TopicId, string TopicName, int Type)
        {
            try
            {

                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@LoginId", Value = LoginId },
                    new SqlParameter { ParameterName = "@VideoId", Value = VideoId },
                    new SqlParameter { ParameterName = "@VideoURL", Value = VideoURL },
                    new SqlParameter { ParameterName = "@Description", Value = Description },
                    new SqlParameter { ParameterName = "@TopicId", Value = TopicId },
                    new SqlParameter { ParameterName = "@Type", Value = Type }
                };
                ErrorResponse response = new BusinessLogic.ErrorResponse();
                DataTable result = DatabaseObject.FetchTableFromSP("VideoAddDispute", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return new SmartEcommerce.Models.Admin.Other { ErrorCode = "999", ErrorDescription = response.ErrorList[0].Message };
                }

                string TypeName = Type == 1 ? "Claim Video" : "Report Video";

                DataRow row = result.Rows[0];
                if (row["ErrorCode"].ToString() == "000")
                {
                    string admin_email = ConfigurationManager.AppSettings["admin_email"];

                    StringBuilder sbHtml = new StringBuilder();
                    var path = System.Web.HttpContext.Current.Server.MapPath("~/content/emails/dispute_video.html");
                    sbHtml.AppendLine(System.IO.File.ReadAllText(path));

                    if (Type == 1)
                    {
                        sbHtml = sbHtml.Replace("{TYPE}", TypeName);
                        sbHtml = sbHtml.Replace("[ISDISPLAYTOPIC]", "display:none;");
                    }
                    else
                    {
                        sbHtml = sbHtml.Replace("{TYPE}", TypeName);
                        sbHtml = sbHtml.Replace("{TOPIC}", TopicName);
                    }

                    sbHtml = sbHtml.Replace("{CUSTOMERNAME}", clsWebSession.UserName);
                    sbHtml = sbHtml.Replace("{VIDEOURL}", VideoURL);
                    sbHtml = sbHtml.Replace("{DESCRIPTION}", Description);

                    Emails.SendMail(admin_email, clsWebSession.UserId, "Net Five - " + TypeName, sbHtml.ToString(), true);

                    return new SmartEcommerce.Models.Admin.Other { ErrorCode = "000", ErrorDescription = "Success" };
                }
                else
                {
                    return new SmartEcommerce.Models.Admin.Other { ErrorCode = row["ErrorCode"].ToString() };
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);

                return new SmartEcommerce.Models.Admin.Other { ErrorCode = "999", ErrorDescription = ae.Message };
            }
        }

        public SmartEcommerce.Models.Admin.Other ContactUs(string Name, string Email, string PhoneNo, string Subject, string Message)
        {
            try
            {

                StringBuilder sbHtml = new StringBuilder();
                var path = System.Web.HttpContext.Current.Server.MapPath("~/content/emails/contactus.html");
                sbHtml.AppendLine(System.IO.File.ReadAllText(path));

                string admin_email = ConfigurationManager.AppSettings["admin_email"];

                sbHtml = sbHtml.Replace("{NAME}", Name);
                sbHtml = sbHtml.Replace("{EMAIL}", Email);
                sbHtml = sbHtml.Replace("{SUBJECT}", Subject);
                sbHtml = sbHtml.Replace("{PHONENO}", PhoneNo);
                sbHtml = sbHtml.Replace("{MESSAGE}", Message);

                bool IsSend = Emails.SendMail(admin_email, "Net Five - New Message Received", sbHtml.ToString(), true);

                if (IsSend)
                {
                    return new SmartEcommerce.Models.Admin.Other { ErrorCode = "000", ErrorDescription = "Success" };
                }
                else
                {
                    return new SmartEcommerce.Models.Admin.Other { ErrorCode = "001" };
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);

                return new SmartEcommerce.Models.Admin.Other { ErrorCode = "999", ErrorDescription = ae.Message };
            }
        }

        public dynamic ForgotPasswordToken(string email)
        {
            try
            {
                Random generator = new Random();
                String token = generator.Next(0, 999999).ToString("D6");

                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@email", Value = email },
                    new SqlParameter { ParameterName = "@token", Value = token }
                };

                DataTable dataTable = DatabaseObject.FetchTableFromSP("GenerateForgotPasswordToken", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return null;
                }
                else
                {
                    DataRow dataRow = dataTable.Rows[0];

                    if (dataRow["ErrorCode"].ToString() == "000")
                    {
                        StringBuilder sbHtml = new StringBuilder();
                        var path = System.Web.HttpContext.Current.Server.MapPath("~/content/emails/forgot-token.html");
                        sbHtml.AppendLine(System.IO.File.ReadAllText(path));

                        sbHtml = sbHtml.Replace("{OTP}", token);

                        bool IsSend = Emails.SendMail(email, "NetFive - Forgot password request!", sbHtml.ToString(), true);
                        

                        return new { ErrorCode = "000", Message = "An email sent to your email" };
                    }
                    else
                    {
                        switch (dataRow["ErrorCode"].ToString())
                        {
                            case "001":
                                return new { ErrorCode = "001", Message = "Account not found" };

                            case "002":
                                return new { ErrorCode = "002", Message = "Your account is deactivated" };
                        }
                    }

                    return null;
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;   
            }
        }

        public dynamic ForgotPasswordTokenVerify(string email, string token)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@email", Value = email },
                    new SqlParameter { ParameterName = "@token", Value = token }
                };

                DataTable dataTable = DatabaseObject.FetchTableFromSP("ForgotPasswordValidateToken", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return null;
                }
                else
                {
                    return new { ErrorCode = dataTable.Rows[0]["ErrorCode"].ToString(), Message = dataTable.Rows[0]["Message"].ToString() };
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }


        public dynamic ForgotPasswordChangePassword(string email, string token, string password)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@email", Value = email },
                    new SqlParameter { ParameterName = "@token", Value = token },
                    new SqlParameter { ParameterName = "@Password", Value = SBEncryption.getMD5Password(email, password) }
                };

                DataTable dataTable = DatabaseObject.FetchTableFromSP("ForgotPasswordChangePassword", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return null;
                }
                else
                {
                    return new { ErrorCode = dataTable.Rows[0]["ErrorCode"].ToString(), Message = dataTable.Rows[0]["Message"].ToString() };
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }

        public dynamic GetDashboardData()
        {
            try
            {
                ErrorResponse response = new BusinessLogic.ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@LoginId", Value = clsSession.LoginId }
                };

                DataTableCollection dataTableCollection = DatabaseObject.FetchFromSP("AdminDashboardData", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    return null;
                }
                else
                {
                    DataTable dt = dataTableCollection[0];

                    int totalUploadedVideos = DataHelper.intParse(dt.Rows[0]["TotalUploadedVideos"]);
                    int totalPendingVideos = DataHelper.intParse(dt.Rows[0]["TotalPendingVideos"]);
                    int totalCustomers = DataHelper.intParse(dt.Rows[0]["TotalCustomers"]);
                    int totalSubscribers = DataHelper.intParse(dt.Rows[0]["TotalSubscribers"]);

                    int totalActiveMontz = DataHelper.intParse(dt.Rows[0]["ActiveMontz"]);
                    int totalMontz = DataHelper.intParse(dt.Rows[0]["TotalMontz"]);

                    List<object> videos_list = new List<object>();

                    if (clsSession.LoginType == 2)
                    {
                        foreach (DataRow item in dataTableCollection[1].Rows)
                        {
                            string imageURL = "";
                            if (DataHelper.intParse(item["LibraryType"]) == 1)
                                imageURL = DataHelper.getVideoImageURL(item["ImageURL"].ToString());
                            else
                                imageURL = DataHelper.getLiveStreamingImageURL(item["ImageURL"].ToString());

                            var vObj = new
                            {
                                Id = DataHelper.longParse(item["Id"]),
                                Title = item["Title"].ToString(),
                                Image = imageURL,
                                Likes = DataHelper.FormatNumber(DataHelper.longParse(item["Likes"])),
                                DisLikes = DataHelper.FormatNumber(DataHelper.longParse(item["DisLikes"])),
                                Views = DataHelper.FormatNumber(DataHelper.longParse(item["Views"]))
                            };

                            videos_list.Add(vObj);
                        }
                    }

                    var obj = new
                    {
                        total_videos = DataHelper.FormatNumber(totalUploadedVideos),
                        total_pending_videos = DataHelper.FormatNumber(totalPendingVideos),
                        total_customers = DataHelper.FormatNumber(totalCustomers),
                        total_subscribers = DataHelper.FormatNumber(totalSubscribers),
                        total_active_montz = DataHelper.FormatNumber(totalActiveMontz),
                        total_montz = DataHelper.FormatNumber(totalMontz),
                        videos = videos_list
                    };

                    return obj;
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("Genreal", ae.Message);
                return null;
            }
        }
    }
}