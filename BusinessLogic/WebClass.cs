using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BusinessLogic
{
    public class WebClass
    {
        public SmartEcommerce.Models.Web.HomePage GetDefault()
        {
            SmartEcommerce.Models.Web.HomePage result = new SmartEcommerce.Models.Web.HomePage();

            try
            {
                XmlDocument xmldoc = new XmlDocument();
                XmlElement doc = xmldoc.CreateElement("doc");
                xmldoc.AppendChild(doc);

                foreach (long itemId in clsCookie.RecentlyViewedProducts)
                {
                    XmlElement item = xmldoc.CreateElement("item");
                    doc.AppendChild(item);

                    item.AppendChild(xmldoc.CreateElement("ItemId")).InnerText = itemId.ToString();
                }

                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param = {
                                        new SqlParameter("@ViewedItems", SqlDbType.Xml)
                                        { Value = new SqlXml(new XmlTextReader(xmldoc.InnerXml, XmlNodeType.Document, null)) },
                                        new SqlParameter("@LoginId",BusinessLogic.clsWebSession.LoginId)
                                    };

                DataTableCollection dtbls = DatabaseObject.FetchFromSP("Web_GetHomePage", param, ref response);

                #region Slider
                foreach (DataRow row in dtbls[0].Rows)
                {
                    string image_url = DataHelper.getSliderImageURL(row["ImageURL"].ToString());
                    if (image_url != "")
                    {
                        result.Sliders.Add(new SmartEcommerce.Models.Web.Sliders
                        {
                            ImageURL = image_url,
                            RedirectionURL = row["RedirectionURL"].ToString()
                        });
                    }
                }
                #endregion Slider

                #region Recent Watch

                foreach (DataRow row in dtbls[1].Rows)
                {
                    result.RecentWatch.Add(new SmartEcommerce.Models.Product.Videos
                    {
                        Id = DataHelper.intParse(row["Id"]),
                        Title = row["Title"].ToString(),
                        ImageURL = DataHelper.getVideoImageURL(row["ImageURL"].ToString()),
                        CreatedOn = DataHelper.dateParse(row["AddedOn"]),
                        Likes = DataHelper.intParse(row["Likes"]),
                        Dislikes = DataHelper.intParse(row["DisLikes"]),
                        Views = DataHelper.intParse(row["Views"]),
                        IsList = DataHelper.intParse(row["IsList"]),
                        URL = "/watch?v=" + SBEncryption.Encrypt(DataHelper.stringParse(row["Id"])),
                        CreatedBy = row["UserName"].ToString(),
                        ProfileURL = DataHelper.FriendlyUrlParse(row["UserName"].ToString())
                    });
                }

                #endregion Recent Watch

                #region New Arrival

                foreach (DataRow row in dtbls[2].Rows)
                {
                    result.NewArrivals.Add(new SmartEcommerce.Models.Product.Videos
                    {
                        Id = DataHelper.intParse(row["Id"]),
                        Title = row["Title"].ToString(),
                        ImageURL = DataHelper.getVideoImageURL(row["ImageURL"].ToString()),
                        CreatedOn = DataHelper.dateParse(row["AddedOn"]),
                        Likes = DataHelper.intParse(row["Likes"]),
                        Dislikes = DataHelper.intParse(row["DisLikes"]),
                        Views = DataHelper.intParse(row["Views"]),
                        IsList = DataHelper.intParse(row["IsList"]),
                        URL = "/watch?v=" + SBEncryption.Encrypt(DataHelper.stringParse(row["Id"])),
                        CreatedBy = row["UserName"].ToString(),
                        ProfileURL = DataHelper.FriendlyUrlParse(row["UserName"].ToString())
                    });
                }

                #endregion New Arrival

                #region Category Wise Data

                DataView dv = dtbls[3].AsDataView();
                dv.Sort = "Priority";

                int previous_category = 0;
                SmartEcommerce.Models.Product.Category category = new SmartEcommerce.Models.Product.Category();

                foreach (DataRowView row in dv)
                {
                    if (previous_category != DataHelper.intParse(row["CategoryId"]))
                    {
                        if (previous_category > 0)
                            result.Categories.Add(category);

                        category = new SmartEcommerce.Models.Product.Category()
                        {
                            Id = DataHelper.intParse(row["CategoryId"]),
                            Title = row["CategoryName"].ToString(),
                            URL = "/category/" + DataHelper.FriendlyUrlParse(row["CategoryName"].ToString())
                        };
                    }

                    category.Videos.Add(new SmartEcommerce.Models.Product.Videos
                    {
                        Id = DataHelper.intParse(row["Id"]),
                        Title = row["Title"].ToString(),
                        ImageURL = DataHelper.getVideoImageURL(row["ImageURL"].ToString()),
                        CreatedOn = DataHelper.dateParse(row["AddedOn"]),
                        Likes = DataHelper.intParse(row["Likes"]),
                        Dislikes = DataHelper.intParse(row["DisLikes"]),
                        Views = DataHelper.intParse(row["Views"]),
                        IsList = DataHelper.intParse(row["IsList"]),
                        URL = "/watch?v=" + SBEncryption.Encrypt(DataHelper.stringParse(row["Id"])),
                        CreatedBy = row["UserName"].ToString(),
                        ProfileURL = DataHelper.FriendlyUrlParse(row["UserName"].ToString())
                    });

                    previous_category = DataHelper.intParse(row["CategoryId"]);
                }

                if (previous_category > 0)
                    result.Categories.Add(category);


                #endregion Category Wise Data

                #region Live Streaming

                foreach (DataRow row in dtbls[4].Rows)
                {
                    result.LiveStreaming.Add(new SmartEcommerce.Models.Product.LiveStreaming
                    {
                        Id = DataHelper.intParse(row["Id"]),
                        Title = row["Title"].ToString(),
                        ImageURL = DataHelper.getLiveStreamingImageURL(row["ImageURL"].ToString()),
                        VideoURL = row["VideoURL"].ToString(),
                        CreatedOn = DataHelper.dateParse(row["AddedOn"]),
                        Likes = DataHelper.intParse(row["Likes"]),
                        Dislikes = DataHelper.intParse(row["DisLikes"]),
                        Views = DataHelper.intParse(row["Views"]),
                        URL = "/live-tv/" + DataHelper.FriendlyUrlParse(row["Title"].ToString())
                    });
                }

                #endregion Live Streaming
                
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
            }

            return result;
        }
    }
}
