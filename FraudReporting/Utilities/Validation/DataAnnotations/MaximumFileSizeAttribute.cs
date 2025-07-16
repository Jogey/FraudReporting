using System;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace FraudReporting.Utilities.Validation.DataAnnotations
{
    /// <summary>
    /// Check if the uploaded file is larger than MaximumBytes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class MaximumFileSizeAttribute : ValidationAttribute
    {
        private const string _defaultErrorMessage = "{0} must be greater than {2} bytes";

        /// <summary>
        /// Gets or sets the maximum allowed bytes for the file size. Cannot be negative.
        /// </summary>
        public int MaximumBytes { get; set; }

        /// <summary>
        /// Create a new instance of the FileSize attribute, which will check if the uploaded file is larger than MaximumBytes.
        /// </summary>
        /// <param name="maximumBytes">The maximum file size allowed, in bytes. Cannot be 0 or negative.</param>
        public MaximumFileSizeAttribute(int maximumBytes) : base(_defaultErrorMessage)
        {
            MaximumBytes = maximumBytes;
        }

        /// <summary>
        /// Applies formatting to an error message, based on the data field where the error occurred.
        /// </summary>
        /// <param name="name">The name to include in the formatted message.</param>
        /// <returns>An instance of the formatted error message.</returns>
        public override string FormatErrorMessage(string name)
        {
            return String.Format(ErrorMessage, name, MaximumBytes);
        }

        /// <summary>
        /// Check whether or not the passed object is valid.
        /// </summary>
        /// <param name="value">The passed object to validate.</param>
        /// <returns>True if valid, false if not.</returns>
        public override bool IsValid(object value)
        {
            if (MaximumBytes < 0)
                throw new ArgumentNullException("MaximumBytes must be 0 or greater");

            bool isValid = true;

            IFormFile file = value as IFormFile;
            if (file != null && file.Length > MaximumBytes)
                isValid = false;

            return isValid;
        }

        /// <summary>
        /// Validates the specified value with respect to the current validation attribute.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context information about the validation operation.</param>
        /// <returns>An instance of the System.ComponentModel.DataAnnotations.ValidationResult class.</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (IsValid(value))
                return ValidationResult.Success;
            else
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }
    }
}