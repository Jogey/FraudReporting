using System;
using System.Collections.Generic;

namespace FraudReporting.Models
{
    public class Report
    {
        public TimeSpan TimeElasped { get; set; }

        public string Scope { get; set; }

        public float ConfidenceThreshold { get; set; }

        public int CountThreshold { get; set; }

        public float FraudScoringIPAddress { get; set; }

        public float FraudScoringAddress { get; set; }

        public float FraudScoringZip4 { get; set; }

        public float FraudScoringEmail { get; set; }

        public float FraudScoringEmailProvider { get; set; }

        public float FraudScoringPhoneNumber { get; set; }


        public List<FraudReportItem> Items { get; set; }

        public string RecordsListFilename { get; set; }

        public Report()
        {
            Items = new List<FraudReportItem>();
        }
    }

    public class FraudReportItem
    {
        public string Scope { get; set; }

        public float PercentageFraud { get; set; }

        public int TotalRecords { get; set; }

        public int TotalFraudCount { get; set; }

        public int SuspectedIPAddressCount { get; set; }

        public int SuspectedAddressCount { get; set; }

        public int SuspectedZip4Count { get; set; }

        public int SuspectedEmailCount { get; set; }

        public int SuspectedEmailProviderCount { get; set; }

        public int SuspectedPhoneNumberCount { get; set; }

        public int SuspectedCorrelationCount { get; set; }

        public FraudReportItem()
        { }
    }

    public class Output
    {
        public List<OutputItem> Items { get; set; }

        public Output()
        { }
    }

    public class OutputItem
    {
        public Entry Entry { get; set; }


        public float FraudLikelihood { get; set; }

        public bool IsSuspectIPAddress { get; set; }

        public bool IsSuspectAddress { get; set; }

        public bool IsSuspectZip4 { get; set; }

        public bool IsSuspectEmail { get; set; }

        public bool IsSuspectEmailProvider { get; set; }

        public bool IsSuspectPhoneNumber { get; set; }

        public bool IsSuspectCorrelation { get; set; }

        public OutputItem()
        { }
    }
}
