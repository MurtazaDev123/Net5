using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public static class SBSession
    {
        public static object SessionValue(object session)
        {
            SessionClass sess = new SessionClass();
            return sess.SessionValue(session);
        }

        public static void ClearSession(object session)
        {
            SessionClass sess = new SessionClass();
            sess.ClearSession(session);
        }

        public static bool SessionExists(object session)
        {
            SessionClass sess = new SessionClass();
            return sess.SessionExists(session);
        }

        public static void CreateSession(object session, object sessionValue)
        {
            SessionClass sess = new SessionClass();
            sess.CreateSession(session, sessionValue);
        }

        public static object GetSessionValue(object session)
        {
            SessionClass sess = new SessionClass();
            return sess.SessionValue(session);
        }

        public static void Logout()
        {
            SessionClass sess = new SessionClass();
            sess.Logout();
        }
    }

    class SessionClass : System.Web.UI.Page
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        /// <summary>
        /// Sessions the exists.
        /// Checks that Session is exists or not
        /// </summary>
        /// <param name="session">The session.</param>
        /// <returns></returns>
        public bool SessionExists(object session)
        {
            try
            {
                if (Session[session.ToString()] != null)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Creates the session.
        /// Create New Session and put assigned value into Session
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="sessionValue">The session value.</param>
        public void CreateSession(object session, object sessionValue)
        {
            Session[session.ToString()] = sessionValue;
        }

        /// <summary>
        /// Sessions the value.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <returns></returns>
        public object SessionValue(object session)
        {
            try
            {
                return Session[session.ToString()];
            }
            catch (Exception)
            {
                return "";
            }
        }

        public void ClearSession(object session)
        {
            if (SessionExists(session))
            {
                Session.Remove(session.ToString());
            }
        }

        public void Logout()
        {
            Session.RemoveAll();
        }

    }

    public class Sessions
    {
        public const string StoreId = "StoreId";
        public const string LoginId = "LoginId";
        public const string UserId = "UserId";
        public const string UserName = "UserName";
        public const string LoginTime = "LoginTime";
        public const string LoginType = "LoginType";
        public const string ProfileImageURL = "ProfileImageURL";
        public const string Basket = "Basket";
        public const string SubscriptionStatus = "SubscriptionStatus";
        public const string SubscriptionType = "SubscriptionType";
        public const string PartnerCategoryId = "PartnerCategoryId";
    }
}
