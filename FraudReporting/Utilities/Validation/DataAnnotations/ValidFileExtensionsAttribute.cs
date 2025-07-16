using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace FraudReporting.Utilities.Validation.DataAnnotations
{
    /// <summary>
    /// Check if the uploaded file has a valid file extension.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ValidFileExtensionsAttribute : ValidationAttribute
    {
        private const string _defaultErrorMessage = "{0} must be one of the following formats: {1}";

        /// <summary>
        /// Gets an array of the valid extensions.
        /// </summary>
        public string[] ValidExtensions { get; private set; }

        /// <summary>
        /// Create a new instance of the ValidFileExtension attribute, which will check if the uploaded file has a valid file extension.
        /// </summary>
        /// <param name="validExtensions">A comma seperated list of valid extensions with a period before each. Spaces are allowed after the commas to make them more readable.</param>
        public ValidFileExtensionsAttribute(string validExtensions) : base(_defaultErrorMessage)
        {
            ValidExtensions = validExtensions.Split(',').Select(x => x.Trim()).ToArray();
        }

        /// <summary>
        /// Applies formatting to an error message, based on the data field where the error occurred.
        /// </summary>
        /// <param name="name">The name to include in the formatted message.</param>
        /// <returns>An instance of the formatted error message.</returns>
        public override string FormatErrorMessage(string name)
        {
            return String.Format(ErrorMessage, name, string.Join(", ", ValidExtensions));
        }

        /// <summary>
        /// Check whether or not the passed object is valid.
        /// </summary>
        /// <param name="value">The passed object to validate.</param>
        /// <returns>True if valid, false if not.</returns>
        public override bool IsValid(object value)
        {
            if (ValidExtensions == null || ValidExtensions.Length == 0)
                throw new ArgumentNullException("ValidExtensions must contain at least one extension");

            bool isValid = false;

            IFormFile file = value as IFormFile;

            // Make sure the file exists and that the file name is not empty
            if (file != null && !string.IsNullOrWhiteSpace(file.FileName))
            {
                bool validFileTypeFound = false;

                // Get the current extension
                var currentExtension = "." + file.FileName.Split('.').LastOrDefault();

                // Extension must be found
                if (!string.IsNullOrWhiteSpace(currentExtension))
                {
                    foreach (var validExtension in ValidExtensions)
                    {
                        if (currentExtension.ToUpper() == validExtension.ToUpper())
                        {
                            validFileTypeFound = true;
                            break;
                        }
                    }
                }

                isValid = validFileTypeFound;
            }
            else
            {
                // If the file is not found or the file name is empty, just allow it through
                // If you don't want this to happen, just add [Required] to the property and it will take care of the rest
                isValid = true;
            }

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