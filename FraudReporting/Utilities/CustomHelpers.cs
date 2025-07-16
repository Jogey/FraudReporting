using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Apfco.Web.Settings;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace FraudReporting.Utilities
{
    public static class CustomHelpers
    {
        public enum LiveStatus
        {
            BeforeLiveDate = -1,
            Live,
            AfterLiveDate
        }

        public static LiveStatus GetSectionOfSiteLiveStatus(string SectionDescription)
        {
            try
            {
                var liveDateRangeSettings = ApfSettings.LiveDateRanges.First(x => x.SiteSection == SectionDescription);
                if (DateTime.Now < liveDateRangeSettings.StartDate)
                    return LiveStatus.BeforeLiveDate;
                else if (DateTime.Now > liveDateRangeSettings.EndDate)
                    return LiveStatus.AfterLiveDate;
                else
                    return LiveStatus.Live;
            }
            catch
            {
                return LiveStatus.Live;
            }
        }

        /// <summary>
        /// Used to render a notification from TempData.
        /// 
        /// TempData Example:   filterContext.Controller.TempData["Notification"] = "Sample Notification";
        /// Display Example:    @Html.RenderAlert(TempData)
        /// </summary>
        /// <param name="tempData">The TempData object to use</param>
        /// <returns>Either an ampty string if no data to render was found, or and HTML div for the relevant alert.</returns>
        public static IHtmlContent RenderAlert(ITempDataDictionary tempData)
        {
            return RenderAlert(null, tempData);
        }

        /// <summary>
        /// Used to render a notification from TempData. The key name used determines which style of alert to display.
        /// 
        /// TempData Example:   filterContext.Controller.TempData["Notification"] = "Sample Notification";
        /// Display Example:    @Html.RenderAlert(TempData)
        /// </summary>
        /// <param name="helper">The HtmlHelper to use</param>
        /// <param name="tempData">The TempData object to use</param>
        /// <returns>Either an ampty string if no data to render was found, or and HTML div for the relevant alert.</returns>
        public static IHtmlContent RenderAlert(this IHtmlHelper helper, ITempDataDictionary tempData)
        {
            //Since this will likely be ran on every page, do a quick check and if no keys are in tempData return right away
            if (tempData.Keys.Count == 0)
                return new HtmlString("");

            //Based on any possible values in TempData based on Key, get the class & text to use for display
            string divClass = "";
            string divText = "";
            if (tempData.ContainsKey("Notification")) //Isn't a bootstrap v4 alert type, but included for backwards compatiblity
            {
                divClass = "alert alert-info";
                divText = (tempData["Notification"] ?? "").ToString();
            }
            else if (tempData.ContainsKey("Error")) //Isn't a bootstrap v4 alert type, but included for backwards compatiblity
            {
                divClass = "alert alert-danger";
                divText = (tempData["Error"] ?? "").ToString();
            }
            else if (tempData.ContainsKey("Success"))
            {
                divClass = "alert alert-success";
                divText = (tempData["Success"] ?? "").ToString();
            }
            else if (tempData.ContainsKey("Info"))
            {
                divClass = "alert alert-info";
                divText = (tempData["Info"] ?? "").ToString();
            }
            else if (tempData.ContainsKey("Warning"))
            {
                divClass = "alert alert-warning";
                divText = (tempData["Warning"] ?? "").ToString();
            }
            else if (tempData.ContainsKey("Danger"))
            {
                divClass = "alert alert-danger";
                divText = (tempData["Danger"] ?? "").ToString();
            }

            //If text was generated, render an alert
            var outputHtml = new HtmlString("");
            if (!string.IsNullOrEmpty(divText))
            {
                outputHtml = new HtmlString
                (
                    @"<div class='" + divClass + "'>" + divText + @"</div>"
                );
            }
            return outputHtml;
        }
    }
}