using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FraudReporting.Filters;
using FraudReporting.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FraudReporting.Controllers
{
    [AutoValidateAntiforgeryToken]
    [TypeFilter(typeof(RedirectAntiforgeryValidationFailedResultFilter))]
    public abstract class BaseController<T> : Controller
    {
        //Properties
        /// <summary>
        /// Static version of nameof() that strips off the "Controller" portion. Ex. HomeController.NameOf = "Home". Uses the T-template
        /// when implementing to do this.
        /// </summary>
        public static string NameOf
        {
            get
            {
                var controllerType = typeof(T);
                return GetControllerNameOf(controllerType.Name);
            }
        }


        //Methods
        public BaseController()
        {
        }

        public static string GetControllerNameOf(string fullControllerName)
        {
            return fullControllerName.Replace("Controller", "");
        }

    }
}
