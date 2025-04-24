using BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Http;
using RestSharp;
using System.Data.Entity;

namespace SmartEcommerce
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {

            //Database.SetInitializer<DatabaseContext>(new DropCreateDatabaseIfModelChanges<DatabaseContext>());

            GlobalConfiguration.Configure(WebApiConfig.Register);
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            string CurrentPath = Request.Path.ToLower();
            string ext = System.IO.Path.GetExtension(Request.Url.AbsolutePath);

            string fullOriginalPath = Request.Url.ToString().ToLower();

            if (HttpContext.Current.Request.Url.ToString().ToLower().Contains("http://www.netfive.tv"))
            {
                // new line here
                HttpContext.Current.Response.Clear();
                //
                HttpContext.Current.Response.Status = "301 Moved Permanently";
                HttpContext.Current.Response.AddHeader("Location", Request.Url.ToString().ToLower().Replace("http://www.netfive.tv", "https://www.netfive.tv"));
            }
            else if (HttpContext.Current.Request.Url.ToString().ToLower().Contains("http://netfive.tv"))
            {
                // new line here
                HttpContext.Current.Response.Clear();
                //
                HttpContext.Current.Response.Status = "301 Moved Permanently";
                HttpContext.Current.Response.AddHeader("Location", Request.Url.ToString().ToLower().Replace("http://netfive.tv", "https://www.netfive.tv"));
            }

            //SBSession.CreateSession("SessionCountryId", "Noman");

            //if (HttpContext.Current.Request.Url.ToString().ToLower().Contains("http://web.smartbooks.pk"))
            //{
            //    // new line here
            //    HttpContext.Current.Response.Clear();
            //    //
            //    HttpContext.Current.Response.Status = "301 Moved Permanently";
            //    HttpContext.Current.Response.AddHeader("Location", Request.Url.ToString().ToLower().Replace("http://web.smartbooks.pk", "https://web.smartbooks.pk"));
            //}

            //string d = fullOriginalPath.Replace("http://", "").Replace("https://", "");

            //if (d.IndexOf('/') != -1)
            //{
            //    string other_data = d.Substring(d.IndexOf('/')+1);

            //    if (other_data.Trim() != "")
            //    {
            //        string[] other_data_array = other_data.Split('/');

            //        if (other_data.Trim().Contains("programs"))
            //        {
            //            if (other_data_array.Count() == 2)
            //            {
            //                if (other_data_array[1].Trim() != "")
            //                {
            //                    string programname = other_data_array[1].Trim();
            //                    long programId = DataHelper.GetProgramIdByName(programname);
            //                    if (programId > 0)
            //                    {
            //                        Context.RewritePath("/Web/ProgramDetail/" + programId);
            //                    }
            //                }
            //            }

            //        }
            //        else if (other_data.Trim().Contains("watch"))
            //        {
            //            //if (other_data_array.Count() == 2)
            //            //{
            //            //    if (other_data_array[1].Trim() != "")
            //            //    {
            //            //        string programname = other_data_array[1].Trim();
            //            //        long programId = DataHelper.GetProgramIdByName(other_data_array[0]);
            //            //        if (programId > 0)
            //            //        {
            //            //            Context.RewritePath("/Web/ProgramDetail/" + programId);
            //            //        }
            //            //    }
            //            //}
            //        }
            //    }

            //}


        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }
    }
}
