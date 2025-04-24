using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BusinessLogic
{
    public static class clsCookie
    {
        public static string cookieName
        {
            get
            {
                return UserInfoCookie.scookiename;
            }
        }

        public static bool Exists
        {
            get
            {
                UserInfoCookie cookie = new UserInfoCookie();
                return cookie.Exists;
            }
        }

        public static int AccountId
        {
            get
            {
                UserInfoCookie cookie = new UserInfoCookie();
                return cookie.AccountId;
            }
        }

        public static int LoginId
        {
            get
            {
                UserInfoCookie cookie = new UserInfoCookie();
                return cookie.LoginId;
            }
        }

        public static int PartnerCategoryId
        {
            get
            {
                UserInfoCookie cookie = new UserInfoCookie();
                return cookie.PartnerCategoryId;
            }
        }

        public static string UserId
        {
            get
            {
                UserInfoCookie cookie = new UserInfoCookie();
                return cookie.UserId;
            }
        }

        public static string UserName
        {
            get
            {
                UserInfoCookie cookie = new UserInfoCookie();
                return cookie.UserName;
            }
        }

        public static int LoginType
        {
            get
            {
                UserInfoCookie cookie = new UserInfoCookie();
                return cookie.LoginType;
            }
        }

        public static string ProfileImageURL
        {
            get
            {
                UserInfoCookie cookie = new UserInfoCookie();
                return cookie.ProfileImageURL;
            }
        }

        public static DateTime LoginTime
        {
            get
            {
                UserInfoCookie cookie = new UserInfoCookie();
                return cookie.LoginTime;
            }
        }

        public static void Logout()
        {
            //clsCookie.Logout();
            HttpCookie userCookie = new HttpCookie(BusinessLogic.UserInfoCookie.scookiename);
            HttpContext.Current.Response.Cookies[BusinessLogic.UserInfoCookie.scookiename].Expires = DateTime.Now.AddDays(-1);
            HttpContext.Current.Response.Cookies.Remove(BusinessLogic.UserInfoCookie.sStoreId);
            HttpContext.Current.Response.Cookies.Remove(BusinessLogic.UserInfoCookie.sLoginId);
            HttpContext.Current.Response.Cookies.Remove(BusinessLogic.UserInfoCookie.sUserId);
            HttpContext.Current.Response.Cookies.Remove(BusinessLogic.UserInfoCookie.sUserName);
            HttpContext.Current.Response.Cookies.Remove(BusinessLogic.UserInfoCookie.sloginType);
        }

        public static List<long> RecentlyViewedProducts
        {
            get
            {
                UserInfoCookie info = new UserInfoCookie();
                string[] val = info.RecentlyViewedProducts.Split(',');

                List<long> list = new List<long>();
                foreach (string item in val)
                {
                    if (DataHelper.longParse(item) > 0)
                        list.Add(DataHelper.longParse(item));
                }

                return list;
            }
        }
    }

    public static class clsWebCookie
    {
        public static string cookieName
        {
            get
            {
                return UserWebInfoCookie.scookiename;
            }
        }

        public static bool Exists
        {
            get
            {
                UserWebInfoCookie cookie = new UserWebInfoCookie();
                return cookie.Exists;
            }
        }

        public static int AccountId
        {
            get
            {
                UserWebInfoCookie cookie = new UserWebInfoCookie();
                return cookie.AccountId;
            }
        }

        public static int LoginId
        {
            get
            {
                UserWebInfoCookie cookie = new UserWebInfoCookie();
                return cookie.LoginId;
            }
        }

        public static string UserId
        {
            get
            {
                UserWebInfoCookie cookie = new UserWebInfoCookie();
                return cookie.UserId;
            }
        }

        public static string UserName
        {
            get
            {
                UserWebInfoCookie cookie = new UserWebInfoCookie();
                return cookie.UserName;
            }
        }

        public static int LoginType
        {
            get
            {
                UserWebInfoCookie cookie = new UserWebInfoCookie();
                return cookie.LoginType;
            }
        }

        public static string ProfileImageURL
        {
            get
            {
                UserWebInfoCookie cookie = new UserWebInfoCookie();
                return cookie.ProfileImageURL;
            }
        }

        public static DateTime LoginTime
        {
            get
            {
                UserWebInfoCookie cookie = new UserWebInfoCookie();
                return cookie.LoginTime;
            }
        }

        public static string SubscriptionStatus
        {
            get
            {
                UserWebInfoCookie cookie = new UserWebInfoCookie();
                return cookie.SubscriptionStatus;
            }
        }

        public static string SubscriptionType
        {
            get
            {
                UserWebInfoCookie cookie = new UserWebInfoCookie();
                return cookie.SubscriptionType;
            }
        }

        public static void Logout()
        {
            //clsCookie.Logout();
            HttpCookie userCookie = new HttpCookie(BusinessLogic.UserWebInfoCookie.scookiename);
            HttpContext.Current.Response.Cookies[BusinessLogic.UserWebInfoCookie.scookiename].Expires = DateTime.Now.AddDays(-1);
            HttpContext.Current.Response.Cookies.Remove(BusinessLogic.UserWebInfoCookie.sStoreId);
            HttpContext.Current.Response.Cookies.Remove(BusinessLogic.UserWebInfoCookie.sLoginId);
            HttpContext.Current.Response.Cookies.Remove(BusinessLogic.UserWebInfoCookie.sUserId);
            HttpContext.Current.Response.Cookies.Remove(BusinessLogic.UserWebInfoCookie.sUserName);
            HttpContext.Current.Response.Cookies.Remove(BusinessLogic.UserWebInfoCookie.sloginType);
        }

        public static List<long> RecentlyViewedProducts
        {
            get
            {
                UserWebInfoCookie info = new UserWebInfoCookie();
                string[] val = info.RecentlyViewedProducts.Split(',');

                List<long> list = new List<long>();
                foreach (string item in val)
                {
                    if (DataHelper.longParse(item) > 0)
                        list.Add(DataHelper.longParse(item));
                }

                return list;
            }
        }
    }
}
