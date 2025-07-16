using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FraudReporting.Services
{
    /// <summary>
    /// Service for rendering a view to a string. Can be used to render body content for emails, and is not dependant on a controller.
    /// </summary>
    public interface IViewRenderService
    {
        string RenderControllerAction(string action, string controller, object model);
        Task<string> RenderControllerActionAsync(string action, string controller, object model);
        string RenderToString(string viewName, object model);
        Task<string> RenderToStringAsync(string viewName, object model);
    }
}
