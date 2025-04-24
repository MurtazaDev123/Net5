using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace BusinessLogic
{
    public static class DataHelper
    {
        
        public static readonly string _DefaultDateFormat = "dd-MMM-yyyy";
        private static readonly Regex _properNameRx = new Regex(@"\b(\w+)\b");
        private static readonly string[] _prefixes = { "mc" };
        private static List<string> _BackgroundColours = new List<string> { "FF6D00" };

        public static string stringParse(object value)
        {
            try
            {
                if (value != null)
                {
                    return value.ToString();
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

        public static int intParse(object value)
        {
            try
            {
                if (value != null)
                {
                    int i = 0;
                    string[] values = value.ToString().Split('.');


                    int.TryParse(replace_values(values[0]), out i);
                    return i;
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

        public static long longParse(object value)
        {
            try
            {
                if (value != null)
                {
                    long i = 0;
                    long.TryParse(replace_values(value), out i);
                    return i;
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

        public static bool boolParse(object value)
        {
            try
            {
                if (value != null)
                {
                    bool i = false;

                    if (value.ToString().ToLower() == "on")
                        return true;

                    bool.TryParse(replace_values(value), out i);
                    return i;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static double doubleParse(object value)
        {
            try
            {
                if (value != null)
                {
                    double i = 0;
                    double.TryParse(replace_values(value), out i);
                    return i;
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

        public static float floatParse(object value)
        {
            try
            {
                if (value != null)
                {
                    float i = 0;
                    float.TryParse(replace_values(value), out i);
                    return i;
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

        public static decimal decimalParse(object value)
        {
            try
            {
                if (value != null)
                {
                    decimal i = 0;
                    decimal.TryParse(replace_values(value), out i);
                    return i;
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

        public static decimal decimalParse(object value, decimal defaultValue)
        {
            try
            {
                if (value != null)
                {
                    decimal i = defaultValue;
                    decimal.TryParse(replace_values(value), out i);
                    return i;
                }
                else
                {
                    return defaultValue;
                }
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        public static string dateOnly(object value)
        {
            try
            {
                if (value != null)
                {
                    DateTime i;
                    if (DateTime.TryParse(value.ToString().Replace(",", "").Replace("_", ""), out i))
                        return i.ToString(_DefaultDateFormat);

                    return "";
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

        public static DateTime dateParse(object value)
        {
            try
            {
                if (value != null)
                {
                    DateTime i;
                    if (DateTime.TryParse(value.ToString(), out i))
                        return i;

                    return DateTime.Now;
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

        public static int MinOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector, int defaultValue)
        {
            if (source.Any<TSource>())
                return source.Min<TSource>(selector);

            return defaultValue;
        }

        public static decimal MinOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector, decimal defaultValue)
        {
            if (source.Any<TSource>())
                return source.Min<TSource>(selector);

            return defaultValue;
        }

        public static int MaxOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector, int defaultValue)
        {
            if (source.Any<TSource>())
                return source.Max<TSource>(selector);

            return defaultValue;
        }

        public static decimal MaxOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector, decimal defaultValue)
        {
            if (source.Any<TSource>())
                return source.Max<TSource>(selector);

            return defaultValue;
        }

        public static int RowCount(System.Data.DataTable dt)
        {
            int row = 0;
            if (dt != null)
            {
                row = dt.Rows.Count;
            }
            return row;
        }

        public static bool HasRows(System.Data.DataTable dt)
        {
            bool has = false;
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                    has = true;
            }
            return has;
        }

        public static string FriendlyUrlParse(string value)
        {
            string newstr = stringParse(value);
            var regexItem = new Regex("[^a-zA-Z0-9_.]+");

            if (value != null)
            {
                if (value.Length > 0)
                {
                    if (regexItem.IsMatch(value[value.Length - 1].ToString()))
                    {
                        newstr = value.Remove(value.Length - 1);
                    }
                }
            }

            string replacestr = Regex.Replace(newstr, "[^a-zA-Z0-9_]+", "-");
            return replacestr.ToLower();
        }

        public static string FriendURLDB(string fieldName)
        {
            return "replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(replace(" + fieldName + ", '-', '_'), '/', ' '), ' ', '-'), '''', ''), '\"',''), '?', ''), '&', 'and_and'), '!', ''), '.', '-'), '---', ''), '*', 'x'), '+', 'plus')";
        }

        public static string GetIPAddress()
        {
            string ipaddress;
            //ipaddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            //if (ipaddress == "" || ipaddress == null)
            //    ipaddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            ipaddress = new System.Net.WebClient().DownloadString("http://icanhazip.com");

            return ipaddress.Trim();
        }

        public static string FormatNumber(long num)
        {
            // Ensure number has max 3 significant digits (no rounding up can happen)
            long i = (long)Math.Pow(10, (int)Math.Max(0, Math.Log10(num) - 2));
            num = num / i * i;

            if (num >= 1000000000)
                return (num / 1000000000D).ToString("0.##") + "B";
            if (num >= 1000000)
                return (num / 1000000D).ToString("0.##") + "M";
            if (num >= 1000)
                return (num / 1000D).ToString("0.##") + "K";

            return num.ToString("#,0");
        }

        public static string TimeAgo(DateTime dateTime)
        {
            string result = string.Empty;
            var timeSpan = DateTime.Now.Subtract(dateTime);

            if (timeSpan <= TimeSpan.FromSeconds(60))
            {
                result = string.Format("{0} seconds ago", timeSpan.Seconds);
            }
            else if (timeSpan <= TimeSpan.FromMinutes(60))
            {
                result = timeSpan.Minutes > 1 ?
                    String.Format("{0} minutes ago", timeSpan.Minutes) :
                    "about a minute ago";
            }
            else if (timeSpan <= TimeSpan.FromHours(24))
            {
                result = timeSpan.Hours > 1 ?
                    String.Format("{0} hours ago", timeSpan.Hours) :
                    "about an hour ago";
            }
            else if (timeSpan <= TimeSpan.FromDays(30))
            {
                result = timeSpan.Days > 1 ?
                    String.Format("{0} days ago", timeSpan.Days) :
                    "yesterday";
            }
            else if (timeSpan <= TimeSpan.FromDays(365))
            {
                result = timeSpan.Days > 30 ?
                    String.Format("{0} months ago", timeSpan.Days / 30) :
                    "a month ago";
            }
            else
            {
                result = timeSpan.Days > 365 ?
                    String.Format("{0} years ago", timeSpan.Days / 365) :
                    "about a year ago";
            }

            return result;
        }

        public static string ToProperCase(this string original)
        {
            if (string.IsNullOrEmpty(original))
                return original;

            string result = _properNameRx.Replace(original.ToLower(CultureInfo.CurrentCulture), HandleWord);
            return result;
        }

        public static string WordToProperCase(this string word)
        {
            try
            {
                if (string.IsNullOrEmpty(word))
                    return word;

                if (word.Length > 1)
                    return Char.ToUpper(word[0], CultureInfo.CurrentCulture) + word.Substring(1);

                return word.ToUpper(CultureInfo.CurrentCulture);
            }
            catch (Exception)
            {
                return word;
            }
        }

        #region NumericToWord

        public static String changeNumericToWords(double numb)
        {
            String num = numb.ToString();
            return changeToWords(num, false);
        }

        public static String changeCurrencyToWords(String numb)
        {
            return changeToWords(numb, true);
        }

        public static String changeNumericToWords(String numb)
        {
            return changeToWords(numb, false);
        }

        public static String changeCurrencyToWords(double numb)
        {
            return changeToWords(numb.ToString(), true);
        }

        public static String changeToWords(String numb, bool isCurrency)
        {
            String val = "", wholeNo = numb, points = "", andStr = "", pointStr = "";
            String endStr = (isCurrency) ? ("Only") : ("");
            try
            {
                int decimalPlace = numb.IndexOf(".");
                if (decimalPlace > 0)
                {
                    wholeNo = numb.Substring(0, decimalPlace);
                    points = numb.Substring(decimalPlace + 1);
                    if (Convert.ToInt32(points) > 0)
                    {
                        andStr = (isCurrency) ? ("and") : ("point");// just to separate whole numbers from points/cents
                        endStr = (isCurrency) ? ("Cents " + endStr) : ("");
                        pointStr = translateCents(points);
                    }
                }
                val = String.Format("{0} {1}{2} {3}", translateWholeNumber(wholeNo).Trim(), andStr, pointStr, endStr);
            }
            catch {; }
            return val;
        }

        public static String translateWholeNumber(String number)
        {
            string word = "";
            try
            {
                bool beginsZero = false;//tests for 0XX
                bool isDone = false;//test if already translated
                double dblAmt = (Convert.ToDouble(number));
                //if ((dblAmt > 0) && number.StartsWith("0"))
                if (dblAmt > 0)
                {//test for zero or digit zero in a nuemric
                    beginsZero = number.StartsWith("0");
                    int numDigits = number.Length;
                    int pos = 0;//store digit grouping
                    String place = "";//digit grouping name:hundres,thousand,etc...
                    switch (numDigits)
                    {
                        case 1://ones' range
                            word = ones(number);
                            isDone = true;
                            break;
                        case 2://tens' range
                            word = tens(number);
                            isDone = true;
                            break;
                        case 3://hundreds' range
                            pos = (numDigits % 3) + 1;
                            place = " Hundred ";
                            break;
                        case 4://thousands' range
                        case 5:
                        case 6:
                            pos = (numDigits % 4) + 1;
                            place = " Thousand ";
                            break;
                        case 7://millions' range
                        case 8:
                        case 9:
                            pos = (numDigits % 7) + 1;
                            place = " Million ";
                            break;
                        case 10://Billions's range
                            pos = (numDigits % 10) + 1;
                            place = " Billion ";
                            break;
                        //add extra case options for anything above Billion...
                        default:
                            isDone = true;
                            break;
                    }
                    if (!isDone)
                    {//if transalation is not done, continue...(Recursion comes in now!!)
                        word = translateWholeNumber(number.Substring(0, pos)) + place + translateWholeNumber(number.Substring(pos));
                        //check for trailing zeros
                        if (beginsZero) word = " and " + word.Trim();
                    }
                    //ignore digit grouping names
                    if (word.Trim().Equals(place.Trim())) word = "";
                }
            }
            catch {; }
            return word.Trim();
        }

        public static String tens(String digit)
        {
            int digt = Convert.ToInt32(digit);
            String name = null;
            switch (digt)
            {
                case 10:
                    name = "Ten";
                    break;
                case 11:
                    name = "Eleven";
                    break;
                case 12:
                    name = "Twelve";
                    break;
                case 13:
                    name = "Thirteen";
                    break;
                case 14:
                    name = "Fourteen";
                    break;
                case 15:
                    name = "Fifteen";
                    break;
                case 16:
                    name = "Sixteen";
                    break;
                case 17:
                    name = "Seventeen";
                    break;
                case 18:
                    name = "Eighteen";
                    break;
                case 19:
                    name = "Nineteen";
                    break;
                case 20:
                    name = "Twenty";
                    break;
                case 30:
                    name = "Thirty";
                    break;
                case 40:
                    name = "Fourty";
                    break;
                case 50:
                    name = "Fifty";
                    break;
                case 60:
                    name = "Sixty";
                    break;
                case 70:
                    name = "Seventy";
                    break;
                case 80:
                    name = "Eighty";
                    break;
                case 90:
                    name = "Ninety";
                    break;
                default:
                    if (digt > 0)
                    {
                        name = tens(digit.Substring(0, 1) + "0") + " " + ones(digit.Substring(1));
                    }
                    break;
            }
            return name;
        }

        public static String ones(String digit)
        {
            int digt = Convert.ToInt32(digit);
            String name = "";
            switch (digt)
            {
                case 1:
                    name = "One";
                    break;
                case 2:
                    name = "Two";
                    break;
                case 3:
                    name = "Three";
                    break;
                case 4:
                    name = "Four";
                    break;
                case 5:
                    name = "Five";
                    break;
                case 6:
                    name = "Six";
                    break;
                case 7:
                    name = "Seven";
                    break;
                case 8:
                    name = "Eight";
                    break;
                case 9:
                    name = "Nine";
                    break;
            }
            return name;
        }

        public static String translateCents(String cents)
        {
            String cts = "", digit = "", engOne = "";
            for (int i = 0; i < cents.Length; i++)
            {
                digit = cents[i].ToString();
                if (digit.Equals("0"))
                {
                    engOne = "Zero";
                }
                else
                {
                    engOne = ones(digit);
                }
                cts += " " + engOne;
            }
            return cts;
        }

        #endregion NumericToWord

        private static string replace_values(object value)
        {
            return value.ToString().Replace(",", "").Replace("_", "").Replace("(", "-").Replace(")", "");
        }

        public static string TimeSinceEvent(DateTime eventTime)
        {
            TimeSpan timeSince = DateTime.Now - eventTime;

            if (timeSince.Days > 7)
                return string.Format("{0} weeks", Math.Floor(DataHelper.decimalParse(timeSince.Days / 7)));
            else if (timeSince.Days > 0)
                return string.Format("{0} days", timeSince.Days);
            else if (timeSince.Hours > 0)
                return string.Format("{0} hours", timeSince.Hours);
            else if (timeSince.Minutes > 0)
                return string.Format("{0} minutes", timeSince.Minutes);
            else
                return string.Format("{0} seconds", timeSince.Seconds);
        }

        private static string HandleWord(Match m)
        {
            string word = m.Groups[1].Value;

            foreach (string prefix in _prefixes)
            {
                if (word.StartsWith(prefix, StringComparison.CurrentCultureIgnoreCase))
                    return prefix.WordToProperCase() + word.Substring(prefix.Length).WordToProperCase();
            }

            return word.WordToProperCase();
        }

        public static string getProductImageURL(string url, DataHelper.ImgSize imageSize = ImgSize.Thumbnail)
        {
            string imageURL = HttpContext.Current.Server.MapPath("~/content/images/products/" + url);

            if (imageSize == ImgSize.Thumbnail)
            {
                imageURL = HttpContext.Current.Server.MapPath("~/content/images/products/thumb/" + url);
                if (File.Exists(imageURL))
                {
                    imageURL = "content/images/products/thumb/" + url;
                }
                else
                {
                    imageURL = HttpContext.Current.Server.MapPath("~/content/images/products/" + url);
                    if (File.Exists(imageURL))
                        imageURL = "content/images/products/" + url;
                    else
                        imageURL = "content/images/products/not-available.jpg";
                }
            }
            else
            {
                if (File.Exists(imageURL))
                {
                    imageURL = "content/images/products/" + url;
                }
                else
                {
                    imageURL = HttpContext.Current.Server.MapPath("~/content/images/products/thumb/" + url);
                    if (File.Exists(imageURL))
                        imageURL = "content/images/products/thumb/" + url;
                    else
                        imageURL = "content/images/products/not-available.jpg";
                }
            }

            return imageURL;
        }

        public static string getStoreImageURL(string url, DataHelper.ImgSize imageSize = ImgSize.Thumbnail)
        {
            string imageURL = HttpContext.Current.Server.MapPath("~/content/uploads/stores/" + url);

            if (imageSize == ImgSize.Thumbnail)
            {
                imageURL = HttpContext.Current.Server.MapPath("~/content/uploads/stores/thumb/" + url);
                if (File.Exists(imageURL))
                {
                    imageURL = "content/uploads/stores/thumb/" + url;
                }
                else
                {
                    imageURL = HttpContext.Current.Server.MapPath("~/content/uploads/stores/" + url);
                    if (File.Exists(imageURL))
                        imageURL = "content/uploads/stores/" + url;
                    else
                        imageURL = "content/uploads/stores/not-available.jpg";
                }
            }
            else
            {
                if (File.Exists(imageURL))
                {
                    imageURL = "content/uploads/stores/" + url;
                }
                else
                {
                    imageURL = HttpContext.Current.Server.MapPath("~/content/uploads/stores/thumb/" + url);
                    if (File.Exists(imageURL))
                        imageURL = "content/uploads/stores/thumb/" + url;
                    else
                        imageURL = "content/uploads/stores/not-available.jpg";
                }
            }

            return imageURL;
        }

        public static string getStoreCoverImageURL(string url)
        {
            string imageURL = HttpContext.Current.Server.MapPath("~/content/uploads/stores/cover/" + url);

            if (File.Exists(imageURL))
                imageURL = "content/uploads/stores/cover/" + url;
            else
                imageURL = "";

            return imageURL;
        }

        public static string getSliderImageURL(string url)
        {
            string imageURL = HttpContext.Current.Server.MapPath("~/content/uploads/slider/" + url);
            
            if (File.Exists(imageURL))
                imageURL = "content/uploads/slider/" + url;
            else
                imageURL = "";

            return imageURL;
        }

        public static string getBannerImageURL(string url)
        {
            string imageURL = HttpContext.Current.Server.MapPath("~/content/uploads/banners/" + url);

            if (File.Exists(imageURL))
                imageURL = "content/uploads/banners/" + url;
            else
                imageURL = "";

            return imageURL;
        }

        public static string getMallCoverImageURL(string url)
        {
            string imageURL = HttpContext.Current.Server.MapPath("~/content/uploads/malls/cover/" + url);

            if (File.Exists(imageURL))
                imageURL = "content/uploads/malls/cover/" + url;
            else
                imageURL = "";

            return imageURL;
        }

        public static string getMallLogoImageURL(string url)
        {
            string imageURL = HttpContext.Current.Server.MapPath("~/content/uploads/malls/logo/" + url);

            if (File.Exists(imageURL))
                imageURL = "content/uploads/malls/logo/" + url;
            else
                imageURL = "";

            return imageURL;
        }

        public static string getLiveStreamingImageURL(string url)
        {
            string imageURL = HttpContext.Current.Server.MapPath("~/content/uploads/livestreaming/" + url);

            if (File.Exists(imageURL))
                imageURL = "content/uploads/livestreaming/" + System.Web.HttpUtility.UrlPathEncode(url);
            else
                imageURL = "content/uploads/videos/thumb/netfive_video_thumb.png";

            return imageURL;
        }

        public static string getLiveVideosImageURL(string url)
        {
            string imageURL = HttpContext.Current.Server.MapPath("~/content/uploads/livevideos/" + url);

            if (File.Exists(imageURL))
                imageURL = "content/uploads/livevideos/" + System.Web.HttpUtility.UrlPathEncode(url);
            else
                imageURL = "content/uploads/videos/thumb/netfive_video_thumb.png";

            return imageURL;
        }

        public static string getVideoImageURL(string url)
        {
            string imageURL = HttpContext.Current.Server.MapPath("~/content/uploads/videos/thumb/" + url);

            if (File.Exists(imageURL))
                imageURL = "content/uploads/videos/thumb/" + System.Web.HttpUtility.UrlPathEncode(url);
            else
                imageURL = "content/uploads/videos/thumb/netfive_video_thumb.png";

            return imageURL;
        }

        public static string getVideoImageURLMobile(string url)
        {
            string imageURL = HttpContext.Current.Server.MapPath("~/content/uploads/videos/thumb/" + url);

            if (File.Exists(imageURL))
                imageURL = "content/uploads/videos/thumb/" + System.Web.HttpUtility.UrlPathEncode(url);
            else
                imageURL = "content/uploads/videos/thumb/netfive_video_thumb.png";

            return imageURL;
        }

        public static string getProgramImageURL(string url)
        {
            string imageURL = HttpContext.Current.Server.MapPath("~/content/uploads/programs/" + url);

            if (File.Exists(imageURL))
                imageURL = "content/uploads/programs/" + System.Web.HttpUtility.UrlPathEncode(url);
            else
                imageURL = "content/uploads/videos/thumb/netfive_video_thumb.png";

            return imageURL;
        }

        public static string getCategoryImageURL(string url)
        {
            string imageURL = HttpContext.Current.Server.MapPath("~/content/uploads/categories/" + url);

            if (File.Exists(imageURL))
                imageURL = "content/uploads/categories/" + System.Web.HttpUtility.UrlPathEncode(url);
            else
                imageURL = "content/uploads/videos/thumb/netfive_video_thumb.png";

            return imageURL;
        }

        public static string getProfileImageURL(string url)
        {
            string imageURL = HttpContext.Current.Server.MapPath("~/content/uploads/users/" + url);

            if (File.Exists(imageURL))
                imageURL = "content/uploads/users/" + url;
            else
                imageURL = "content/uploads/users/icons-user.png";

            return imageURL;
        }

        public static long GetProductIdByName(string name)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param = { new SqlParameter { ParameterName = "@ProductURL", Value = name } };
                long result = DataHelper.longParse(DatabaseObject.DLookupDB("Web_GetProductIdByName", param, ref response));

                return result;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return 0;
            }
        }

        public static int GetProgramIdByName(string url)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param = { new SqlParameter { ParameterName = "@URL", Value = url } };
                int result = DataHelper.intParse(DatabaseObject.DLookupDB("Web_GetProgramIdByName", param, ref response));

                return result;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return 0;
            }
        }

        public static int GetTvIdByName(string url)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param = { new SqlParameter { ParameterName = "@URL", Value = url } };
                int result = DataHelper.intParse(DatabaseObject.DLookupDB("Web_GetLiveTvIdByName", param, ref response));

                return result;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return 0;
            }
        }

        public static int GetLiveVideosIdByName(string url)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param = { new SqlParameter { ParameterName = "@URL", Value = url } };
                int result = DataHelper.intParse(DatabaseObject.DLookupDB("Web_GetLiveVideosIdByName", param, ref response));

                return result;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return 0;
            }
        }

        public static int GetCategoryIdByName(string url)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param = { new SqlParameter { ParameterName = "@URL", Value = url } };
                int result = DataHelper.intParse(DatabaseObject.DLookupDB("Web_GetCategoryIdByName", param, ref response));

                return result;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return 0;
            }
        }

        public static int GetUserIdByName(string url)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param = { new SqlParameter { ParameterName = "@URL", Value = url } };
                int result = DataHelper.intParse(DatabaseObject.DLookupDB("Web_GetUserIdByName", param, ref response));

                return result;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return 0;
            }
        }

        public static string GetFullNameById(int Id)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param = { new SqlParameter { ParameterName = "@Id", Value = Id } };
                string result = DatabaseObject.DLookupDB("Web_GetFullNameById", param, ref response);

                return result;
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return "";
            }
        }

        public static string Currency
        {
            get
            {
                return "C$";
            }
        }
        
        public static string GenerateProfilePicture(string FullName)
        {

            string[] Names = FullName.Split(' ');
            string firstName = Names.First();
            string lastName = Names.Last();

            var avatarString = string.Format("{0}{1}", firstName[0], lastName[0]).ToUpper();

            var randomIndex = new Random().Next(0, _BackgroundColours.Count - 1);
            var bgColour = _BackgroundColours[randomIndex];

            var bmp = new Bitmap(180, 160);
            var sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;

            var font = new Font("Arial", 72, FontStyle.Bold, GraphicsUnit.Pixel);
            var graphics = Graphics.FromImage(bmp);

            graphics.Clear((Color)new ColorConverter().ConvertFromString("#" + bgColour));
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            graphics.DrawString(avatarString, font, new SolidBrush(Color.WhiteSmoke), new RectangleF(0, 0, 180, 160), sf);

            graphics.Flush();

            string filename = DateTime.Now.Ticks + "_" + firstName.ToLower() + lastName.ToLower() + ".png";

            bmp.Save(HttpContext.Current.Server.MapPath("~/content/uploads/users/" + filename));

            return filename;
        }

        public static DataTable GetCountryFromSession(string ip_address, bool is_mobile_api = false)
        {
            try
            {
                int CountryId = 0; DataTable dt = null;
                ErrorResponse response = new ErrorResponse();
                //CountryId = DataHelper.intParse(SBSession.GetSessionValue("SessionCountryId"));

                if (SBSession.SessionExists("SessionCountryId"))
                {
                    CountryId = DataHelper.intParse(SBSession.GetSessionValue("SessionCountryId"));
                    SqlParameter[] param = { new SqlParameter { ParameterName = "@Id", Value = CountryId } };
                    return dt = DatabaseObject.FetchTableFromSP("CountryGetById", param, ref response);
                }
                else
                {
                    // commit this source because it take server ip even after running from any country it returns canada ip
                    // by touseef on 20 Jul 2020
                    //string ip = GetIPAddress();

                    string ip_access_key = ConfigurationManager.AppSettings["ip_access_key"];

                    var client = new RestClient("http://api.ipapi.com/"+ ip_address +"?access_key="+ ip_access_key +"&format=1");
                    client.Timeout = -1;
                    var request = new RestRequest(Method.GET);
                    IRestResponse responses = client.Execute(request);
                    string json = responses.Content;

                    var data = (JObject)JsonConvert.DeserializeObject(json);
                    string country_code = data["country_code"].Value<string>();

                    SqlParameter[] param = { new SqlParameter { ParameterName = "@Code", Value = country_code } };
                    DataTableCollection dtbls = DatabaseObject.FetchFromSP("CountryGetByCode", param, ref response);

                    DataRow row = dtbls[1].Rows[0];

                    CountryId = DataHelper.intParse(row["CountryId"]);

                    if (!is_mobile_api)
                        SBSession.CreateSession("SessionCountryId", CountryId);

                    return dtbls[0];
                }
                
                
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }
        public static DataTable GetUserDetails(int Id)
        {
            try
            {
                ErrorResponse response = new ErrorResponse();
                SqlParameter[] param = { new SqlParameter { ParameterName = "@Id", Value = Id } };
                DataTableCollection dtbls = DatabaseObject.FetchFromSP("GetUserDetails", param, ref response);
                return dtbls[0];
            }
            catch (Exception ae)
            {
                Logs.WriteError("General", ae.Message);
                return null;
            }
        }
        public enum ImgSize
        {
            Thumbnail,
            Large
        }
    }
}
