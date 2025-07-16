using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apfco;
using Microsoft.Extensions.Configuration;

namespace FraudReporting.Services
{
    /// <summary>
    /// Service for retrieving values from the APF Password Manager system. Used as a separte class instead of retrieving passwords inline in other
    /// classes, as this way calls only need to be made once and can be pre-plugged or mocked as well.
    /// </summary>
    public class PasswordManagerService
    {
        private IConfiguration configuration;


        //Strongly typed lazy-loaded properties
        public string ApfEmailSmtpPassword
        {
            get
            {
                if (_ApfEmailSmtpPassword == null)
                    _ApfEmailSmtpPassword = GetVariable(nameof(ApfEmailSmtpPassword));
                return _ApfEmailSmtpPassword;
            }
            set
            {
                _ApfEmailSmtpPassword = value;
            }
        }
        private string _ApfEmailSmtpPassword;

        public string ApfReCaptchaNoDomainVerifyPrivateKey
        {
            get
            {
                if (_ApfReCaptchaNoDomainVerifyPrivateKey == null)
                    _ApfReCaptchaNoDomainVerifyPrivateKey = GetVariable(nameof(ApfReCaptchaNoDomainVerifyPrivateKey));
                return _ApfReCaptchaNoDomainVerifyPrivateKey;
            }
            set
            {
                _ApfReCaptchaNoDomainVerifyPrivateKey = value;
            }
        }
        private string _ApfReCaptchaNoDomainVerifyPrivateKey;

        public string AuthorizeNetLoginId_Apfco
        {
            get
            {
                if (_AuthorizeNetLoginId_Apfco == null)
                    _AuthorizeNetLoginId_Apfco = GetVariable(nameof(AuthorizeNetLoginId_Apfco));
                return _AuthorizeNetLoginId_Apfco;
            }
            set
            {
                _AuthorizeNetLoginId_Apfco = value;
            }
        }
        private string _AuthorizeNetLoginId_Apfco;

        public string AuthorizeNetTransactionKey_Apfco
        {
            get
            {
                if (_AuthorizeNetTransactionKey_Apfco == null)
                    _AuthorizeNetTransactionKey_Apfco = GetVariable(nameof(AuthorizeNetTransactionKey_Apfco));
                return _AuthorizeNetTransactionKey_Apfco;
            }
            set
            {
                _AuthorizeNetTransactionKey_Apfco = value;
            }
        }
        private string _AuthorizeNetTransactionKey_Apfco;


        public string Zip4SatoriProductKey
        {
            get
            {
                if (_Zip4SatoriProductKey == null)
                    _Zip4SatoriProductKey = GetVariable(nameof(Zip4SatoriProductKey));
                return _Zip4SatoriProductKey;
            }
            set
            {
                _Zip4SatoriProductKey = value;
            }
        }
        private string _Zip4SatoriProductKey;

        public string ApfPayPalApiPasswordStage
        {
            get
            {
                if (_ApfPayPalApiPasswordStage == null)
                    _ApfPayPalApiPasswordStage = GetVariable("apfPayPalApiPassword_Stage");
                return _ApfPayPalApiPasswordStage;
            }
            set
            {
                _ApfPayPalApiPasswordStage = value;
            }
        }
        private string _ApfPayPalApiPasswordStage;

        public string ApfPayPalApiPasswordLive
        {
            get
            {
                if (_ApfPayPalApiPasswordLive == null)
                    _ApfPayPalApiPasswordLive = GetVariable("apfPayPalApiPassword_Live");
                return _ApfPayPalApiPasswordLive;
            }
            set
            {
                _ApfPayPalApiPasswordLive = value;
            }
        }
        private string _ApfPayPalApiPasswordLive;

        public string ApfEmailApiPasswordStage
        {
            get
            {
                if (_ApfEmailApiPasswordStage == null)
                    _ApfEmailApiPasswordStage = GetVariable("apfEmailApiPassword_Stage");
                return _ApfEmailApiPasswordStage;
            }
            set
            {
                _ApfEmailApiPasswordStage = value;
            }
        }
        private string _ApfEmailApiPasswordStage;

        public string ApfEmailApiPasswordLive
        {
            get
            {
                if (_ApfEmailApiPasswordLive == null)
                    _ApfEmailApiPasswordLive = GetVariable("apfEmailApiPassword_Live");
                return _ApfEmailApiPasswordLive;
            }
            set
            {
                _ApfEmailApiPasswordLive = value;
            }
        }
        private string _ApfEmailApiPasswordLive;


        //Methods
        public PasswordManagerService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string GetVariable(string name)
        {
            return UTIL.GetPasswordManagerVariable(
                    configuration.GetConnectionString("Support"),
                    name,
                    configuration["PasswordManagerGroups:GroupId"],
                    configuration["PasswordManagerGroups:PassPhrase"]
                );
        }

        public PasswordManagerConfigElement GetConfigElement()
        {
            var configElement = new PasswordManagerConfigElement();
            configElement.GroupName = configuration["PasswordManagerGroups:GroupName"];
            configElement.GroupId = configuration["PasswordManagerGroups:GroupId"];
            configElement.PassPhrase = configuration["PasswordManagerGroups:PassPhrase"];
            return configElement;
        }
    }
}
