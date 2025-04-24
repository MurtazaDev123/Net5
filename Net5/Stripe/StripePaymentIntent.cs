using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BusinessLogic;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace SmartEcommerce.Stripe
{
    public static class StripePaymentIntent
    {
        private static readonly string failure_email = "admin@netfive.tv";
        public static PaymentIntent FetchIntent(string CustomerName, string Email, long Amount, string Currency)
        {
            // Set your secret key. Remember to switch to your live secret key in production!
            // See your keys here: https://dashboard.stripe.com/account/apikeys
            StripeConfiguration.ApiKey = BusinessLogic.clsWebSession.StripeSecretKey;

            var options_customer = new CustomerCreateOptions { Name = CustomerName, Email = Email };
            var custom_service = new CustomerService();
            var customer = custom_service.Create(options_customer);

            Amount = (Amount * 100); // for stripe functionality

            var options = new PaymentIntentCreateOptions
            {
                Amount = Amount,
                Currency = Currency,
                Customer = customer.Id,
                SetupFutureUsage = "off_session",
                // Verify your integration in this guide by including this parameter
                Metadata = new Dictionary<string, string>
                {
                  { "integration_check", "accept_a_payment" },
                },
            };

            var service = new PaymentIntentService();
            PaymentIntent paymentIntent = service.Create(options);

            return paymentIntent;
        }

        public static PaymentIntent FetchIntentTest(string CustomerName, string Email, long Amount, string Currency)
        {
            // Set your secret key. Remember to switch to your live secret key in production!
            // See your keys here: https://dashboard.stripe.com/account/apikeys
            StripeConfiguration.ApiKey = "";

            var options_customer = new CustomerCreateOptions { Name = CustomerName, Email = Email };
            var custom_service = new CustomerService();
            var customer = custom_service.Create(options_customer);

            Amount = (Amount * 100); // for stripe functionality

            var options = new PaymentIntentCreateOptions
            {
                Amount = Amount,
                Currency = Currency,
                Customer = customer.Id,
                SetupFutureUsage = "off_session",
                // Verify your integration in this guide by including this parameter
                Metadata = new Dictionary<string, string>
                {
                  { "integration_check", "accept_a_payment" },
                },
            };

            var service = new PaymentIntentService();
            PaymentIntent paymentIntent = service.Create(options);

            return paymentIntent;
        }

        public static void ChargeSubscriptionPayment()
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param = null;

                DataTable dt = DatabaseObject.FetchTableFromSP("GetExpiredSubscriptions", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    // Send email about failure of charge 
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Failed to load expired subscription data");
                    sb.AppendLine(response.ErrorList[0].Message);

                    Emails.SendMail(failure_email, "NetFive - Subscription failure!", sb.ToString(), true);
                } 
                else
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        string customer_id = row["StripeRefId"].ToString();
                        long amount = DataHelper.longParse(row["SubscriptionAmount"].ToString().Replace(".00",""));
                        string currency = row["SubscriptionCurrency"].ToString();
                        string user_id = row["UserId"].ToString();
                        long Id = DataHelper.longParse(row["Id"]);
                        string user_name = row["UserName"].ToString();
                        string subscription_plan = row["SubscriptionPlan"].ToString();
                        DateTime subscription_end = DataHelper.dateParse(row["SubscriptionEnd"]);

                        if (customer_id != "")
                        {
                            dynamic result = ChargeCustomer(customer_id, amount, currency, user_id);

                            if (result != null)
                            {
                                UpdateDbAndSendEmail(Id, user_id, user_name, currency, amount, customer_id,
                                    result.client_secret, result.payment_intent, result.payment_method, result.status, "", "", "", "", subscription_plan, subscription_end);

                                continue;
                            }
                            else
                            {
                                UpdateDbAndSendEmail(Id, user_id, user_name, currency, amount, customer_id,
                                    "", "", "", "failed", "some_field_missing", "Some fields are missing", "generic_decline", "", subscription_plan, subscription_end);
                            }
                        }
                        else
                        {
                            UpdateDbAndSendEmail(Id, user_id, user_name, currency, amount, customer_id,
                                    "", "", "", "failed", "empty_card", "Card info is missing", "generic_decline", "", subscription_plan, subscription_end);
                        }
                    }
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
            }
        }

        public static object ChargeCustomer(string customerId, long Amount, string Currency, string UserId)
        {
            try
            {
                Amount = (Amount * 100);
                StripeConfiguration.ApiKey = BusinessLogic.clsWebSession.StripeSecretKey;

                var paymentMethods = new PaymentMethodService();
                var availableMethods = paymentMethods.List(new PaymentMethodListOptions
                {
                    Customer = customerId,
                    Type = "card",
                });
                // Charge the customer and payment method immediately
                var paymentIntents = new PaymentIntentService();
                var paymentIntent = paymentIntents.Create(new PaymentIntentCreateOptions
                {
                    Amount = Amount,
                    Currency = Currency,
                    Customer = customerId,
                    PaymentMethod = availableMethods.Data[0].Id,
                    OffSession = true,
                    Confirm = true
                });
                if (paymentIntent.Status == "succeeded")
                {
                    var obj = new
                    {
                        success = true,
                        client_secret = paymentIntent.ClientSecret,
                        payment_intent = paymentIntent.Id,
                        payment_method = paymentIntent.PaymentMethodId,
                        status = paymentIntent.Status
                    };
                    return obj;
                    //Console.WriteLine("✅ Successfully charged card off session");
                }
                else
                {
                    var obj = new
                    {
                        success = false
                    };
                    return obj;
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                // Send email about failure of charge 
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Failed to load expired subscription of " + UserId);
                sb.AppendLine(ae.Message);

                Emails.SendMail(failure_email, "NetFive - Subscription failure!", sb.ToString(), true);

                return null;
            }
        }

        public static void UpdateDbAndSendEmail(long Id, string UserId, string UserName, string Currency, long Amount, 
            string CustomerId, string ClientSecret, string PaymentIntentId, string PaymentMethod, string Status, 
            string ErrorCode, string ErrorMessage, string DeclineCode, string Charge, string SubscriptionPlan, DateTime StartDate)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param =
                {
                    new SqlParameter { ParameterName = "@StripeCustomerId", Value = CustomerId },
                    new SqlParameter { ParameterName = "@StripeAmount", Value = Amount },
                    new SqlParameter { ParameterName = "@ClientSecret", Value = ClientSecret },
                    new SqlParameter { ParameterName = "@StripeCurrency", Value = Currency },
                    new SqlParameter { ParameterName = "@PaymentIntentId", Value = PaymentIntentId },
                    new SqlParameter { ParameterName = "@PaymentMethod", Value = PaymentMethod },
                    new SqlParameter { ParameterName = "@PaymentStatus", Value = Status },
                    new SqlParameter { ParameterName = "@ErrorCode", Value = ErrorCode },
                    new SqlParameter { ParameterName = "@ErrorMessage", Value = ErrorMessage },
                    new SqlParameter { ParameterName = "@DeclineCode", Value = DeclineCode },
                    new SqlParameter { ParameterName = "@Charge", Value = Charge },
                    new SqlParameter { ParameterName = "@LoginId", Value = Id },
                    new SqlParameter { ParameterName = "@SubscriptionPlan", Value = SubscriptionPlan },
                    new SqlParameter { ParameterName = "@StartDate", Value = StartDate }
                };

                DataTable dtResult = DatabaseObject.FetchTableFromSP("PaymentSubscription", param, ref response);
                if (response.Error)
                {
                    Logs.WriteError("General", response.ErrorList[0].Message);
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("NetFive - Subscription failed to update db for " + UserId);
                    sb.AppendLine(response.ErrorList[0].Message);

                    Emails.SendMail(failure_email, "NetFive - Subscription failure!", sb.ToString(), true);
                }
                else
                {

                    string admin_email = ConfigurationManager.AppSettings["admin_email"];

                    if (Status == "succeeded")
                    {
                        long PaymentId = DataHelper.longParse(dtResult.Rows[0]["PaymentId"]);
                        string receipt_no = PaymentId.ToString("000000");

                        StringBuilder sbHtml = new StringBuilder();
                        var path = System.Web.HttpContext.Current.Server.MapPath("~/content/emails/payment-success.html");
                        sbHtml.AppendLine(System.IO.File.ReadAllText(path));

                        sbHtml = sbHtml.Replace("{NAME}", clsWebSession.UserName);
                        sbHtml = sbHtml.Replace("{CURRENCY}", Currency.ToUpper());
                        sbHtml = sbHtml.Replace("{AMOUNT}", Amount.ToString("#,##0.00"));
                        sbHtml = sbHtml.Replace("{RECEIPT_NO}", receipt_no);

                        Emails.SendMail(UserId, admin_email, "NetFive - Payment Receipt!", sbHtml.ToString(), true);
                    }
                    else
                    {
                        StringBuilder sbHtml = new StringBuilder();
                        var path = System.Web.HttpContext.Current.Server.MapPath("~/content/emails/payment-failed.html");
                        sbHtml.AppendLine(System.IO.File.ReadAllText(path));

                        Emails.SendMail(UserId, admin_email, "NetFive - Payment failed!", sbHtml.ToString(), true);
                    }
                }
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("NetFive - Subscription failed to update db and send email for " + UserId);
                sb.AppendLine(ae.Message);

                Emails.SendMail(failure_email, "NetFive - Subscription failure!", sb.ToString(), true);
            }
        }
    }
}