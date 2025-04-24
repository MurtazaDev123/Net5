using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public static class clsSession
    {
        public static readonly bool useSession = false;

        public static bool HasLogin
        {
            get
            {
                if (useSession)
                    return SBSession.SessionExists(Sessions.LoginId);
                else
                    return (clsCookie.LoginId > 0);
            }
        }

        public static int AccountId
        {
            get
            {

                if (useSession)
                    return DataHelper.intParse(SBSession.GetSessionValue(Sessions.StoreId));
                else
                    return clsCookie.AccountId;
            }
        }

        public static int LoginId
        {
            get
            {

                if (useSession)
                    return DataHelper.intParse(SBSession.GetSessionValue(Sessions.LoginId));
                else
                    return clsCookie.LoginId;
            }
        }

        public static int PartnerCategoryId
        {
            get
            {

                if (useSession)
                    return DataHelper.intParse(SBSession.GetSessionValue(Sessions.PartnerCategoryId));
                else
                    return clsCookie.PartnerCategoryId;
            }
        }

        public static string UserId
        {
            get
            {
                if (useSession)
                    return DataHelper.stringParse(SBSession.GetSessionValue(Sessions.UserId));
                else
                    return clsCookie.UserId;
            }
        }

        public static string UserName
        {
            get
            {
                if (useSession)
                    return DataHelper.stringParse(SBSession.GetSessionValue(Sessions.UserName));
                else
                    return clsCookie.UserName;
            }
        }

        public static int LoginType
        {
            get
            {
                if (useSession)
                {
                    return DataHelper.intParse(SBSession.GetSessionValue(Sessions.LoginType));
                }
                else
                    return clsCookie.LoginType;
            }
        }

        public static string ProfileImageURL
        {
            get
            {
                if (useSession)
                {
                    return DataHelper.stringParse(SBSession.GetSessionValue(Sessions.ProfileImageURL));
                }
                else
                    return clsCookie.ProfileImageURL;
            }
        }

        public static DateTime LoginTime
        {
            get
            {
                if (useSession)
                {
                    return DataHelper.dateParse(SBSession.GetSessionValue(Sessions.LoginTime));
                }
                else
                    return clsCookie.LoginTime;
            }
        }
    }

    public static class clsWebSession
    {
        public static readonly bool useSession = false;
    
        public static bool HasLogin
        {
            get
            {

                if (useSession)
                    return SBSession.SessionExists(Sessions.LoginId);
                else
                    return (clsWebCookie.LoginId > 0);

            }
        }

        public static int LoginId
        {
            get
            {
                if (useSession)
                    return DataHelper.intParse(SBSession.GetSessionValue(Sessions.LoginId));
                else
                    return clsWebCookie.LoginId;

            }
        }

        public static int LoginType
        {
            get
            {
                if (useSession)
                {
                    return DataHelper.intParse(SBSession.GetSessionValue(Sessions.LoginType));
                }
                else
                    return clsWebCookie.LoginType;
            }
        }

        public static string UserId
        {
            get
            {
                if (useSession)
                    return DataHelper.stringParse(SBSession.GetSessionValue(Sessions.UserId));
                else
                    return clsWebCookie.UserId;
            }
        }

        public static string UserName
        {
            get
            {
                if (useSession)
                    return DataHelper.stringParse(SBSession.GetSessionValue(Sessions.UserName));
                else
                    return clsWebCookie.UserName;
            }
        }

        public static string ProfileImageURL
        {
            get
            {
                if (useSession)
                {
                    return DataHelper.stringParse(SBSession.GetSessionValue(Sessions.ProfileImageURL));
                }
                else
                    return clsWebCookie.ProfileImageURL;
            }
        }

        public static DateTime LoginTime
        {
            get
            {
                if (useSession)
                {
                    return DataHelper.dateParse(SBSession.GetSessionValue(Sessions.LoginTime));
                }
                else
                    return clsWebCookie.LoginTime;
            }
        }

        public static bool Subscription
        {
            get
            {
                if (useSession)
                {
                    if (SBSession.GetSessionValue(Sessions.SubscriptionStatus).ToString() == "Active" || SBSession.GetSessionValue(Sessions.SubscriptionStatus).ToString() == "Cancelled")
                        return true;
                    else
                        return false;
                }
                else
                {
                    if (clsWebCookie.SubscriptionStatus == "Active" || clsWebCookie.SubscriptionStatus == "Cancelled")
                        return true;
                    else
                        return false;    
                }
            }
        }

        public static string SubscriptionType
        {
            get
            {
                if (useSession)
                {
                    return SBSession.GetSessionValue(Sessions.SubscriptionType).ToString();
                }
                else
                {
                    return clsWebCookie.SubscriptionType;
                }
            }
        }

        public static void Logout()
        {
            SBSession.Logout();
        }
        
        public static bool StripeTestMode
        {
            get
            {
                return DataHelper.boolParse(System.Configuration.ConfigurationManager.AppSettings["stripe_test_mode"]);
            }
        }

        public static string StripePublishKey
        {
            get
            {
                if (StripeTestMode)
                    return System.Configuration.ConfigurationManager.AppSettings["stripe_test_publishable_key"].ToString();
                else
                    return System.Configuration.ConfigurationManager.AppSettings["stripe_publishable_key"].ToString();
            }
        }

        public static string StripeSecretKey
        {
            get
            {
                if (StripeTestMode)
                    return System.Configuration.ConfigurationManager.AppSettings["stripe_test_secret_key"].ToString();
                else
                    return System.Configuration.ConfigurationManager.AppSettings["stripe_secret_key"].ToString();
            }
        }
    }
}
