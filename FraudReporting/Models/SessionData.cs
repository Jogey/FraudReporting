using DocumentFormat.OpenXml.Spreadsheet;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FraudReporting.Models
{
    public class SessionData
    {
        //Static Read-only Values
        [NotMapped]
        public static readonly string FileUploadRelativePath = "~/Uploads/";

        [NotMapped]
        public static readonly string ReportUploadRelativePath = "~/Documents/";


        //Properties
        public string ValidationScope { get; set; }

        public float ConfidenceThreshold { get; set; }

        public int CountThreshold { get; set; }

        public bool DownloadOutput { get; set; }

        public List<FileUpload> FileUploads { get; set; }


        //Constructor
        public SessionData()
        {
            FileUploads = new List<FileUpload>();
        }
    }
}
