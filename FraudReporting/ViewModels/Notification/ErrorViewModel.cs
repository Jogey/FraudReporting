using System;

namespace FraudReporting.ViewModels.Notification
{
    public class ErrorViewModel
    {
        public string CustomerServiceEmails { get; set; }
        public string Promo { get; set; }
        public string IPAddress { get; set; }
        public string SessionId { get; set; }
    }
}
