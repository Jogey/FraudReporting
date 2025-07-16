using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Apfco;
using Apfco.Email.API;
using Apfco.Web;
using Apfco.Web.Settings;
using FraudReporting.Models;
using FraudReporting.Services;
using FraudReporting.Utilities;
using FraudReporting.ViewModels.Notification;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting.Internal;

namespace FraudReporting.Controllers
{
    //Use the following order when applying 'IAuthorizationFilter' implementing attributes:
    //[CookiesRequired("Notification", "CookiesRequired", "TestCookiesEnabled", Order = 10)]
    //[Authorize(Order = 20)]
    //[ApfValidateAntiForgeryToken(Order = 30)]

    public class NotificationController : BaseController<NotificationController>
    {
        //---- Utilities/Services ----------------
        //----------------------------------------
        private IHttpContextAccessor httpContextAccessor;
        private IPathService pathService;
        private IWebHostEnvironment webHostEnvironment;
        private IConfiguration configuration;
        private IViewRenderService viewRenderService;
        private PasswordManagerService passwordManagerService;


        //---- Constructor -----------------------
        //----------------------------------------
        public NotificationController(IViewRenderService viewRenderService, IConfiguration configuration, PasswordManagerService passwordManagerService, IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor, IPathService pathService) : base()
        {
            this.httpContextAccessor = httpContextAccessor;
            this.pathService = pathService;
            this.webHostEnvironment = webHostEnvironment;
            this.passwordManagerService = passwordManagerService;
            this.configuration = configuration;
            this.viewRenderService = viewRenderService;
        }


        //---- Action Mappings -------------------
        //----------------------------------------
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            //Create the viewModel with data to display to the user
            var viewModel = new ErrorViewModel();
            int customerServiceEmailsCount = ApfSettings.Website.CustomerServiceEmailList.Count;
            viewModel.CustomerServiceEmails = string.Join(" ", ApfSettings.Website.CustomerServiceEmailList.Select((x, i) => x.EmailAddress + (i < customerServiceEmailsCount - 2 ? ", " : (i < customerServiceEmailsCount - 1 ? (customerServiceEmailsCount == 2 ? "" : ",") + " or" : ""))));
            viewModel.Promo = ApfSettings.Website.Promo;
            viewModel.IPAddress = httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress.ToString();
            viewModel.SessionId = httpContextAccessor.HttpContext?.Session?.Id;

            return View(viewModel);
        }

        public IActionResult ComingSoon()
        {
            return View();
        }

        public IActionResult ProgramEnded()
        {
            return View();
        }

        public IActionResult UnderConstruction()
        {
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult InvalidRequest()
        {
            return View();
        }   


        //---- Helpers ---------------------------
        //----------------------------------------
        // (none)
    }
}
