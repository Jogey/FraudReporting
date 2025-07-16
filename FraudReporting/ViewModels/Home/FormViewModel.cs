using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;
using Apfco.Validation;
using Apfco.Validation.DataAnnotations;
using Apfco.Web.Validation.DataAnnotations;
using FraudReporting;
using FraudReporting.Filters;
using FraudReporting.Models;
using FraudReporting.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;

namespace FraudReporting.ViewModels.Home
{
    public class FormViewModel : IValidatableObject
    {
        [Required]
        [Display(Name = "Scope")]
        public string ValidationScope { get; set; }

        [Required]
        [Display(Name = "Confidence")]
        [Range(0, 1, ErrorMessage = "Confidence must be between 0 and 1.")]
        public decimal ConfidenceThreshold { get; set; }

        [Required]
        [Display(Name = "Dupe Limit")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Dupe limit must be greater than 1.")]
        public int CountThreshold { get; set; }

        public bool DownloadOuput { get; set; }

        public bool UseParallelThreading { get; set; }

        [Range(0, 1, ErrorMessage = "Scoring must be between 0 and 1.")]
        public decimal FraudScoringIPAddress { get; set; }

        [Range(0, 1, ErrorMessage = "Scoring must be between 0 and 1.")]
        public decimal FraudScoringAddress { get; set; }

        [Range(0, 1, ErrorMessage = "Scoring must be between 0 and 1.")]
        public decimal FraudScoringZip4 { get; set; }

        [Range(0, 1, ErrorMessage = "Scoring must be between 0 and 1.")]
        public decimal FraudScoringEmail { get; set; }

        [Range(0, 1, ErrorMessage = "Scoring must be between 0 and 1.")]
        public decimal FraudScoringEmailProvider { get; set; }

        [Range(0, 1, ErrorMessage = "Scoring must be between 0 and 1.")]
        public decimal FraudScoringPhoneNumber { get; set; }

        public List<SelectListItem> GetValidationScopes()
        {
            return new List<SelectListItem>
            {
                new SelectListItem() { Text = "", Value = "", Selected = true, Disabled = true },
                new SelectListItem() { Text = "All Records", Value = nameof(FraudReportingService.Scopes.ALL) },
                new SelectListItem() { Text = "By Entry Datetime", Value = nameof(FraudReportingService.Scopes.DATETIME) },
                new SelectListItem() { Text = "By Batch", Value = nameof(FraudReportingService.Scopes.BATCH) }
            };
        }

        [Display(Name = "Upload Records Spreadsheet")]
        public IFormFile FileUpload { get; set; }

        public FormViewModel()
        { }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //Get services
            var configuration = (IConfiguration)validationContext.GetService(typeof(IConfiguration));
            var httpContextAccessor = (IHttpContextAccessor)validationContext.GetService(typeof(IHttpContextAccessor));
            var sessionService = (SessionService)validationContext.GetService(typeof(SessionService));
            var pathService = (IPathService)validationContext.GetService(typeof(IPathService));

            //Validate that a user has uploaded a file if one hasn't already been provided
            if (sessionService.sessionData == null || sessionService.sessionData.FileUploads.Count() == 0)
                yield return new ValidationResult("Please upload a file to continue.", new[] { nameof(FileUpload) });
        }
    }
}