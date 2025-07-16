using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FraudReporting.Filters
{
    public class JsonEndpointsAuthorizationHeaderValidAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            var jsonEndpointsAuthorizationHeaderEncrypted = filterContext.HttpContext?.Request?.Headers["JSONEndpointsAuthorization"].ToString();
            if (!string.IsNullOrWhiteSpace(jsonEndpointsAuthorizationHeaderEncrypted))
            {
                var jsonEndpointsAuthorizationHeaderDecrypted = Convert.FromBase64String(jsonEndpointsAuthorizationHeaderEncrypted);
                if (jsonEndpointsAuthorizationHeaderDecrypted != null)
                {
                    var jsonEndpointsAuthorizationHeader = System.Text.Encoding.UTF8.GetString(jsonEndpointsAuthorizationHeaderDecrypted);
                    if (jsonEndpointsAuthorizationHeader == "371F1AB5DEC44AA7A51C4C43D201AD43")
                    {
                        // Valid
                        return;
                    }
                }
            }

            // Not valid
            filterContext.Result = new NotFoundResult();
            return;
        }
    }
}
