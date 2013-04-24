using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Helpers;
using umbraco.BusinessLogic;

namespace Bootstrap.Logic.Utils
{
    public static class MainHelper
    {
        public const string XmlSitemapCache = "SitemapXml{0}";
        public const string FeedCache = "Feed{0}";
        public const string EmailRegex = @"^([^@\s]+)@((?:[-a-z0-9]+\.)+[a-z]{2,})$";

        public static bool TrySendMail(string recipients, string subject, string body, CultureInfo ci, int nodeId = -1)
        {
            using (var smtp = new SmtpClient())
            {
                var credentials = (NetworkCredential)smtp.Credentials;
                var from = credentials.UserName;
                try
                {
                    // Assign a sender, recipient
                    using (var message = new MailMessage(from, recipients))
                    {
                        // Assign subject to new mail message
                        message.Subject = subject;

                        // Set encoding to UTF8
                        message.HeadersEncoding = message.SubjectEncoding = message.BodyEncoding = Encoding.UTF8;

                        // Set the Content-Language header
                        message.Headers.Add("Content-Language", ci.TwoLetterISOLanguageName);

                        // Define the plain text alternate view and add to message
                        AlternateView plainTextView = AlternateView.CreateAlternateViewFromString(body, Encoding.UTF8, MediaTypeNames.Text.Plain);
                        message.AlternateViews.Add(plainTextView);

                        // Define the html alternate view and add to message
                        const string htmlBodyTemplate = "<!doctype html>" +
                                                        "<html lang=\"{0}\">" +
                                                        "<head><meta charset=\"utf-8\"/><title>{1}</title></head>" +
                                                        "<body{2}>{3}</body>" +
                                                        "</html>";
                        string htmlBody = string.Format(htmlBodyTemplate, 
                                                        ci.TwoLetterISOLanguageName, 
                                                        subject,
                                                        ci.TextInfo.IsRightToLeft ? " dir=\"rtl\"" : string.Empty,
                                                        body.Replace(Environment.NewLine, "<br/>"));

                        // Should I set the encoding from the current culture?
                        // var encoding = Encoding.GetEncoding(ci.TextInfo.ANSICodePage);
                        AlternateView htmlView = AlternateView.CreateAlternateViewFromString(htmlBody, Encoding.UTF8, MediaTypeNames.Text.Html);
                        message.AlternateViews.Add(htmlView);

                        // Send the message
                        smtp.Send(message);
                        Log.Add(LogTypes.Custom, nodeId, "A new email has been sent from " + from + " to " + recipients);
                        return true;   
                    }
                }
                catch (Exception ex)
                {
                    Log.Add(LogTypes.Error, nodeId, String.Format("Message not sent! " + ex.Message + "|" + ex.StackTrace));
                    Log.Add(LogTypes.Error, nodeId, String.Format("Message contents: " + "from:" + from + ";to:" + recipients + ";subject:" + subject + ";body:" + body));
                    return false;
                }
            }
        }

        public static string ConstructQueryString(this NameValueCollection parameters, string delimiter = "&", bool omitEmpty = true)
        {
            var items = new List<string>();
            for (var i = 0; i < parameters.Count; i++)
            {
                var strings = parameters.GetValues(i);
                if (strings == null)
                {
                    continue;
                }

                foreach (var value in strings)
                {
                    var addValue = !(omitEmpty) || !String.IsNullOrEmpty(value);
                    if (addValue)
                    {
                        items.Add(String.Concat(parameters.GetKey(i), '=', HttpUtility.UrlEncode(value)));
                    }
                }
            }

            return String.Join(delimiter, items.ToArray());
        }

        public static HtmlString GetAntiForgeryHtml(this HttpContextBase context)
        {
            return AntiForgery.GetHtml(context, String.Empty, String.Empty, String.Empty);
        }

        public static bool IsValidAntiForgery(this HttpContextBase context)
        {
            try
            {
                AntiForgery.Validate(context, String.Empty);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsMatch(this string value, string regex)
        {
            if (String.IsNullOrEmpty(value))
            {
                return false;
            }

            var re = new Regex(regex, RegexOptions.IgnoreCase);
            return re.IsMatch(value);
        }

        public static bool IsNullOrWhiteSpace(this string value)
        {
            return String.IsNullOrWhiteSpace(value);
        }

    }
}
