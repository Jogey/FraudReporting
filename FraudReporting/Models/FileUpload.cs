using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace FraudReporting.Models
{
    public class FileUpload
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public long EntryId { get; set; }

        [ForeignKey(nameof(EntryId))]
        public virtual Entry Entry { get; set; }

        [MaxLength(10)]
        public string Type { get; set; }

        [MaxLength(100)]
        public string FileName { get; set; }

        [MaxLength(100)]
        public string RelativeFilePath { get; set; }

        [NotMapped]
        [MaxLength(100)]
        public string PhysicalFilePath { get; set; }

        [NotMapped]
        [MaxLength(100)]
        public string DisplayName { get; set; }

        [NotMapped]
        public string UniqueId { get; set; }

        public FileUpload()
        {
            UniqueId = Guid.NewGuid().ToString();
        }
    }

    public class FileUploadData
    {
        [Display(Name = "Upload Spreadsheet")]
        [DataType(DataType.Upload)]
        [Utilities.Validation.DataAnnotations.ValidFileExtensions(".csv, .xls, .xlsx", ErrorMessage = "The file must be in either CSV, XLS, or XLSX format.")]
        [Utilities.Validation.DataAnnotations.ValidFileMimeTypes("text/csv, application/vnd.ms-excel, application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", ErrorMessage = "The file must be in either CSV, XLS, or XLSX format.")]
        [Utilities.Validation.DataAnnotations.MinimumFileSize(1, ErrorMessage = "The file is empty. Please submit a new file.")]
        public IFormFile File { get; set; }

        public string Type { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //Check that the image format is valid and it's the correct size
            if (File == null)
                yield return new ValidationResult("The file is missing/invalid.", new[] { nameof(File) });
        }
    }
}
