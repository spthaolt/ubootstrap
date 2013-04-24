using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;

using System.Web.Mvc;
using Bootstrap.Logic.Utils;
using Umbraco.Web.Mvc;
using uBootStrapMvc.Site.Models;


namespace uBootStrapMvc.Site.Controllers.SurfaceControllers
{
    public class ContactFormSurfaceController : SurfaceController
    {
        // Method for rendering partial view with @Html.Action("RenderContactForm","ContactFormSurface");
        [ChildActionOnly]
        public ActionResult RenderContactForm()
        {
            return PartialView("ContactForm", new ContactFormModel());
        }

        // Endpoint for posting data to a surface controller action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HandleContactForm(ContactFormModel model)
        {
            
            if (ModelState.IsValid)
            {
                // If validation passes process the form data
                if (ProcessForm(model))
                {
                    // If Form data has been sent, we are all good, continue
                    // Redirect to a specific page
                    model.ThankYouPageId = (string)CurrentPage.GetProperty("thankYouPage").Value;
                    int thankYouPageId;
                    if (int.TryParse(model.ThankYouPageId, out thankYouPageId))
                    {
                        return RedirectToUmbracoPage(thankYouPageId);
                    }

                    // OR, Perhaps you might want to store some data in TempData which will be available 
                    // in the View after the redirect below. An example might be to show a custom 'submit
                    // successful' message on the View, for example:

                    TempData.Add("SubmitMessage", Umbraco.GetDictionaryValue("FormSuccess"));
                    //return RedirectToAction("Success");

                    // redirect to current page to clear the form
                    return RedirectToCurrentUmbracoPage();
                }
            }

            // If we have got here something went wrong (either server validation failed, or the email was not sent properly (check log).
            // Perhaps you might want to add a custom message to the TempData or ViewBag
            // which will be available on the View when it renders (since we're not redirecting)      
            TempData.Add("SubmitMessage", Umbraco.GetDictionaryValue("FormFail"));
            return CurrentUmbracoPage();       
        }

        //public ActionResult Success()
        //{
        //    return PartialView("ContactFormSuccess");
        //}

        private bool ProcessForm(ContactFormModel model)
        {
            // 1. Load model properties with the umbraco node data (validation?)
            model.EmailBody = (string)CurrentPage.GetProperty("emailBody").Value;
            model.EmailReplyBody = (string)CurrentPage.GetProperty("emailReplyBody").Value;
            model.EmailTo = (string)CurrentPage.GetProperty("emailTo").Value;
            model.EmailSubject = (string)CurrentPage.GetProperty("emailSubject").Value;
            model.EmailReplySubject = (string)CurrentPage.GetProperty("emailReplySubject").Value;
            model.PageId = CurrentPage.Id;

            // Set some local values for the email
            var now = DateTime.Now;
            var emailTime = String.Format("{0:HH:mm:ss}", now);
            var emailDate = String.Format("{0:dd/MM/yyyy}", now);

            // 2. Get the values from the form and build the email strings
            var strEmailBody = new StringBuilder(model.EmailBody);
            strEmailBody.Replace("[Name]", model.Name);             // Find and Replace [Name]
            strEmailBody.Replace("[AddressLine1]", model.Address1); // Find and Replace [AddressLine1]
            strEmailBody.Replace("[AddressLine2]", model.Address2); // Find and Replace [AddressLine2]
            strEmailBody.Replace("[Email]", model.Email);           // Find and Replace [Email]
            strEmailBody.Replace("[Message]", model.Message);       // Find and Replace [Message]
            strEmailBody.Replace("[Time]", emailTime);              // Find and Replace [Time]
            strEmailBody.Replace("[Date]", emailDate);              // Find and Replace [Date]

            var strEmailReplyBody = new StringBuilder(model.EmailReplyBody);
            strEmailReplyBody.Replace("[Name]", model.Name);        // Find and Replace [Name]

            // 3. Reply to the sender and notify the site owner
            return MainHelper.TrySendMail(model.EmailTo, model.EmailSubject, strEmailBody.ToString(),
                                          Thread.CurrentThread.CurrentUICulture, model.PageId);
        }

    }
}