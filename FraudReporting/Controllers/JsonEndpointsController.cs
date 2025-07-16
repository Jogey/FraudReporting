using System.Linq;
using FraudReporting.Filters;
using Microsoft.AspNetCore.Mvc;

namespace FraudReporting.Controllers
{
    [JsonEndpointsAuthorizationHeaderValid]
    [ApiController]
    [Route("[controller]/[action]")]
    public class JsonEndpointsController : ControllerBase
    {
        public IActionResult Dates()
        {
            var dates = ApfSettings.LiveDateRanges.Select(x => new { x.SiteSection, x.StartDate, x.EndDate }).ToList();
            return Ok(dates);
        }
    }
}