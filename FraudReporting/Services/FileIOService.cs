using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using FraudReporting.Models;
using FraudReporting.Utilities;
using FraudReporting.ViewModels.Home;
using Microsoft.AspNetCore.Components.Forms;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FraudReporting.Services
{
    public class FileIOService
    {
        //Properties
        public enum Results
        {
            Success,
            MissingData,
            InvalidFormat,
            BadData
        }

        private IPathService pathService;
        private FraudReportingService fraudReportingService;


        //Main methods
        public FileIOService(IPathService pathService, FraudReportingService fraudReportingService)
        {
            this.pathService = pathService;
            this.fraudReportingService = fraudReportingService;
        }

        public Context CreateContextFromFileData(List<FileUpload> files, string scope = "ALL")
        {
            Context output = new Context();

            bool isValidFormat = true;
            bool dataListSuccessful = true;

            foreach (var file in files)
            {
                using (FileStream fileStream = System.IO.File.Open(file.PhysicalFilePath, FileMode.Open))
                {
                    //Validate the activity header row (first row) from a given stream and confirms whether its layout/format is valid.
                    try
                    {
                        using (var workbook = new XLWorkbook(fileStream))
                        {
                            //Instantiate the worksheet and extract the activity header row.
                            var worksheet = workbook.Worksheet(1);
                            var activityHeaderRow = worksheet.Row(1);

                            //Validate the data set header.
                            isValidFormat =
                                activityHeaderRow.Cell(1).Value.ToString().ToUpper().Trim() == "ID" &&
                                activityHeaderRow.Cell(2).Value.ToString().ToUpper().Trim() == "DATETIME" &&
                                activityHeaderRow.Cell(3).Value.ToString().ToUpper().Trim() == "IPADDRESS" &&
                                activityHeaderRow.Cell(4).Value.ToString().ToUpper().Trim() == "NAME" &&
                                activityHeaderRow.Cell(5).Value.ToString().ToUpper().Trim() == "EMAIL" &&
                                activityHeaderRow.Cell(6).Value.ToString().ToUpper().Trim() == "PHONE" &&
                                activityHeaderRow.Cell(7).Value.ToString().ToUpper().Trim() == "ADDRESS" &&
                                activityHeaderRow.Cell(8).Value.ToString().ToUpper().Trim() == "CITY" &&
                                activityHeaderRow.Cell(9).Value.ToString().ToUpper().Trim() == "STATE" &&
                                activityHeaderRow.Cell(10).Value.ToString().ToUpper().Trim() == "ZIP" &&
                                activityHeaderRow.Cell(11).Value.ToString().ToUpper().Trim() == "ZIP4" &&
                                activityHeaderRow.Cell(12).Value.ToString().ToUpper().Trim() == "COUNTRY" &&
                                activityHeaderRow.Cell(13).Value.ToString().ToUpper().Trim() == "BATCH2";
                        }
                    }
                    catch (Exception ex)
                    {
                        isValidFormat = false;
                    }
                    //If header is valid, construct the spreadsheet into a dynamic list of <Entry> objects.
                    if (isValidFormat)
                    {
                        try
                        {
                            using (var workbook = new XLWorkbook(fileStream))
                            {
                                //Instantiate the worksheet and extract the data rows.
                                var worksheet = workbook.Worksheet(1);
                                ConcurrentBag<Entry> entries = new ConcurrentBag<Entry>();
                                var dataRows = worksheet.RangeUsed().RowsUsed().Skip(1);

                                //Verify that there is valid data in the body.
                                if (dataRows.Count() <= 0)
                                {
                                    throw new Exception("No body data found");
                                }

                                Parallel.ForEach(dataRows, row =>
                                {
                                    Entry entry = new Entry()
                                    {
                                        Id = row.Cell(1).Value.ToString().ToUpper().Trim(),
                                        DateTime = DateTime.Parse(row.Cell(2).Value.ToString().ToUpper().Trim()),
                                        IPAddress = row.Cell(3).Value.ToString().ToUpper().Trim(),
                                        Name = row.Cell(4).Value.ToString().ToUpper().Trim(),
                                        Email = row.Cell(5).Value.ToString().ToUpper().Trim(),
                                        Phone = row.Cell(6).Value.ToString().ToUpper().Trim(),
                                        Address = row.Cell(7).Value.ToString().ToUpper().Trim(),
                                        City = row.Cell(8).Value.ToString().ToUpper().Trim(),
                                        State = row.Cell(9).Value.ToString().ToUpper().Trim(),
                                        Zip = row.Cell(10).Value.ToString().ToUpper().Trim(),
                                        Zip4 = row.Cell(11).Value.ToString().ToUpper().Trim(),
                                        Country = row.Cell(12).Value.ToString().ToUpper().Trim(),
                                        Batch2 = row.Cell(13).Value.ToString().ToUpper().Trim()
                                    };
                                    entry.Scope = fraudReportingService.SetEntryScope(entry, scope);
                                    entries.Add(entry);
                                });
                                output.Entries = entries.ToList();
                            }
                        }
                        catch (Exception ex)
                        {
                            dataListSuccessful = false;
                        }
                    }
                }

                if (!isValidFormat)
                    throw new Exception(nameof(Results.InvalidFormat));

                if (!dataListSuccessful)
                    throw new Exception(nameof(Results.BadData));

                if (output.Entries.Count() <= 0)
                    throw new Exception(nameof(Results.MissingData));
            }

            return output;
        }

        public string GetReportPhysicalPath(string filename, string format = "")
        {
            if (string.IsNullOrEmpty(filename))
                return string.Empty;

            return pathService.GetPhysicalPath($"{SessionData.ReportUploadRelativePath}{filename}" + format);
        }

        public string WriteReport(string name, Report report, Output recordsList)
        {
            string output = string.Empty;

            //If records list exists, save first and assign filename to the report
            string recordsListFilename = string.Empty;
            if (recordsList != null && recordsList.Items.Count > 0)
            {
                try
                {
                    var fileBytes = GetRecordsListByteData(recordsList);
                    recordsListFilename = PromoHelper.GenerateStoredFileName(name);
                    var path = GetReportPhysicalPath(recordsListFilename, ".xlsx");
                    File.WriteAllBytes(path, fileBytes);
                    report.RecordsListFilename = recordsListFilename;
                }
                catch (Exception ex)
                { }
            }

            //Then, save the report
            string content = JsonConvert.SerializeObject(report);
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(content))
                return string.Empty;

            try
            {
                var filename = PromoHelper.GenerateStoredFileName(name);
                var path = GetReportPhysicalPath(filename, ".txt");
                File.WriteAllText(path, content);
                output = filename;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }

            return output;
        }

        public Report ReadReport(string path)
        {
            Report output = null;

            if (string.IsNullOrEmpty(path))
                return null;

            try
            {
                const Int32 BufferSize = 128;
                using (var fileStream = File.OpenRead(path))
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
                {
                    string content = streamReader.ReadToEnd();
                    output = JsonConvert.DeserializeObject<Report>(content);
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return output;
        }

        private byte[] GetRecordsListByteData(Output recordsList)
        {
            byte[] output = null;

            using (var workbook = new XLWorkbook())
            {
                // Create a new worksheet.
                var worksheet = workbook.Worksheets.Add("File");

                // Build the activity header row.
                int headerRow = 1;
                worksheet.Cell(headerRow, 1).Value = "Id";
                worksheet.Cell(headerRow, 2).Value = "DateTime";
                worksheet.Cell(headerRow, 3).Value = "IPAddress";
                worksheet.Cell(headerRow, 4).Value = "Name";
                worksheet.Cell(headerRow, 5).Value = "Email";
                worksheet.Cell(headerRow, 6).Value = "Phone";
                worksheet.Cell(headerRow, 7).Value = "Address";
                worksheet.Cell(headerRow, 8).Value = "City";
                worksheet.Cell(headerRow, 9).Value = "State";
                worksheet.Cell(headerRow, 10).Value = "Zip";
                worksheet.Cell(headerRow, 11).Value = "Zip4";
                worksheet.Cell(headerRow, 12).Value = "Country";
                worksheet.Cell(headerRow, 13).Value = "Batch2";
                worksheet.Cell(headerRow, 14).Value = "FraudScore";
                worksheet.Cell(headerRow, 15).Value = "IsSuspectIPAddress";
                worksheet.Cell(headerRow, 16).Value = "IsSuspectAddress";
                worksheet.Cell(headerRow, 17).Value = "IsSuspectZip4";
                worksheet.Cell(headerRow, 18).Value = "IsSuspectEmail";
                worksheet.Cell(headerRow, 19).Value = "IsSuspectEmailProvider";
                worksheet.Cell(headerRow, 20).Value = "IsSuspectPhoneNumber";
                worksheet.Cell(headerRow, 21).Value = "CorrelationFraud";

                // Build each data row from the input list of FileUploadData objects.
                int i = headerRow + 1;
                foreach (var data in recordsList.Items)
                {
                    worksheet.Cell(i, 1).Value = data.Entry.Id;
                    worksheet.Cell(i, 2).Value = data.Entry.DateTime;
                    worksheet.Cell(i, 3).Value = data.Entry.IPAddress;
                    worksheet.Cell(i, 4).Value = data.Entry.Name;
                    worksheet.Cell(i, 5).Value = data.Entry.Email;
                    worksheet.Cell(i, 6).Value = data.Entry.Phone;
                    worksheet.Cell(i, 7).Value = data.Entry.Address;
                    worksheet.Cell(i, 8).Value = data.Entry.City;
                    worksheet.Cell(i, 9).Value = data.Entry.State;
                    worksheet.Cell(i, 10).Value = data.Entry.Zip;
                    worksheet.Cell(i, 11).Value = data.Entry.Zip4;
                    worksheet.Cell(i, 12).Value = data.Entry.Country;
                    worksheet.Cell(i, 13).Value = data.Entry.Batch2;
                    worksheet.Cell(i, 14).Value = data.FraudLikelihood;
                    worksheet.Cell(i, 15).Value = data.IsSuspectIPAddress;
                    worksheet.Cell(i, 16).Value = data.IsSuspectAddress;
                    worksheet.Cell(i, 17).Value = data.IsSuspectZip4;
                    worksheet.Cell(i, 18).Value = data.IsSuspectEmail;
                    worksheet.Cell(i, 19).Value = data.IsSuspectEmailProvider;
                    worksheet.Cell(i, 20).Value = data.IsSuspectPhoneNumber;
                    worksheet.Cell(i, 21).Value = data.IsSuspectCorrelation;
                    i++;
                }

                // Save the Excel to a MemoryStream.
                MemoryStream stream = new MemoryStream();
                workbook.SaveAs(stream);
                output = stream.ToArray();
                stream.Dispose();
            }

            // Return the bytes of the MemoryStream.
            return output;
        }
    }
}
