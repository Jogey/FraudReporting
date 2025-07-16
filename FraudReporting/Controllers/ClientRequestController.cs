using FraudReporting.Models;
using FraudReporting.Services;
using FraudReporting.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

namespace FraudReporting.Controllers
{
    public class ClientRequestController : BaseController<ClientRequestController>
    {
        //---- Utilities/Services ----------------
        //----------------------------------------
        private SessionService sessionService;
        private IPathService pathService;


        //---- Constructor -----------------------
        //----------------------------------------
        public ClientRequestController(SessionService sessionService, IPathService pathService) : base()
        {
            this.sessionService = sessionService;
            this.pathService = pathService;
        }

        //---- Helpers ---------------------------
        //----------------------------------------
        [HttpPost]
        public ActionResult SaveFileUpload(FileUploadData data)
        {
            if (data == null || data.File == null)
                return new ObjectResult("No file was found. Please upload a file.") { StatusCode = 403 };

            try
            {
                //Parse the file upload
                var file = data.File;
                var fileName = file.FileName;
                var contentType = file.ContentType;
                var contentLength = file.Length;

                //Validate file mime type
                var mimeTypes = new List<string>() { "text/csv", "application/vnd.ms-excel", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" };
                if (!mimeTypes.Contains(contentType))
                    return new ObjectResult("Invalid file type. File type must be .csv, .xls, or .xlsx.") { StatusCode = 406 };

                //Save the file upload to wwwroot
                var entryFile = new FileUpload();
                entryFile.Type = data.Type;
                entryFile.DisplayName = fileName;
                entryFile.FileName = PromoHelper.GenerateStoredFileName(fileName);
                entryFile.RelativeFilePath = SessionData.FileUploadRelativePath;
                entryFile.PhysicalFilePath = pathService.GetPhysicalPath($"{entryFile.RelativeFilePath}{entryFile.FileName}");
                using (FileStream stream = new FileStream(entryFile.PhysicalFilePath, FileMode.Create))
                    file.CopyTo(stream);

                //Update the pending entry
                var entry = (sessionService.sessionData == null) ? new SessionData() : sessionService.sessionData;
                entry.FileUploads.Add(entryFile);
                sessionService.sessionData = entry;

                return new JsonResult(entryFile.UniqueId);
            }
            catch
            {
                return new ObjectResult("An unexpected failure occurred when trying to upload the file.") { StatusCode = 400 };
            }
        }

        [HttpPost]
        public ActionResult DeleteFileUpload(string id)
        {
            var entry = sessionService.sessionData;
            if (entry == null)
                return StatusCode(403);

            try
            {
                //Delete the previously uploaded file if it exists
                var file = sessionService.sessionData.FileUploads.FirstOrDefault(x => x.UniqueId == id);
                if (file == default) throw new Exception("Object not found in list");

                if (System.IO.File.Exists(file.PhysicalFilePath))
                    System.IO.File.Delete(file.PhysicalFilePath);

                //Update the pending entry
                entry.FileUploads.RemoveAll(x => x.UniqueId == id);
                sessionService.sessionData = entry;

                return StatusCode(200);
            }
            catch
            {
                return StatusCode(400);
            }
        }
    }
}
