using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;

namespace FraudReporting.Utilities
{
    /// <summary>
    /// Settings for handling the live date range of the website. Only needed for sites that start and/or end sometime reasonable in the future.
    /// </summary>
    public class LiveDateRangeSettings
    {
        /// <summary>
        /// What part of the site this applies to. (Defaults to "Whole Site" as most sites are either active or completely inactive.)
        /// </summary>
        public string SiteSection { get; set; }

        /// <summary>
        /// The start date of the website.
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// The end date of the website.
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// The route to redirect to before the date range is valid. Defaults to Area "", Controller "Notification", Action "ComingSoon".
        /// </summary>
        public RouteValueDictionary RedirectToBeforeDate { get; set; }

        /// <summary>
        /// The route to redirect to before the date range is valid. Defaults to Area "", Controller "Notification", Action "ProgramEnded".
        /// </summary>
        public RouteValueDictionary RedirectToAfterDate { get; set; }

        /// <summary>
        /// Settings for handling the live date range of the website. Only needed for sites that start and/or end sometime reasonable in the future.
        /// </summary>
        public LiveDateRangeSettings()
        {
            SiteSection = "Whole Site";
            StartDate = null;
            EndDate = null;
            RedirectToBeforeDate = new RouteValueDictionary { { "area", "" }, { "controller", "Notification" }, { "action", "ComingSoon" } };
            RedirectToAfterDate = new RouteValueDictionary { { "area", "" }, { "controller", "Notification" }, { "action", "ProgramEnded" } };
        }
    }
}
