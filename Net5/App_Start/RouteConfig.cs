using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SmartEcommerce
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "live-tv",
                url: "live-tv",
                defaults: new { controller = "Web", action = "LiveStreaming", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "live-videos",
                url: "live-videos",
                defaults: new { controller = "Web", action = "LiveVideos", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "programs",
                url: "programs",
                defaults: new { controller = "Web", action = "Programs", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "library",
                url: "library",
                defaults: new { controller = "Web", action = "Library", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "contact-us",
                url: "contact-us",
                defaults: new { controller = "Web", action = "ContactUs", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "about-us",
                url: "about-us",
                defaults: new { controller = "Web", action = "AboutUs", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "payment",
                url: "payment",
                defaults: new { controller = "Web", action = "Payment", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "privacy-and-policy",
                url: "privacy-and-policy",
                defaults: new { controller = "Web", action = "PrivacyAndPolicy", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "terms-and-conditions",
                url: "terms-and-conditions",
                defaults: new { controller = "Web", action = "TermsAndConditions", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "return-policy",
                url: "return-policy",
                defaults: new { controller = "Web", action = "ReturnPolicy", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "career",
                url: "career",
                defaults: new { controller = "Web", action = "Career", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "faqs",
                url: "faqs",
                defaults: new { controller = "Web", action = "FAQS", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "pricing",
                url: "pricing",
                defaults: new { controller = "Web", action = "Pricing", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "edit-profile",
                url: "edit-profile",
                defaults: new { controller = "Web", action = "EditProfile", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "change-password",
                url: "change-password",
                defaults: new { controller = "Web", action = "ChangePassword", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "my-list",
                url: "my-list",
                defaults: new { controller = "Web", action = "MyList", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "login",
                url: "login",
                defaults: new { controller = "Web", action = "Login", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "signup",
                url: "signup",
                defaults: new { controller = "Web", action = "SignUp", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "forget-password",
                url: "forget-password",
                defaults: new { controller = "Web", action = "ForgetPassword", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "category",
                url: "category",
                defaults: new { controller = "Web", action = "Category", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "404",
                url: "404",
                defaults: new { controller = "Web", action = "NotFound", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "error",
                url: "error",
                defaults: new { controller = "Web", action = "ErrorPage", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "ProgramDetail",
                url: "programs/{title}",
                defaults: new { controller = "Web", action = "ProgramDetail", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "LibraryDetail",
                url: "watch",
                defaults: new { controller = "Web", action = "LibraryDetail", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "LiveStreamDetail",
                url: "live-tv/{title}",
                defaults: new { controller = "Web", action = "LiveStreamDetail", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                      name: "LiveVideosDetail",
                      url: "live-videos/{title}",
                      defaults: new { controller = "Web", action = "LiveVideosDetail", id = UrlParameter.Optional }
                  );

            routes.MapRoute(
                      name: "LibraryByCategory",
                      url: "category/{title}",
                      defaults: new { controller = "Web", action = "LibraryByCategory", id = UrlParameter.Optional }
                  );



            routes.MapRoute(
                      name: "Search",
                      url: "results",
                      defaults: new { controller = "Web", action = "Search", id = UrlParameter.Optional }
                  );

            routes.MapRoute(
                   name: "recent-watch",
                   url: "recent-watch",
                   defaults: new { controller = "Web", action = "RecentWatch", id = UrlParameter.Optional }
               );

            routes.MapRoute(
                   name: "signup-complete",
                   url: "signup-complete",
                   defaults: new { controller = "Web", action = "SignupComplete", id = UrlParameter.Optional }
               );

            routes.MapRoute(
                   name: "subscription-plan",
                   url: "subscription-plan",
                   defaults: new { controller = "Web", action = "SubscriptionPlan", id = UrlParameter.Optional }
               );

            routes.MapRoute(
                   name: "subscription-payment",
                   url: "subscription-payment",
                   defaults: new { controller = "Web", action = "SubscriptionPayment", id = UrlParameter.Optional }
               );


            routes.MapRoute(
                name: "subscription-test",
                url: "subscription-test",
                defaults: new { controller = "Web", action = "SubscriptionTest", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "payment-success",
                url: "payment-success",
                defaults: new { controller = "Web", action = "PaymentSuccess", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "payment-failed",
                url: "payment-failed",
                defaults: new { controller = "Web", action = "PaymentFailed", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "checkout",
                url: "checkout",
                defaults: new { controller = "Web", action = "checkout", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "secret",
                url: "secret",
                defaults: new { controller = "StripeApi", action = "GetSecretKey", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "secret-test",
                url: "secret-test",
                defaults: new { controller = "StripeApi", action = "GetSecretKeyTest", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "update-payment",
                url: "update-payment",
                defaults: new { controller = "Web", action = "UpdatePaymentStatus", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                   name: "my-subscription",
                   url: "my-subscription",
                   defaults: new { controller = "Web", action = "SubscriptionMy", id = UrlParameter.Optional }
               );

            routes.MapRoute(
                   name: "partner-request",
                   url: "partner-request",
                   defaults: new { controller = "Web", action = "PartnerRequest", id = UrlParameter.Optional }
               );

            routes.MapRoute(
                   name: "request-complete",
                   url: "request-complete",
                   defaults: new { controller = "Web", action = "PartnerRequestComplete", id = UrlParameter.Optional }
               );

            #region Admin Routes

            routes.MapRoute(
                   name: "admin",
                   url: "admin",
                   defaults: new { controller = "Admin", action = "Index", id = UrlParameter.Optional }
               );

            routes.MapRoute(
                   name: "admin-login",
                   url: "admin/login",
                   defaults: new { controller = "Admin", action = "Login", id = UrlParameter.Optional }
               );

            routes.MapRoute(
                   name: "admin-dashboard",
                   url: "admin/dashboard",
                   defaults: new { controller = "Admin", action = "Dashboard", id = UrlParameter.Optional }
               );

            routes.MapRoute(
                   name: "admin-livestreamings",
                   url: "admin/livestreamings",
                   defaults: new { controller = "Admin", action = "LiveStreamings", id = UrlParameter.Optional }
               );

            routes.MapRoute(
                   name: "admin-livestreamingspending",
                   url: "admin/livestreamingspending",
                   defaults: new { controller = "Admin", action = "LiveStreamingPending", id = UrlParameter.Optional }
               );

            routes.MapRoute(
                   name: "admin-livevideos",
                   url: "admin/livevideos",
                   defaults: new { controller = "Admin", action = "LiveVideos", id = UrlParameter.Optional }
               );

            routes.MapRoute(
                   name: "admin-livevideospending",
                   url: "admin/livevideospending",
                   defaults: new { controller = "Admin", action = "LiveVideosPending", id = UrlParameter.Optional }
               );

            routes.MapRoute(
                  name: "admin-categories",
                  url: "admin/categories",
                  defaults: new { controller = "Admin", action = "Categories", id = UrlParameter.Optional }
              );

            routes.MapRoute(
                 name: "admin-programs",
                 url: "admin/programs",
                 defaults: new { controller = "Admin", action = "Programs", id = UrlParameter.Optional }
             );

            routes.MapRoute(
                 name: "admin-videos",
                 url: "admin/videos",
                 defaults: new { controller = "Admin", action = "Videos", id = UrlParameter.Optional }
             );

            routes.MapRoute(
                 name: "admin-videospending",
                 url: "admin/videospending",
                 defaults: new { controller = "Admin", action = "VideosPending", id = UrlParameter.Optional }
             );

            routes.MapRoute(
                 name: "admin-claimvideos",
                 url: "admin/claimvideos",
                 defaults: new { controller = "Admin", action = "ClaimVideos", id = UrlParameter.Optional }
             );

            routes.MapRoute(
                 name: "admin-reportvideos",
                 url: "admin/reportvideos",
                 defaults: new { controller = "Admin", action = "ReportVideos", id = UrlParameter.Optional }
             );

            routes.MapRoute(
                 name: "admin-subscribers",
                 url: "admin/subscribers",
                 defaults: new { controller = "Admin", action = "Customers", id = UrlParameter.Optional }
             );

            routes.MapRoute(
                 name: "admin-payments",
                 url: "admin/payments",
                 defaults: new { controller = "Admin", action = "Payments", id = UrlParameter.Optional }
             );

            routes.MapRoute(
                 name: "admin-partners",
                 url: "admin/partners",
                 defaults: new { controller = "Admin", action = "Users", id = UrlParameter.Optional }
             );

            routes.MapRoute(
                 name: "admin-sliders",
                 url: "admin/sliders",
                 defaults: new { controller = "Admin", action = "Sliders", id = UrlParameter.Optional }
             );

            routes.MapRoute(
                 name: "admin-topics",
                 url: "admin/topics",
                 defaults: new { controller = "Admin", action = "Topics", id = UrlParameter.Optional }
             );

            routes.MapRoute(
                 name: "admin-country",
                 url: "admin/country",
                 defaults: new { controller = "Admin", action = "Country", id = UrlParameter.Optional }
             );

            routes.MapRoute(
                 name: "admin-state",
                 url: "admin/state",
                 defaults: new { controller = "Admin", action = "State", id = UrlParameter.Optional }
             );

            routes.MapRoute(
                 name: "admin-city",
                 url: "admin/city",
                 defaults: new { controller = "Admin", action = "City", id = UrlParameter.Optional }
             );

            routes.MapRoute(
                 name: "admin-subscription",
                 url: "admin/subscription",
                 defaults: new { controller = "Admin", action = "Subscription", id = UrlParameter.Optional }
             );

            routes.MapRoute(
                 name: "partner-pending",
                 url: "admin/pendingpartners",
                 defaults: new { controller = "Admin", action = "PendingPartners", id = UrlParameter.Optional }
             );

            routes.MapRoute(
                   name: "admin-settings",
                   url: "admin/settings",
                   defaults: new { controller = "Admin", action = "Settings", id = UrlParameter.Optional }
               );

            routes.MapRoute(
                   name: "partner-livestreamingspending",
                   url: "partner/livestreamingspending",
                   defaults: new { controller = "Admin", action = "LiveStreamingPending", id = UrlParameter.Optional }
               );

            routes.MapRoute(
                   name: "partner-livevideos",
                   url: "partner/livevideos",
                   defaults: new { controller = "Admin", action = "LiveVideos", id = UrlParameter.Optional }
               );

            routes.MapRoute(
                   name: "partner-livevideospending",
                   url: "partner/livevideospending",
                   defaults: new { controller = "Admin", action = "LiveVideosPending", id = UrlParameter.Optional }
               );

            #endregion

            #region Admin/Partner Routes

            routes.MapRoute(
                  name: "partner",
                  url: "partner",
                  defaults: new { controller = "Admin", action = "Index", id = UrlParameter.Optional }
              );

            routes.MapRoute(
                   name: "partner-login",
                   url: "partner/login",
                   defaults: new { controller = "Admin", action = "Login", id = UrlParameter.Optional }
               );

            routes.MapRoute(
                   name: "partner-dashboard",
                   url: "partner/dashboard",
                   defaults: new { controller = "Admin", action = "Dashboard", id = UrlParameter.Optional }
               );

            routes.MapRoute(
                   name: "partner-changepassword",
                   url: "partner/changepassword",
                   defaults: new { controller = "Admin", action = "ChangePassword", id = UrlParameter.Optional }
               );

            routes.MapRoute(
                 name: "partner-programs",
                 url: "partner/programs",
                 defaults: new { controller = "Admin", action = "Programs", id = UrlParameter.Optional }
             );

            routes.MapRoute(
                 name: "partner-videos",
                 url: "partner/videos",
                 defaults: new { controller = "Admin", action = "Videos", id = UrlParameter.Optional }
             );

            routes.MapRoute(
                 name: "partner-livestreamings",
                 url: "partner/livestreamings",
                 defaults: new { controller = "Admin", action = "LiveStreamings", id = UrlParameter.Optional }
             );

            #endregion

            routes.MapRoute(
                  name: "LibraryByUser",
                  url: "{title}",
                  defaults: new { controller = "Web", action = "LibraryByUser", id = UrlParameter.Optional }
             );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Web", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
