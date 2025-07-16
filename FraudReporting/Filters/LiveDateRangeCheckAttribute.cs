using System;
using System.Linq;
using FraudReporting.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FraudReporting.Filters
{
    /// <summary>
    /// Handles a list of LiveDateRangeSettings in ApfSettings.LiveDateRanges.
    /// 
    /// Uses the date range in the LiveDateRangeSettings to determine if before or after the live date.
    /// 
    /// Uses the RedirectToBeforeDate and RedirectToAfterDate RouteValueDictionaries in LiveDateRangeSettings when redirecting before or after the live date range.
    /// 
    /// Can be specified multiple times on the same controller/action.
    /// </summary>
    public class LiveDateRangeCheckAttribute : Attribute, IAuthorizationFilter
    {
        /// <summary>
        /// What section of the site this applies to. (Defaults to "Whole Site")
        /// </summary>
        public string SiteSection { get; set; }

        public LiveDateRangeCheckAttribute(string SiteSection = "Whole Site")
        {
            this.SiteSection = SiteSection;
        }

        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            try
            {
                LiveDateRangeSettings liveDateRangeSettings = ApfSettings.LiveDateRanges.FirstOrDefault(x => x.SiteSection == SiteSection);
                if (liveDateRangeSettings != null)
                {
                    if (liveDateRangeSettings.StartDate != null && DateTime.Now < liveDateRangeSettings.StartDate)
                    {
                        filterContext.Result = new RedirectToRouteResult(liveDateRangeSettings.RedirectToBeforeDate);
                    }
                    else if (liveDateRangeSettings.EndDate != null && DateTime.Now > liveDateRangeSettings.EndDate)
                    {
                        filterContext.Result = new RedirectToRouteResult(liveDateRangeSettings.RedirectToAfterDate);
                    }
                }
            }
            catch { }
        }
    }
}