using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FraudReporting.Utilities.Validation
{
    /// <summary>
    /// Custom exception for catching user-facing errors (from form validation, etc.).
    /// The messages used here should be written in a user-friendly way, to be caught and displayed to users as a validation message, notification box, etc.
    /// </summary>
    public class UserException : Exception
    {
        public UserException()
        {
        }

        public UserException(string message)
            : base(message)
        {
        }

        public UserException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
