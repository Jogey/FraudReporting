using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace FraudReporting.Models
{
    public class Entry
    {
        public string Scope { get; set; }

        public string Id { get; set; }

        public DateTime? DateTime { get; set; }

        [MaxLength(45)]
        public string IPAddress { get; set; }

        [MaxLength(40)]
        public string Name { get; set; }

        [MaxLength(50)]
        public string Email { get; set; }

        [MaxLength(10)]
        public string Phone { get; set; }

        [MaxLength(45)]
        public string Address { get; set; }

        [MaxLength(35)]
        public string City { get; set; }

        [MaxLength(2)]
        public string State { get; set; }

        [MaxLength(6)]
        public string Zip { get; set; }

        [MaxLength(4)]
        public string Zip4 { get; set; }

        [MaxLength(2)]
        public string Country { get; set; }

        [MaxLength(4)]
        public string Batch2 { get; set; }
    }

    public class Context
    {
        public List<Entry> Entries { get; set; }
    }
}
