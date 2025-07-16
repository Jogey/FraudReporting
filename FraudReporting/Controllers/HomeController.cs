using AutoMapper;
using DocumentFormat.OpenXml.Vml.Office;
using FraudReporting.Filters;
using FraudReporting.Models;
using FraudReporting.Services;
using FraudReporting.Utilities;
using FraudReporting.ViewModels.Home;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace FraudReporting.Controllers
{
    [LiveDateRangeCheck]
    public class HomeController : BaseController<HomeController>
    {
        //---- Utilities/Services ----------------
        //----------------------------------------
        private IHttpContextAccessor httpContextAccessor;
        private ApfVariablesService apfVariablesService;
        private PasswordManagerService passwordManagerService;
        private SessionService sessionService;
        private IPathService pathService;
        private IMapper mapper;
        private IConfiguration configuration;
        private ComponentsValidationService componentsValidationService;
        private FileIOService fileIOService;
        private FraudReportingService fraudReportingService;
        private IViewRenderService viewRenderService;


        //---- Constructor -----------------------
        //----------------------------------------
        public HomeController(IViewRenderService viewRenderService, IHttpContextAccessor httpContextAccessor, ApfVariablesService apfVariablesService, PasswordManagerService passwordManagerService, SessionService sessionService, IPathService pathService, IMapper mapper, IConfiguration configuration, ComponentsValidationService componentsValidationService, FileIOService fileIOService, FraudReportingService fraudReportingService) : base()
        {
            this.httpContextAccessor = httpContextAccessor;
            this.apfVariablesService = apfVariablesService;
            this.passwordManagerService = passwordManagerService;
            this.sessionService = sessionService;
            this.pathService = pathService;
            this.mapper = mapper;
            this.configuration = configuration;
            this.componentsValidationService = componentsValidationService;
            this.fileIOService = fileIOService;
            this.fraudReportingService = fraudReportingService;
            this.viewRenderService = viewRenderService;
        }


        //---- Action Mappings -------------------
        //----------------------------------------
        public IActionResult Index()
        {
            var viewModel = new FormViewModel();
            viewModel.ConfidenceThreshold = 0.75m;
            viewModel.CountThreshold = 1;
            viewModel.FraudScoringIPAddress = 0.5m;
            viewModel.FraudScoringAddress = 0.75m;
            viewModel.FraudScoringZip4 = 0.3m;
            viewModel.FraudScoringEmail = 0.3m;
            viewModel.FraudScoringEmailProvider = 0.2m;
            viewModel.FraudScoringPhoneNumber = 0.2m;

            return View(viewModel);
        }
           
        [HttpPost]
        public IActionResult Index(FormViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            Context context = new Context();
            try
            {
                var files = sessionService.sessionData.FileUploads;
                context = fileIOService.CreateContextFromFileData(files, viewModel.ValidationScope);
            }
            catch (Exception ex)
            {
                string message = "Something went wrong while attempting to upload the file. Please contact IT support.";

                if (ex.Message == nameof(FileIOService.Results.InvalidFormat))
                    message = "The header data sets found in the uploaded file is missing or is not valid.";

                if (ex.Message == nameof(FileIOService.Results.BadData))
                    message = "The uploaded file could not be processed due to bad/corrupted data.";

                if (ex.Message == nameof(FileIOService.Results.MissingData))
                    message = "The uploaded file contains no data entries.";

                ModelState.AddModelError(nameof(FormViewModel.FileUpload), message);
                return View(viewModel);
            }

            var payload = fraudReportingService.CreatePayload(viewModel.FraudScoringIPAddress, viewModel.FraudScoringAddress, viewModel.FraudScoringZip4, viewModel.FraudScoringEmail, viewModel.FraudScoringEmailProvider, viewModel.FraudScoringPhoneNumber);
            var output = fraudReportingService.GetFraudReport(context, viewModel.ValidationScope, (float)viewModel.ConfidenceThreshold, viewModel.CountThreshold, payload, viewModel.DownloadOuput, viewModel.UseParallelThreading);
            var id = fileIOService.WriteReport(DateTime.Now.ToString(), output.Report, output.RecordsList);

            if (string.IsNullOrEmpty(id))
            {
                ModelState.AddModelError(nameof(FormViewModel.FileUpload), "Something went wrong while attempting to upload the file. Please contact IT support.");
                return View(viewModel);
            }

            return RedirectToAction(nameof(Result), new { id = id });
        }

        public IActionResult Result(string id)
        {
            var viewModel = new ResultViewModel();

            var report = fileIOService.ReadReport(fileIOService.GetReportPhysicalPath(id, ".txt"));
            if (report == null) return RedirectToAction(nameof(Index));

            viewModel.Report = report;

            return View(viewModel);
        }


        //---- Helpers ---------------------------
        //----------------------------------------
        //(none)
    }
}
