using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BusinessLogic
{
    public class UserInfoCookie
    {
        public const string scookiename = "sbonl__inf";
        public const string sStoreId = "daid";
        public const string sLoginId = "dlid";
        public const string sUserId = "duid";
        public const string sUserName = "dun";
        public const string sLoginTime = "dlt";
        public const string sloginType = "slctype";
        public const string simageurl = "simageurl";
        public const string sItemId = "tmid";
        public const string sPartnerCategoryId = "dpcid";

        public bool Exists
        {
            get
            {
                try
                {
                    HttpCookie userCookie = HttpContext.Current.Request.Cookies[scookiename];
                    if (userCookie != null)
                    {
                        //if (LoginId == 0)
                        //{
                        //    int test = LoginId;
                        //}

                        if (userCookie.HasKeys)
                            return true;
                        else
                            return false;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ae)
                {
                    Logs.WriteError("UserInfoCookie.cs", "Exists", "General", ae.Message);
                    return false;
                }
            }
        }

        public bool Exists2
        {
            get
            {
                try
                {
                    HttpCookie userCookie = HttpContext.Current.Request.Cookies[scookiename];
                    if (userCookie != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ae)
                {
                    Logs.WriteError("UserInfoCookie.cs", "Exists", "General", ae.Message);
                    return false;
                }
            }
        }

        public int AccountId
        {
            get
            {
                try
                {
                    if (Exists)
                    {
                        HttpCookie userCookie = HttpContext.Current.Request.Cookies[scookiename];
                        return DataHelper.intParse(SBEncryption.Decrypt(userCookie[sStoreId]));
                    }
                    else
                    {
                        return 0;
                    }
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        public int LoginId
        {
            get
            {
                try
                {
                    if (Exists)
                    {
                        HttpCookie userCookie = HttpContext.Current.Request.Cookies[scookiename];
                        return DataHelper.intParse(SBEncryption.Decrypt(userCookie[sLoginId]));
                    }
                    else
                    {
                        return 0;
                    }
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        public int LoginType
        {
            get
            {
                try
                {
                    if (Exists)
                    {
                        HttpCookie userCookie = HttpContext.Current.Request.Cookies[scookiename];
                        return DataHelper.intParse(SBEncryption.Decrypt(userCookie[sloginType]));
                    }
                    else
                    {
                        return 0;
                    }
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        public string UserId
        {
            get
            {
                try
                {
                    if (Exists)
                    {
                        HttpCookie userCookie = HttpContext.Current.Request.Cookies[scookiename];
                        return SBEncryption.Decrypt(userCookie[sUserId]);
                    }
                    else
                    {
                        return "";
                    }

                }
                catch (Exception)
                {
                    return "";
                }
            }
        }

        public string UserName
        {
            get
            {
                try
                {
                    if (Exists)
                    {
                        HttpCookie userCookie = HttpContext.Current.Request.Cookies[scookiename];
                        return SBEncryption.Decrypt(userCookie[sUserName]);
                    }
                    else
                    {
                        return "";
                    }

                }
                catch (Exception)
                {
                    return "";
                }
            }
        }

        public string ProfileImageURL
        {
            get
            {
                try
                {
                    if (Exists)
                    {
                        HttpCookie userCookie = HttpContext.Current.Request.Cookies[scookiename];
                        return SBEncryption.Decrypt(userCookie[simageurl]);
                    }
                    else
                    {
                        return "";
                    }

                }
                catch (Exception)
                {
                    return "";
                }
            }
        }

        public DateTime LoginTime
        {
            get
            {
                try
                {
                    if (Exists)
                    {
                        HttpCookie userCookie = HttpContext.Current.Request.Cookies[scookiename];
                        return DataHelper.dateParse(SBEncryption.Decrypt(userCookie[sLoginTime]));
                    }
                    else
                    {
                        return DateTime.Now;
                    }

                }
                catch (Exception)
                {
                    return DateTime.Now;
                }
            }
        }

        public string RecentlyViewedProducts
        {
            get
            {
                try
                {
                    HttpCookie userCookie = HttpContext.Current.Request.Cookies[scookiename];
                    if (userCookie != null)
                    {
                        return SBEncryption.Decrypt(userCookie[sItemId]);
                    }
                    else
                    {
                        return "";
                    }
                }
                catch (Exception ae)
                {
                    return "";
                }
            }
        }

        public int PartnerCategoryId
        {
            get
            {
                try
                {
                    if (Exists)
                    {
                        HttpCookie userCookie = HttpContext.Current.Request.Cookies[scookiename];
                        return DataHelper.intParse(SBEncryption.Decrypt(userCookie[sPartnerCategoryId]));
                    }
                    else
                    {
                        return 0;
                    }
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }
    }

    public class UserWebInfoCookie
    {
        public const string scookiename = "sbonl__web";
        public const string sStoreId = "wsid";
        public const string sLoginId = "wlid";
        public const string sUserId = "wuid";
        public const string sUserName = "wun";
        public const string sLoginTime = "wlt";
        public const string sloginType = "wlctype";
        public const string simageurl = "wimageurl";
        public const string sItemId = "wmid";
        public const string sSubscriptionStatus = "wsst";
        public const string sSubscriptionType = "wstyp";

        public bool Exists
        {
            get
            {
                try
                {
                    HttpCookie userCookie = HttpContext.Current.Request.Cookies[scookiename];
                    if (userCookie != null)
                    {
                        //if (LoginId == 0)
                        //{
                        //    int test = LoginId;
                        //}

                        if (userCookie.HasKeys)
                            return true;
                        else
                            return false;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ae)
                {
                    Logs.WriteError("UserInfoCookie.cs", "Exists", "General", ae.Message);
                    return false;
                }
            }
        }

        public bool Exists2
        {
            get
            {
                try
                {
                    HttpCookie userCookie = HttpContext.Current.Request.Cookies[scookiename];
                    if (userCookie != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ae)
                {
                    Logs.WriteError("UserInfoCookie.cs", "Exists", "General", ae.Message);
                    return false;
                }
            }
        }

        public int AccountId
        {
            get
            {
                try
                {
                    if (Exists)
                    {
                        HttpCookie userCookie = HttpContext.Current.Request.Cookies[scookiename];
                        return DataHelper.intParse(SBEncryption.Decrypt(userCookie[sStoreId]));
                    }
                    else
                    {
                        return 0;
                    }
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        public int LoginId
        {
            get
            {
                try
                {
                    if (Exists)
                    {
                        HttpCookie userCookie = HttpContext.Current.Request.Cookies[scookiename];
                        return DataHelper.intParse(SBEncryption.Decrypt(userCookie[sLoginId]));
                    }
                    else
                    {
                        return 0;
                    }
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        public int LoginType
        {
            get
            {
                try
                {
                    if (Exists)
                    {
                        HttpCookie userCookie = HttpContext.Current.Request.Cookies[scookiename];
                        return DataHelper.intParse(SBEncryption.Decrypt(userCookie[sloginType]));
                    }
                    else
                    {
                        return 0;
                    }
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        public string UserId
        {
            get
            {
                try
                {
                    if (Exists)
                    {
                        HttpCookie userCookie = HttpContext.Current.Request.Cookies[scookiename];
                        return SBEncryption.Decrypt(userCookie[sUserId]);
                    }
                    else
                    {
                        return "";
                    }

                }
                catch (Exception)
                {
                    return "";
                }
            }
        }

        public string UserName
        {
            get
            {
                try
                {
                    if (Exists)
                    {
                        HttpCookie userCookie = HttpContext.Current.Request.Cookies[scookiename];
                        return SBEncryption.Decrypt(userCookie[sUserName]);
                    }
                    else
                    {
                        return "";
                    }

                }
                catch (Exception)
                {
                    return "";
                }
            }
        }

        public string ProfileImageURL
        {
            get
            {
                try
                {
                    if (Exists)
                    {
                        HttpCookie userCookie = HttpContext.Current.Request.Cookies[scookiename];
                        return SBEncryption.Decrypt(userCookie[simageurl]);
                    }
                    else
                    {
                        return "";
                    }

                }
                catch (Exception)
                {
                    return "";
                }
            }
        }

        public DateTime LoginTime
        {
            get
            {
                try
                {
                    if (Exists)
                    {
                        HttpCookie userCookie = HttpContext.Current.Request.Cookies[scookiename];
                        return DataHelper.dateParse(SBEncryption.Decrypt(userCookie[sLoginTime]));
                    }
                    else
                    {
                        return DateTime.Now;
                    }

                }
                catch (Exception)
                {
                    return DateTime.Now;
                }
            }
        }

        public string SubscriptionStatus
        {
            get
            {
                try
                {
                    if (Exists)
                    {
                        HttpCookie userCookie = HttpContext.Current.Request.Cookies[scookiename];
                        return SBEncryption.Decrypt(userCookie[sSubscriptionStatus]);
                    }
                    else
                    {
                        return "InActive";
                    }

                }
                catch (Exception ae)
                {
                    return "InActive" ;
                }
            }
        }

        public string SubscriptionType
        {
            get
            {
                try
                {
                    if (Exists)
                    {
                        HttpCookie userCookie = HttpContext.Current.Request.Cookies[scookiename];
                        return SBEncryption.Decrypt(userCookie[sSubscriptionType]);
                    }
                    else
                    {
                        return "";
                    }

                }
                catch (Exception ae)
                {
                    return "";
                }
            }
        }

        public string RecentlyViewedProducts
        {
            get
            {
                try
                {
                    HttpCookie userCookie = HttpContext.Current.Request.Cookies[scookiename];
                    if (userCookie != null)
                    {
                        return SBEncryption.Decrypt(userCookie[sItemId]);
                    }
                    else
                    {
                        return "";
                    }
                }
                catch (Exception ae)
                {
                    return "";
                }
            }
        }
    }
}
