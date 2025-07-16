using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FraudReporting.Services
{
    public interface IPathService
    {
        string GetBaseUrl();
        string GetCurrentUrl();
        string GetActionUrl(string action, string controller);
        string GetActionUrl(string action, string controller, object values);
        string GetAbsoluteUrl(string inputRelativePath);
        string GetPhysicalPath(string inputRelativePath);
    }
}
