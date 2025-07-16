using FraudReporting.Models;
using FraudReporting.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FraudReporting.Services
{
    public class ComponentsValidationService
    {
        //Properties
        private float likelinessThreshold;
        private int countThreshold;


        //Main methods
        public ComponentsValidationService()
        { }

        public void SetVal(float likelinessThreshold, int countThreshold)
        {
            this.likelinessThreshold = likelinessThreshold;
            this.countThreshold = countThreshold;
        }

        public float GetLikelinessThreshold()
        {
            return likelinessThreshold;
        }

        public int GetCountThreshold()
        {
            return countThreshold;
        }

        public bool IsSuspectIPAddress(string ip, IEnumerable<Entry> context)
        {
            if (string.IsNullOrEmpty(ip)) return false;
            string baseIp = (ip.Contains(".")) ? ip.Substring(0, ip.LastIndexOf(".")) : ip;
            int output = context.Where(x => x.IPAddress.StartsWith(baseIp)).Count();
            return output >= Math.Min(countThreshold * 10, 50);
        }

        public bool IsSuspectAddress(string address, IEnumerable<Entry> context)
        {
            int count = 0;
            foreach (var i in context)
            {
                string comparativeAddress = $"{i.Address} {i.City} {i.State} {i.Zip}";
                if (CompareAddresses(address, comparativeAddress)) count++;
                if (count >= countThreshold * 2) return true;
            }
            return false;
        }

        public bool IsSuspectZip4(string zip4, string country)
        {
            bool output = false;
            if (country == "US") output = string.IsNullOrEmpty(zip4);
            return output;
        }

        public bool IsSuspectEmail(string name, string email)
        {
            float output = 1;
            string n = name.ToUpper();
            string e = email.ToUpper().Split('@')[0];
            e = Regex.Replace(e, @"!@#$%^&*()-=+.,", string.Empty);
            e = Regex.Replace(e, @"[\d-]", string.Empty);
            output = LevenshteinDistance.GetStringSimilarity(n, e);
            return output <= (1 - likelinessThreshold);
        }

        public bool IsSuspectEmailProvider(string email)
        {
            bool output = false;
            if (!string.IsNullOrEmpty(email) && email.Contains('@'))
            {
                List<string> commonProviders = new List<string>() {
                    "AIM.COM",
                    "ALICE.IT",
                    "ALICEADSL.FR",
                    "AOL.COM",
                    "ARCOR.DE",
                    "ATT.NET",
                    "BELLSOUTH.NET",
                    "BIGPOND.COM",
                    "BIGPOND.NET.AU",
                    "BLUEWIN.CH",
                    "BLUEYONDER.CO.UK",
                    "BOL.COM.BR",
                    "CENTURYTEL.NET",
                    "CHARTER.NET",
                    "CHELLO.NL",
                    "CLUB-INTERNET.FR",
                    "COMCAST.NET",
                    "COX.NET",
                    "EARTHLINK.NET",
                    "FACEBOOK.COM",
                    "FREE.FR",
                    "FREENET.DE",
                    "FRONTIERNET.NET",
                    "GMAIL.COM",
                    "GMX.DE",
                    "GMX.NET",
                    "GOOGLEMAIL.COM",
                    "HETNET.NL",
                    "HOME.NL",
                    "HOTMAIL.CO.UK",
                    "HOTMAIL.COM",
                    "HOTMAIL.DE",
                    "HOTMAIL.ES",
                    "HOTMAIL.FR",
                    "HOTMAIL.IT",
                    "IG.COM.BR",
                    "JUNO.COM",
                    "LAPOSTE.NET",
                    "LIBERO.IT",
                    "LIVE.CA",
                    "LIVE.CO.UK",
                    "LIVE.COM",
                    "LIVE.COM.AU",
                    "LIVE.FR",
                    "LIVE.IT",
                    "LIVE.NL",
                    "MAC.COM",
                    "MAIL.COM",
                    "MAIL.RU",
                    "ME.COM",
                    "MSN.COM",
                    "NEUF.FR",
                    "NTLWORLD.COM",
                    "OPTONLINE.NET",
                    "OPTUSNET.COM.AU",
                    "ORANGE.FR",
                    "OUTLOOK.COM",
                    "PLANET.NL",
                    "PROTON.ME",
                    "PROTONMAIL.COM",
                    "QQ.COM",
                    "RAMBLER.RU",
                    "REDIFFMAIL.COM",
                    "ROCKETMAIL.COM",
                    "SBCGLOBAL.NET",
                    "SFR.FR",
                    "SHAW.CA",
                    "SKY.COM",
                    "SKYNET.BE",
                    "SYMPATICO.CA",
                    "T-ONLINE.DE",
                    "TELENET.BE",
                    "TERRA.COM.BR",
                    "TIN.IT",
                    "TISCALI.CO.UK",
                    "TISCALI.IT",
                    "UOL.COM.BR",
                    "VERIZON.NET",
                    "VIRGILIO.IT",
                    "VOILA.FR",
                    "WANADOO.FR",
                    "WEB.DE",
                    "WINDSTREAM.NET",
                    "YAHOO.CA",
                    "YAHOO.CO.ID",
                    "YAHOO.CO.IN",
                    "YAHOO.CO.JP",
                    "YAHOO.CO.UK",
                    "YAHOO.COM",
                    "YAHOO.COM.AR",
                    "YAHOO.COM.AU",
                    "YAHOO.COM.BR",
                    "YAHOO.COM.MX",
                    "YAHOO.COM.SG",
                    "YAHOO.DE",
                    "YAHOO.ES",
                    "YAHOO.FR",
                    "YAHOO.IN",
                    "YAHOO.IT",
                    "YANDEX.RU",
                    "YMAIL.COM",
                    "ZOHO.COM",
                    "ZONNET.NL"
                };
                string provider = email.ToUpper().Split('@')[1];
                output = !commonProviders.Contains(provider);
            }
            return output;
        }

        public bool IsSuspectPhoneNumber(string phone, string state)
        {
            bool output = false;
            if (string.IsNullOrEmpty(phone)) return output;
            Dictionary<string, List<string>> USAreaCodes = new Dictionary<string, List<string>>() {
                { "AK", new List<string>() { "907" } },
                { "AL", new List<string>() { "205", "251", "256", "334" } },
                { "AR", new List<string>() { "479", "501", "870" } },
                { "AZ", new List<string>() { "480", "520", "602", "623", "928" } },
                { "CA", new List<string>() { "209", "213", "310", "323", "408", "415", "510", "530", "559", "562", "619", "626", "650", "661", "707", "714", "760", "805", "818", "831", "858", "909", "916", "925", "949", "951" } },
                { "CO", new List<string>() { "303", "719", "970" } },
                { "CT", new List<string>() { "203", "860" } },
                { "DC", new List<string>() { "202" } },
                { "DE", new List<string>() { "302" } },
                { "FL", new List<string>() { "239", "305", "321", "352", "386", "407", "561", "727", "772", "813", "850", "863", "904", "941", "954" } },
                { "GA", new List<string>() { "229", "404", "478", "706", "770", "912" } },
                { "HI", new List<string>() { "808" } },
                { "IA", new List<string>() { "319", "515", "563", "641", "712" } },
                { "ID", new List<string>() { "208" } },
                { "IL", new List<string>() { "217", "309", "312", "618", "630", "708", "773", "815", "847" } },
                { "IN", new List<string>() { "219", "260", "317", "574", "765", "812" } },
                { "KS", new List<string>() { "316", "620", "785", "913" } },
                { "KY", new List<string>() { "270", "502", "606", "859" } },
                { "LA", new List<string>() { "225", "318", "337", "504", "985" } },
                { "MA", new List<string>() { "413", "508", "617", "781", "978" } },
                { "MD", new List<string>() { "301", "410" } },
                { "ME", new List<string>() { "207" } },
                { "MI", new List<string>() { "231", "248", "269", "313", "517", "586", "616", "734", "810", "906", "989" } },
                { "MN", new List<string>() { "218", "320", "507", "612", "651", "763", "952" } },
                { "MO", new List<string>() { "314", "417", "573", "636", "660", "816" } },
                { "MS", new List<string>() { "228", "601", "662" } },
                { "MT", new List<string>() { "406" } },
                { "NC", new List<string>() { "252", "336", "704", "828", "910", "919" } },
                { "ND", new List<string>() { "701" } },
                { "NE", new List<string>() { "308", "402" } },
                { "NH", new List<string>() { "603" } },
                { "NJ", new List<string>() { "201", "609", "732", "856", "908", "973" } },
                { "NM", new List<string>() { "505", "575" } },
                { "NV", new List<string>() { "702", "775" } },
                { "NY", new List<string>() { "212", "315", "516", "518", "585", "607", "631", "716", "718", "845", "914" } },
                { "OH", new List<string>() { "216", "330", "419", "440", "513", "614", "740", "937" } },
                { "OK", new List<string>() { "405", "580", "918" } },
                { "OR", new List<string>() { "503", "541" } },
                { "PA", new List<string>() { "215", "412", "570", "610", "717", "724", "814" } },
                { "RI", new List<string>() { "401" } },
                { "SC", new List<string>() { "803", "843", "864" } },
                { "SD", new List<string>() { "605" } },
                { "TN", new List<string>() { "423", "615", "731", "865", "901", "931" } },
                { "TX", new List<string>() { "210", "214", "254", "281", "325", "361", "409", "432", "512", "713", "806", "817", "830", "903", "915", "936", "940", "956", "972", "979" } },
                { "UT", new List<string>() { "435", "801" } },
                { "VA", new List<string>() { "276", "434", "540", "703", "757", "804" } },
                { "VT", new List<string>() { "802" } },
                { "WA", new List<string>() { "206", "253", "360", "425", "509" } },
                { "WI", new List<string>() { "262", "414", "608", "715", "920" } },
                { "WV", new List<string>() { "304" } },
                { "WY", new List<string>() { "307" } }
            };
            string phoneAreaCode = (phone.Length > 3) ? phone.Substring(0, 3) : phone;
            if (USAreaCodes.ContainsKey(state)) output = USAreaCodes[state].Contains(phoneAreaCode);
            return output;
        }

        public bool IsSuspectCorrelation(Entry e, IEnumerable<Entry> context)
        {
            foreach (var i in context)
            {
                float output = 0;
                if (CompareIpAddresses(e.IPAddress, i.IPAddress)) output += 0.5f;
                if (CompareNames(e.Name, i.Name)) output += 0.25f;
                if (CompareEmails(e.Email, i.Email)) output += 0.75f;
                if (ComparePhones(e.Phone, i.Phone)) output += 0.25f;
                if (output >= likelinessThreshold) return true;
            }
            return false;
        }


        //Helper methods
        private bool CompareAddresses(string address1, string address2)
        {
            float output = CosineSimilarity.GetWordSimilarity($"_{address1}", $"_{address2}", " ", AssignAddressWeight);
            return output >= likelinessThreshold;
        }

        private float AssignAddressWeight(string str)
        {
            float output = 1;
            List<string> unitDesignators = new List<string>() {
                "APT",
                "BLDG",
                "DEPT",
                "FL",
                "UNIT",
                "RM",
                "STE",
                "APARTMENT",
                "BUILDING",
                "DEPARTMENT",
                "FLOOR",
                "ROOM",
                "SUITE"
            };
            if (string.IsNullOrEmpty(str))
            {
                return 0;
            }
            else
            {
                if (str.Length <= 2)
                    output = 0.5f;

                if (unitDesignators.Contains(str.ToUpper()))
                    output = 0.25f;

                if (str[0] != '_' && int.TryParse(str, out _))
                    output = 0.25f;
            }
            return output;
        }

        private bool CompareIpAddresses(string ip1, string ip2)
        {
            if (string.IsNullOrEmpty(ip1) || string.IsNullOrEmpty(ip2)) return false;
            string baseIp1 = ip1.Substring(0, ip1.LastIndexOf("."));
            string baseIp2 = ip2.Substring(0, ip2.LastIndexOf("."));
            return baseIp1 == baseIp2;
        }

        private bool CompareNames(string name1, string name2)
        {
            string n1 = name1.ToUpper();
            string n2 = name2.ToUpper();
            float output = LevenshteinDistance.GetStringSimilarity(n1, n2);
            return output >= likelinessThreshold;
        }

        private bool CompareEmails(string email1, string email2)
        {
            string[] e1 = email1.ToUpper().Split('@');
            string[] e2 = email2.ToUpper().Split('@');
            if (e1[1] == e2[1])
            {
                float output = LevenshteinDistance.GetStringSimilarity(e1[0], e2[0]);
                return output >= likelinessThreshold;
            }
            return false;
        }

        private bool ComparePhones(string phone1, string phone2)
        {
            return phone1 == phone2;
        }
    }
}
