using System;
using System.Collections.Generic;
using System.Linq;
using FraudReporting.Controllers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FraudReporting.Utilities
{
    /// <summary>
    /// Misc. utilities for handling MVC Views, such as displaying data & generating HTML/CSS values.
    /// </summary>
    public static class ViewHelper
    {
        public static string GetCssClassBody(ViewContext viewContext)
        {
            return GetCssClassBody(viewContext.RouteData.Values["action"].ToString(), viewContext.RouteData.Values["controller"].ToString());
        }

        public static string GetCssClassBody(string actionName, string controllerName)
        {
            //Per programmer meeting 2020-09-25, use the controller & action as separate classes, prefixed so that common controller/action names don't collide with existing styles
            return string.Format("c{0} a{1}", BaseController<Object>.GetControllerNameOf(controllerName), actionName);
        }

        public static string GetCssClassViewWrapper()
        {
            return "viewWrapper";
        }


        public static string DisplayDate(DateTime input)
        {
            DateTime? adjInput = input;
            return DisplayDate(adjInput);
        }

        public static string DisplayDate(DateTime? input)
        {
            return DisplayDatetime(input, "yyyy-MM-dd");
        }

        public static string DisplayDate(DateTime? input, string defaultValue)
        {
            return DisplayDatetime(input, "yyyy-MM-dd", defaultValue);
        }

        public static string DisplayDatetime(DateTime input)
        {
            DateTime? adjInput = input;
            return DisplayDatetime(adjInput);
        }

        public static string DisplayDatetime(DateTime? input)
        {
            return DisplayDatetime(input, "yyyy-MM-dd hh:mm:ss tt");
        }

        public static string DisplayDatetime(DateTime? input, string format)
        {
            return DisplayDatetime(input, format, "");
        }

        public static string DisplayDatetime(DateTime? input, string format, string defaultValue)
        {
            if (input == null)
                return defaultValue;

            return input.HasValue ? input.Value.ToString(format) : defaultValue;
        }


        public static List<SelectListItem> GetSelectListNumbers(int numStart, int numEnd, bool includeDefaultOption = false, string defaultOption = "---")
        {
            List<SelectListItem> outputList = new List<SelectListItem>();

            if (includeDefaultOption)
                outputList.Add(new SelectListItem { Text = defaultOption, Value = "" });

            for (int i = numStart; i <= numEnd; i++)
                outputList.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });

            return outputList;
        }

        public static List<SelectListItem> GetAsSelectList(List<int> inputList, bool includeDefaultOption = false, string defaultOption = "---")
        {
            List<SelectListItem> outputList = new List<SelectListItem>();

            if (includeDefaultOption)
                outputList.Add(new SelectListItem { Text = defaultOption, Value = "" });

            foreach (var i in inputList.OrderBy(x => x))
                outputList.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
            return outputList;
        }

        public static List<SelectListItem> GetAsSelectList(Dictionary<string, string> inputList, bool includeDefaultOption = false, string defaultOption = "---")
        {
            List<SelectListItem> outputList = new List<SelectListItem>();

            if (includeDefaultOption)
                outputList.Add(new SelectListItem { Text = defaultOption, Value = "" });

            foreach (var i in inputList.OrderBy(x => x.Value))
                outputList.Add(new SelectListItem { Text = i.Value.ToString(), Value = i.Key.ToString() });
            return outputList;
        }

        public static List<SelectListItem> GetAsSelectList(string[] inputList, bool includeDefaultOption = false, string defaultOption = "---")
        {
            List<SelectListItem> outputList = new List<SelectListItem>();

            if (includeDefaultOption)
                outputList.Add(new SelectListItem { Text = defaultOption, Value = "" });

            foreach (var i in inputList.OrderBy(x => x))
                outputList.Add(new SelectListItem { Text = i, Value = i });
            return outputList;
        }

        public static List<SelectListItem> GetAsSelectList(List<string> inputList, bool includeDefaultOption = false, string defaultOption = "---")
        {
            List<SelectListItem> outputList = new List<SelectListItem>();

            if (includeDefaultOption)
                outputList.Add(new SelectListItem { Text = defaultOption, Value = "" });

            foreach (var i in inputList.OrderBy(x => x))
                outputList.Add(new SelectListItem { Text = i, Value = i });
            return outputList;
        }

    }
}
