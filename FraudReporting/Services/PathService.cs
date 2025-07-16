using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FraudReporting.Services
{
    public class PathService : IPathService
    {
        private IHttpContextAccessor httpContextAccessor;
        private IUrlHelper urlHelper;
        private IWebHostEnvironment webHostEnvironment;

        public PathService(IHttpContextAccessor httpContextAccessor, IUrlHelper urlHelper, IWebHostEnvironment webHostEnvironment)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.urlHelper = urlHelper;
            this.webHostEnvironment = webHostEnvironment;
        }

        public string GetBaseUrl()
        {
            var request = httpContextAccessor.HttpContext.Request;
            var location = new Uri($"{request.Scheme}://{request.Host}{request.PathBase}");
            return location.AbsoluteUri;
        }

        public string GetCurrentUrl()
        {
            var request = httpContextAccessor.HttpContext.Request;
            var location = new Uri($"{request.Scheme}://{request.Host}{request.PathBase}{request.Path}{request.QueryString}");
            return location.AbsoluteUri;
        }

        public string GetActionUrl(string action, string controller)
        {
            return GetActionUrl(action, controller, null);
        }

        public string GetActionUrl(string action, string controller, object values)
        {
            var request = httpContextAccessor.HttpContext.Request;
            return urlHelper.Action(action, controller, values, request.Scheme);
        }

        public string GetPhysicalPath(string inputRelativePath)
        {
            try
            {
                inputRelativePath = inputRelativePath.Replace(@"~", @""); //Remove the tilde if supplied
                inputRelativePath = inputRelativePath.Replace(@"\\", @"\"); //Fix any double slashes
                inputRelativePath = inputRelativePath.Replace(@"//", @"/"); //Fix any double slashes
                inputRelativePath = inputRelativePath.Replace(@"/", @"\"); //Fix backslashes for physical path instead of URL
                inputRelativePath = inputRelativePath.TrimStart('\\'); //If applicable, remove the leading backslash so Path.Combine() works properly
                var path = Path.Combine(webHostEnvironment.WebRootPath, inputRelativePath); //Finally combine the formatted relative path with the web root path
                return path;
            }
            catch
            {
                return ""; //Never crash the page because there's a missing value - just return an empty value 
            }
        }

        public string GetAbsoluteUrl(string inputRelativePath)
        {
            try
            {
                //First clean up the input
                inputRelativePath = inputRelativePath.Replace(@"~", @""); //Remove the tilde if supplied
                inputRelativePath = inputRelativePath.Replace(@"\", @"/"); //Fix backslashes for URLs instead of physical paths
                inputRelativePath = inputRelativePath.TrimStart('/'); //If applicable, remove the leading backslash so Path.Combine() works properly

                //Then combine the the input with the base url, and do any additional cleanup for issues caused by Path.Combine()
                var path = Path.Combine(GetBaseUrl(), inputRelativePath); //Finally combine the formatted relative path with the website base url
                path = path.Replace(@"\\", @"\").Replace(@"\", @"/"); //Fix any remaining incorrect slashes
                return path;
            }
            catch
            {
                return ""; //Never crash the page because there's a missing value - just return an empty value 
            }
        }
    }
}
