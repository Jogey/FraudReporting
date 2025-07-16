using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Apfco;
using Apfco.Web;
using Apfco.Web.Settings;
using FraudReporting.Models;
using FraudReporting.Services;
using FraudReporting.ViewModels.Notification;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FraudReporting.Controllers
{
    public class DebugController : BaseController<DebugController>
    {
        //---- Utilities/Services ----------------
        //----------------------------------------
        // (none)


        //---- Constructor -----------------------
        //----------------------------------------
        public DebugController() : base()
        {
        }


        //---- Action Mappings -------------------
        //----------------------------------------
#if (LOCAL || DEVELOPMENT || STAGING)
        //Put all of the actions here inside a preprocessor directive, to ensure the code here never accidentally ends up in the live environment
        public IActionResult TestError()
        {
            var innerException = new Exception("Test Inner Exception");
            var exception = new Exception("Test Exception; See inner exception for details.", innerException);
            throw exception;
        }


        //---- Helpers ---------------------------
        //----------------------------------------
        // (none)
#endif
    }
}
