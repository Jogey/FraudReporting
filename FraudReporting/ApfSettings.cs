using System;
using System.Collections.Generic;
using Apfco;
using Apfco.Web.Settings;
using static Apfco.Web.Settings.WebsiteSettings;

namespace FraudReporting
{
    /// <summary>
    /// A static class used to store global application variables, as well as helper functions to accomodate ASP.NET development
    /// within Arrowhead's systems. 
    /// </summary>
    public static class ApfSettings
    {
        // Current version of the template. Please develop on the most up-to-date version. Read up on the latest updates/changes made to the template here: https://github.com/APFCO/FraudReporting
        public static readonly string Version = "1.1.0";

        public static WebsiteSettings Website = new WebsiteSettings
        {
            /* 
             * Note:    Remember to go into web.config and web.release.config and change PromoDatabase's Initial Catalog to the correct 
             *          value (since it's ApfcoWebMvcCoreTemplate/ApfcoWebMvcCoreTemplateStage by default which will get replaced by Visual Studio 
             *          with possibly the wrong value)
             */

            Promo = "FraudReporting",
            PromoDescription = "Fraud Analytics Tool",
            Title = "Fraud Analytics Tool",
            EmailDomain = "apfco.com",
            ProgrammerEmailList = new List<EmailListEntry>
            {
                new EmailListEntry { Type = "Primary", EmailAddress = "jogeyv@apfco.com" }
            },
            CustomerServiceEmailList = new List<EmailListEntry>
            {
                new EmailListEntry { Type = "Primary", EmailAddress = "promotions@apfco.com" }
            }
        };

        public static GoogleAnalyticsSettings GoogleAnalytics = new GoogleAnalyticsSettings
        {
            TrackingId = null
        };

        public static List<FraudReporting.Utilities.LiveDateRangeSettings> LiveDateRanges = new List<FraudReporting.Utilities.LiveDateRangeSettings>();

        public static bool requireHttps = true;

        // Set the start and end dates
        private static Dictionary<string, int> timeZone = new Dictionary<string, int>()
        {
            // Time zone conversion relative to Arrowhead's servers clock
            { "PST", 2 }, { "MST", 1 }, { "CST", 0 }, { "EST", -1 }
        };
        public static DateTime LiveStartDate = new DateTime(1999, 1, 1, 0, 0, 0).AddHours(timeZone["CST"]);
        public static DateTime LiveEndDate = new DateTime(3000, 1, 1, 23, 59, 59).AddHours(timeZone["CST"]);

        /// <summary>
        /// This function should be called only ONCE, and ONLY in the Application_Start() function in Global.asax. Because these are
        /// static variables, they're not thread-safe between requests from multiple users. They should not have their values changed
        /// at any time after the application has started.
        /// </summary>
        public static void Initialize()
        {
            // Initialize the error email list with the programmer email list
            Website.ErrorEmailList = Website.ProgrammerEmailList;

            // Set variables depending on DEBUG constant
            if (IsDebugBuild)
            {
            }
            else
            {
                Website.ErrorEmailList.Add(new EmailListEntry { Type = "Group", EmailAddress = "webpgmr@apfco.com" });

                LiveDateRanges.Add(new FraudReporting.Utilities.LiveDateRangeSettings // SiteSection defaults to "Whole Site", so don't need to set it. Same for RedirectToBeforeDate and RedirectToAfterDate. They default to the typical controller and actions. (And no area)
                {
                    StartDate = LiveStartDate,
                    EndDate = LiveEndDate //Can also be null if no end date
                });

                Website.IsUnderConstruction = false;
            }
        }

        /// <summary>
        /// Should only be used in views, as views don't easily support compiler flags. Should use compiler flags elsewhere.
        /// </summary>
        public static bool IsDebugBuild
        {
            get
            {
#if (PRODUCTION)
				return false;
#else
                return true;
#endif
            }
        }
    }
}