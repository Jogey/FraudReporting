using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FraudReporting.Controllers;
using FraudReporting.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace FraudReporting.Services
{
    /// <summary>
    /// Service for accessing data as strongly typed. This is a remake of the old SessionHelper class. Note that in comparsion to the Session handling 
    /// in the previous .Net Framework, the only way to store complex objects in the .Net Core Session is converting to/from JSON.
    /// </summary>
    public class SessionService
    {
        private IHttpContextAccessor httpContextAccessor;

        public SessionService(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Save a simple value to the user's session to preserve their Session ID between requests.
        /// 
        /// Sessions in ASP.Net Core 3.1 require that a value be currently stored in session to keep the same SessionId. If this is
        /// not done, the SessionId would change for every request. Put a call to this in the main layout of the site to ensure
        /// this value gets plugged.
        /// </summary>
        public void SaveSessionId()
        {
            SessionSet<Entry>(nameof(SaveSessionId), true);
        }

        public T SessionGet<T>(string id)
        {
            return SessionGet<T>(id, null);
        }

        private T SessionGet<T>(string id, object defaultValue)
        {
            var data = httpContextAccessor.HttpContext.Session.GetString(id);
            if (string.IsNullOrWhiteSpace(data) || data == "null") //This may actually return the string "null" in some cases - need to check for that as well
            {
                httpContextAccessor.HttpContext.Session.SetString(id, JsonConvert.SerializeObject(defaultValue));
                data = httpContextAccessor.HttpContext.Session.GetString(id);
            }
            return JsonConvert.DeserializeObject<T>(data);
        }

        private void SessionSet<T>(string id, object value)
        {
            httpContextAccessor.HttpContext.Session.SetString(id, JsonConvert.SerializeObject(value));
        }


        //User session variables

        public SessionData sessionData
        {
            get
            {
                return SessionGet<SessionData>(nameof(sessionData), new SessionData());
            }
            set
            {
                SessionSet<SessionData>(nameof(sessionData), value);
            }
        }
    }
}
