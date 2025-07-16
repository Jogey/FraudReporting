using FraudReporting.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FraudReporting.Services
{
    public class FraudReportingService
    {
        //Properties
        public enum Scopes
        {
            ALL,
            DATETIME,
            BATCH
        }
        private float fraudScoringIPAddress, fraudScoringAddress, fraudScoringZip4, fraudScoringEmail, fraudScoringEmailProvider, fraudScoringPhoneNumber;
        private ComponentsValidationService componentsValidationService { get; set; }


        //Main methods
        public FraudReportingService(ComponentsValidationService componentsValidationService)
        {
            this.componentsValidationService = componentsValidationService;
        }

        public (Report Report, Output RecordsList) GetFraudReport(Context context, string scope, float confidenceThreshold, int countThreshold, FraudReportingServicePayload payload, bool createRecordsList = false, bool useParallelThreading = false)
        {
            componentsValidationService.SetVal(confidenceThreshold, countThreshold);
            fraudScoringIPAddress = payload.FraudScoringIPAddress;
            fraudScoringAddress = payload.FraudScoringAddress;
            fraudScoringZip4 = payload.FraudScoringZip4;
            fraudScoringEmail = payload.FraudScoringEmail;
            fraudScoringEmailProvider = payload.FraudScoringEmailProvider;
            fraudScoringPhoneNumber = payload.FraudScoringPhoneNumber;
            return (useParallelThreading) ? GetFraudReportParallelThreading(context, scope, createRecordsList) : GetFraudReport(context, scope, createRecordsList);
        }

        public string SetEntryScope(Entry entry, string scope)
        {
            string output = "ALL";

            if (scope == nameof(Scopes.DATETIME))
            {
                output = entry.DateTime.Value.Date.ToString("yyyy-MM-dd");
            }

            if (scope == nameof(Scopes.BATCH))
            {
                if (entry.Batch2 == "0")
                    output = "BATCH-ZERO";
                else
                    output = $"BATCH-{entry.Batch2}";
            }

            return output;
        }

        public FraudReportingServicePayload CreatePayload(float fraudScoringIPAddress, float fraudScoringAddress, float fraudScoringZip4, float fraudScoringEmail, float fraudScoringEmailProvider, float fraudScoringPhoneNumber)
        {
            return new FraudReportingServicePayload()
            {
                FraudScoringIPAddress = fraudScoringIPAddress,
                FraudScoringAddress = fraudScoringAddress,
                FraudScoringZip4 = fraudScoringZip4,
                FraudScoringEmail = fraudScoringEmail,
                FraudScoringEmailProvider = fraudScoringEmailProvider,
                FraudScoringPhoneNumber = fraudScoringPhoneNumber,
            };
        }

        public FraudReportingServicePayload CreatePayload(decimal fraudScoringIPAddress, decimal fraudScoringAddress, decimal fraudScoringZip4, decimal fraudScoringEmail, decimal fraudScoringEmailProvider, decimal fraudScoringPhoneNumber)
        {
            return CreatePayload((float)fraudScoringIPAddress, (float)fraudScoringAddress, (float)fraudScoringZip4, (float)fraudScoringEmail, (float)fraudScoringEmailProvider, (float)fraudScoringPhoneNumber);
        }


        //Helper methods
        private (Report Report, Output RecordsList) GetFraudReport(Context context, string scope, bool createRecordsList)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Restart();

            var query = context.Entries.GroupBy(x => x.Scope).OrderBy(x => x.Key);

            List<Entry> realtimeContext = new List<Entry>();
            List<Entry> suspectEntries = new List<Entry>();

            List<(string Scope,
                (int entriesTotal, int suspectTotal, int suspectIPAddressTotal, int suspectAddressTotal, int suspectZip4Total, int suspectEmailTotal, int suspectEmailProviderTotal, int suspectPhoneNumberTotal, int suspectCorrelationTotal) Data)>
                report = new();

            (string Scope,
                (int entriesTotal, int suspectTotal, int suspectIPAddressTotal, int suspectAddressTotal, int suspectZip4Total, int suspectEmailTotal, int suspectEmailProviderTotal, int suspectPhoneNumberTotal, int suspectCorrelationTotal) Data)
                reportItem = new();

            Output outputList = new Output();
            outputList.Items = new List<OutputItem>();

            foreach (var group in query)
            {
                bool isNewScope = !string.IsNullOrEmpty(reportItem.Scope) && group.Key != reportItem.Scope;

                realtimeContext.AddRange(group.ToList());
                reportItem = new() { Scope = group.Key };
                reportItem.Data.entriesTotal = realtimeContext.Count;
                if (isNewScope)
                {
                    reportItem.Data.suspectTotal += report.LastOrDefault().Data.suspectTotal;
                    reportItem.Data.suspectIPAddressTotal += report.LastOrDefault().Data.suspectIPAddressTotal;
                    reportItem.Data.suspectAddressTotal += report.LastOrDefault().Data.suspectAddressTotal;
                    reportItem.Data.suspectZip4Total += report.LastOrDefault().Data.suspectZip4Total;
                    reportItem.Data.suspectEmailTotal += report.LastOrDefault().Data.suspectEmailTotal;
                    reportItem.Data.suspectPhoneNumberTotal += report.LastOrDefault().Data.suspectPhoneNumberTotal;
                    reportItem.Data.suspectCorrelationTotal += report.LastOrDefault().Data.suspectCorrelationTotal;
                }

                foreach (var e in group) {
                    float fraudLikelihood = 0;
                    OutputItem outputListItem = new OutputItem() { Entry = e };

                    // Validate IP address against the entire entry database
                    string ip = e.IPAddress;
                    IEnumerable<Entry> ipList = realtimeContext.Where(x => x.Id != e.Id);
                    if (componentsValidationService.IsSuspectIPAddress(ip, ipList))
                    {
                        reportItem.Data.suspectIPAddressTotal++;
                        fraudLikelihood += fraudScoringIPAddress;
                        outputListItem.IsSuspectIPAddress = true;
                    }

                    // Validate phone number against list of known phone numbers by state
                    string phone = e.Phone;
                    string state = e.State;
                    if (componentsValidationService.IsSuspectPhoneNumber(phone, state))
                    {
                        reportItem.Data.suspectPhoneNumberTotal++;
                        fraudLikelihood += fraudScoringPhoneNumber;
                        outputListItem.IsSuspectPhoneNumber = true;
                    }

                    // Validate name-to-email similarity
                    string name = e.Name;
                    string email = e.Email;
                    if (componentsValidationService.IsSuspectEmail(name, email))
                    {
                        reportItem.Data.suspectEmailTotal++;
                        fraudLikelihood += fraudScoringEmail;
                        outputListItem.IsSuspectEmail = true;
                    }

                    if (componentsValidationService.IsSuspectEmailProvider(email))
                    {
                        reportItem.Data.suspectEmailProviderTotal++;
                        fraudLikelihood += fraudScoringEmailProvider;
                        outputListItem.IsSuspectEmailProvider = true;
                    }

                    // Validate address against the entire entry database
                    string address = $"{e.Address} {e.City} {e.State} {e.Zip}";
                    IEnumerable<Entry> addressList = realtimeContext.Where(x => x.City == e.City && x.Id != e.Id);
                    if (componentsValidationService.IsSuspectAddress(address, addressList))
                    {
                        reportItem.Data.suspectAddressTotal++;
                        fraudLikelihood += fraudScoringAddress;
                        outputListItem.IsSuspectAddress = true;
                    }

                    // Validate zip4 to ensure that it isn't empty (only applies to US addresses)
                    string zip4 = e.Zip4;
                    string country = e.Country;
                    if (componentsValidationService.IsSuspectZip4(zip4, country))
                    {
                        reportItem.Data.suspectZip4Total++;
                        fraudLikelihood += fraudScoringZip4;
                        outputListItem.IsSuspectZip4 = true;
                    }

                    bool isSuspect = fraudLikelihood >= componentsValidationService.GetLikelinessThreshold();

                    // Validate the correlation of the entry with known suspect entries
                    if (!isSuspect)
                    {
                        IEnumerable<Entry> suspectEntriesList = suspectEntries;
                        if (componentsValidationService.IsSuspectCorrelation(e, suspectEntriesList))
                        {
                            reportItem.Data.suspectCorrelationTotal++;
                            fraudLikelihood += 1.0f;
                            isSuspect = true;
                            outputListItem.IsSuspectCorrelation = true;
                        }
                    }

                    // Finally, flag the entry as a suspect fraud if the confidence threshold has been met
                    if (isSuspect)
                    {
                        if (suspectEntries.FirstOrDefault(x => x.Id == e.Id) == null)
                        {
                            suspectEntries.Add(e);
                        }
                        reportItem.Data.suspectTotal++;
                        outputListItem.FraudLikelihood = fraudLikelihood;
                        outputList.Items.Add(outputListItem);
                    }
                }

                report.Add(reportItem);
            }

            stopwatch.Stop();

            var output = new Report()
            {
                TimeElasped = stopwatch.Elapsed,
                Scope = scope,
                ConfidenceThreshold = componentsValidationService.GetLikelinessThreshold(),
                CountThreshold = componentsValidationService.GetCountThreshold(),
                FraudScoringIPAddress = fraudScoringIPAddress,
                FraudScoringAddress = fraudScoringAddress,
                FraudScoringZip4 = fraudScoringZip4,
                FraudScoringEmail = fraudScoringEmail,
                FraudScoringEmailProvider = fraudScoringEmailProvider,
                FraudScoringPhoneNumber = fraudScoringPhoneNumber,
                Items = new List<FraudReportItem>()
            };
            foreach (var r in report)
            {
                output.Items.Add(new FraudReportItem()
                {
                    Scope = r.Scope,
                    PercentageFraud = MathF.Round(((float)r.Data.suspectTotal / (float)r.Data.entriesTotal) * 100f, 2),
                    TotalRecords = r.Data.entriesTotal,
                    TotalFraudCount = r.Data.suspectTotal,
                    SuspectedIPAddressCount = r.Data.suspectIPAddressTotal,
                    SuspectedAddressCount = r.Data.suspectAddressTotal,
                    SuspectedZip4Count = r.Data.suspectZip4Total,
                    SuspectedEmailCount = r.Data.suspectEmailTotal,
                    SuspectedEmailProviderCount = r.Data.suspectEmailProviderTotal,
                    SuspectedPhoneNumberCount = r.Data.suspectPhoneNumberTotal,
                    SuspectedCorrelationCount = r.Data.suspectCorrelationTotal
                });
            }
            
            return (output, createRecordsList ? outputList : null);
        }

        private (Report Report, Output RecordsList) GetFraudReportParallelThreading(Context context, string scope, bool createRecordsList)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Restart();

            var query = context.Entries.GroupBy(x => x.Scope).OrderBy(x => x.Key);

            List<Entry> realtimeContext = new List<Entry>();
            ConcurrentBag<Entry> suspectEntries = new ConcurrentBag<Entry>();

            List<(string Scope,
                (int entriesTotal, int suspectTotal, int suspectIPAddressTotal, int suspectAddressTotal, int suspectZip4Total, int suspectEmailTotal, int suspectEmailProviderTotal, int suspectPhoneNumberTotal, int suspectCorrelationTotal) Data)>
                report = new();

            (string Scope,
                (int entriesTotal, int suspectTotal, int suspectIPAddressTotal, int suspectAddressTotal, int suspectZip4Total, int suspectEmailTotal, int suspectEmailProviderTotal, int suspectPhoneNumberTotal, int suspectCorrelationTotal) Data)
                reportItem = new();

            ConcurrentBag<OutputItem> outputListItems = new ConcurrentBag<OutputItem>();

            foreach (var group in query)
            {
                bool isNewScope = !string.IsNullOrEmpty(reportItem.Scope) && group.Key != reportItem.Scope;

                realtimeContext.AddRange(group.ToList());
                reportItem = new() { Scope = group.Key };
                reportItem.Data.entriesTotal = realtimeContext.Count;
                if (isNewScope)
                {
                    reportItem.Data.suspectTotal += report.LastOrDefault().Data.suspectTotal;
                    reportItem.Data.suspectIPAddressTotal += report.LastOrDefault().Data.suspectIPAddressTotal;
                    reportItem.Data.suspectAddressTotal += report.LastOrDefault().Data.suspectAddressTotal;
                    reportItem.Data.suspectZip4Total += report.LastOrDefault().Data.suspectZip4Total;
                    reportItem.Data.suspectEmailTotal += report.LastOrDefault().Data.suspectEmailTotal;
                    reportItem.Data.suspectPhoneNumberTotal += report.LastOrDefault().Data.suspectPhoneNumberTotal;
                    reportItem.Data.suspectCorrelationTotal += report.LastOrDefault().Data.suspectCorrelationTotal;
                }

                Parallel.ForEach(group, e => {
                    float fraudLikelihood = 0;
                    OutputItem outputListItem = new OutputItem() { Entry = e };

                    // Validate IP address against the entire entry database
                    string ip = e.IPAddress;
                    IEnumerable<Entry> ipList = realtimeContext.Where(x => x.Id != e.Id);
                    if (componentsValidationService.IsSuspectIPAddress(ip, ipList))
                    {
                        Interlocked.Increment(ref reportItem.Data.suspectIPAddressTotal);
                        fraudLikelihood += fraudScoringIPAddress;
                        outputListItem.IsSuspectIPAddress = true;
                    }

                    // Validate phone number against list of known phone numbers by state
                    string phone = e.Phone;
                    string state = e.State;
                    if (componentsValidationService.IsSuspectPhoneNumber(phone, state))
                    {
                        Interlocked.Increment(ref reportItem.Data.suspectPhoneNumberTotal);
                        fraudLikelihood += fraudScoringPhoneNumber;
                        outputListItem.IsSuspectPhoneNumber = true;
                    }

                    // Validate name-to-email similarity
                    string name = e.Name;
                    string email = e.Email;
                    if (componentsValidationService.IsSuspectEmail(name, email))
                    {
                        Interlocked.Increment(ref reportItem.Data.suspectEmailTotal);
                        fraudLikelihood += fraudScoringEmail;
                        outputListItem.IsSuspectEmail = true;
                    }

                    if (componentsValidationService.IsSuspectEmailProvider(email))
                    {
                        Interlocked.Increment(ref reportItem.Data.suspectEmailProviderTotal);
                        fraudLikelihood += fraudScoringEmailProvider;
                        outputListItem.IsSuspectEmailProvider = true;
                    }

                    // Validate address against the entire entry database
                    string address = $"{e.Address} {e.City} {e.State} {e.Zip}";
                    IEnumerable<Entry> addressList = realtimeContext.Where(x => x.City == e.City && x.Id != e.Id);
                    if (componentsValidationService.IsSuspectAddress(address, addressList))
                    {
                        Interlocked.Increment(ref reportItem.Data.suspectAddressTotal);
                        fraudLikelihood += fraudScoringAddress;
                        outputListItem.IsSuspectAddress = true;
                    }

                    // Validate zip4 to ensure that it isn't empty (only applies to US addresses)
                    string zip4 = e.Zip4;
                    string country = e.Country;
                    if (componentsValidationService.IsSuspectZip4(zip4, country))
                    {
                        Interlocked.Increment(ref reportItem.Data.suspectZip4Total);
                        fraudLikelihood += fraudScoringZip4;
                        outputListItem.IsSuspectZip4 = true;
                    }

                    bool isSuspect = fraudLikelihood >= componentsValidationService.GetLikelinessThreshold();

                    // Validate the correlation of the entry with known suspect entries
                    if (!isSuspect)
                    {
                        IEnumerable<Entry> suspectEntriesList = suspectEntries;
                        if (componentsValidationService.IsSuspectCorrelation(e, suspectEntriesList))
                        {
                            Interlocked.Increment(ref reportItem.Data.suspectCorrelationTotal);
                            fraudLikelihood += 1.0f;
                            isSuspect = true;
                            outputListItem.IsSuspectCorrelation = true;
                        }
                    }

                    // Finally, flag the entry as a suspect fraud if the confidence threshold has been met
                    if (isSuspect)
                    {
                        if (suspectEntries.FirstOrDefault(x => x.Id == e.Id) == null)
                        {
                            suspectEntries.Add(e);
                        }
                        Interlocked.Increment(ref reportItem.Data.suspectTotal);
                        outputListItem.FraudLikelihood = fraudLikelihood;
                        outputListItems.Add(outputListItem);
                    }
                });

                report.Add(reportItem);
            }

            stopwatch.Stop();

            var output = new Report()
            {
                TimeElasped = stopwatch.Elapsed,
                Scope = scope,
                ConfidenceThreshold = componentsValidationService.GetLikelinessThreshold(),
                CountThreshold = componentsValidationService.GetCountThreshold(),
                FraudScoringIPAddress = fraudScoringIPAddress,
                FraudScoringAddress = fraudScoringAddress,
                FraudScoringZip4 = fraudScoringZip4,
                FraudScoringEmail = fraudScoringEmail,
                FraudScoringEmailProvider = fraudScoringEmailProvider,
                FraudScoringPhoneNumber = fraudScoringPhoneNumber,
                Items = new List<FraudReportItem>()
            };
            foreach (var r in report)
            {
                output.Items.Add(new FraudReportItem()
                {
                    Scope = r.Scope,
                    PercentageFraud = MathF.Round(((float)r.Data.suspectTotal / (float)r.Data.entriesTotal) * 100f, 2),
                    TotalRecords = r.Data.entriesTotal,
                    TotalFraudCount = r.Data.suspectTotal,
                    SuspectedIPAddressCount = r.Data.suspectIPAddressTotal,
                    SuspectedAddressCount = r.Data.suspectAddressTotal,
                    SuspectedZip4Count = r.Data.suspectZip4Total,
                    SuspectedEmailCount = r.Data.suspectEmailTotal,
                    SuspectedEmailProviderCount = r.Data.suspectEmailProviderTotal,
                    SuspectedPhoneNumberCount = r.Data.suspectPhoneNumberTotal,
                    SuspectedCorrelationCount = r.Data.suspectCorrelationTotal
                });
            }
            var outputList = new Output();
            if (createRecordsList)
            {
                outputList.Items = outputListItems.ToList();
            }
            else
            {
                outputList = null;
            }

            return (output, outputList);
        }
    }

    public class FraudReportingServicePayload
    {
        public float FraudScoringIPAddress { get; set; }

        public float FraudScoringAddress { get; set; }

        public float FraudScoringZip4 { get; set; }

        public float FraudScoringEmail { get; set; }

        public float FraudScoringEmailProvider { get; set; }

        public float FraudScoringPhoneNumber { get; set; }

        public FraudReportingServicePayload()
        { }
    }
}
