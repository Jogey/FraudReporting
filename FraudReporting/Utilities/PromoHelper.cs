using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Apfco;
using FraudReporting.Controllers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FraudReporting.Utilities
{
    /// <summary>
    /// Misc. helper functions to use throughout the project
    /// </summary>
    public static class PromoHelper
    {
        public static string GenerateId(string prefix)
        {
            return UTIL.StringLeft(prefix + UTIL.GetBase32ID(), 16);
        }

        public static string GenerateId(string prefix, int length)
        {
            return UTIL.StringLeft(prefix + UTIL.GetBase32ID(), length);
        }

        public static string GenerateStoredFileName(string originalFileName)
        {
            string numericTimestamp = DateTime.Now.ToString("yyyyMMdd_HHmmssf");
            string base32ID = UTIL.StringLeft(UTIL.GetBase32ID(), 8);
            string fileExtension = Path.GetExtension(originalFileName ?? "").ToLower();
            return string.Format("{0}_{1}{2}", numericTimestamp, base32ID, fileExtension);
        }

        public static string GetFileFormattedExtension(string input)
        {
            //If empty just return the input
            if (string.IsNullOrEmpty(input))
                return input;

            //Get the input extension, standardized to lowercase without the period
            return Path.GetExtension(input).Replace(".", "").ToLower().Trim();
        }

        public static string[] GetConstantClassValues(Type type)
        {
            return type.GetFields().Select(x => (string)x.GetValue(null)).OrderBy(x => x).ToArray();
        }

        public static string GetDeepestExceptionMessage(Exception exception, string currentExceptionMessage = "")
        {
            if (exception.Message.Contains("inner exception") && exception.InnerException != null)
                return GetDeepestExceptionMessage(exception.InnerException, exception.Message);
            else
            {
                if (!string.IsNullOrWhiteSpace(currentExceptionMessage))
                    return $"{exception.Message} - {currentExceptionMessage}";
                else
                    return exception.Message;
            }
        }
    }
}