using FraudReporting.Controllers;
using FraudReporting.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FraudReporting.Filters
{
    /// <summary>
    /// This filter will help redirect to the NotificationController InvalidRequest when an IAntiforgeryValidationFailedResult result is encountered. This result is returned when the ValidateAntiForgeryToken filter errors out.
    /// </summary>
    public class RedirectAntiforgeryValidationFailedResultFilter : IAlwaysRunResultFilter
    {
        public RedirectAntiforgeryValidationFailedResultFilter()
        {
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is IAntiforgeryValidationFailedResult result)
            {
                context.Result = new RedirectToActionResult(nameof(NotificationController.InvalidRequest), NotificationController.NameOf, null);
            }
        }

        public void OnResultExecuted(ResultExecutedContext context)
        { }
    }
}