using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmartEcommerce.Stripe;
using BusinessLogic;

namespace SmartEcommerce.Controllers
{
    public class StripeApiController : Controller
    {
        //[Route("secret")]
        
        [HttpGet]
        public JsonResult GetSecretKey()
        {
            string customer_name = Request.QueryString["c"].ToString();
            string email = Request.QueryString["e"].ToString();
            long amount = DataHelper.longParse(Request.QueryString["amt"]);
            string currency = Request.QueryString["cur"].ToString();

            var intent = StripePaymentIntent.FetchIntent(customer_name, email, amount, currency); // ... Fetch or create the PaymentIntent
            return Json(new { client_secret = intent.ClientSecret, customer_id = intent.CustomerId }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetSecretKeyTest()
        {
            string customer_name = Request.QueryString["c"].ToString();
            string email = Request.QueryString["e"].ToString();
            long amount = DataHelper.longParse(Request.QueryString["amt"]);
            string currency = Request.QueryString["cur"].ToString();

            var intent = StripePaymentIntent.FetchIntentTest(customer_name, email, amount, currency); // ... Fetch or create the PaymentIntent
            return Json(new { client_secret = intent.ClientSecret, customer_id = intent.CustomerId }, JsonRequestBehavior.AllowGet);
        }
    }
}