using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Apfco;
using Microsoft.Extensions.Configuration;

namespace FraudReporting.Services
{
    /// <summary>
    /// Service for retrieving values from the apfvariables SQL table. Used as a separte class instead of retrieving ApfVariables inline in other
    /// classes, as this way calls only need to be made once and can be pre-plugged or mocked as well.
    /// </summary>
    public class ApfVariablesService
    {
        private IConfiguration configuration;


        //Strongly typed lazy-loaded properties
        public string ApfEmailSmtpHost
        {
            get
            {
                if (_ApfEmailSmtpHost == null)
                    _ApfEmailSmtpHost = GetVariable(nameof(ApfEmailSmtpHost));
                return _ApfEmailSmtpHost;
            }
            set
            {
                _ApfEmailSmtpHost = value;
            }
        }
        private string _ApfEmailSmtpHost;

        public string ApfEmailSmtpUsername
        {
            get
            {
                if (_ApfEmailSmtpUsername == null)
                    _ApfEmailSmtpUsername = GetVariable(nameof(ApfEmailSmtpUsername));
                return _ApfEmailSmtpUsername;
            }
            set
            {
                _ApfEmailSmtpUsername = value;
            }
        }
        private string _ApfEmailSmtpUsername;

        public string ApfReCaptchaNoDomainVerifyPublicKey
        {
            get
            {
                if (_ApfReCaptchaNoDomainVerifyPublicKey == null)
                    _ApfReCaptchaNoDomainVerifyPublicKey = GetVariable(nameof(ApfReCaptchaNoDomainVerifyPublicKey));
                return _ApfReCaptchaNoDomainVerifyPublicKey;
            }
            set
            {
                _ApfReCaptchaNoDomainVerifyPublicKey = value;
            }
        }
        private string _ApfReCaptchaNoDomainVerifyPublicKey;

        public string AuthorizeNetApiUrl
        {
            get
            {
                if (_AuthorizeNetApiUrl == null)
                    _AuthorizeNetApiUrl = GetVariable(nameof(AuthorizeNetApiUrl));
                return _AuthorizeNetApiUrl;
            }
            set
            {
                _AuthorizeNetApiUrl = value;
            }
        }
        private string _AuthorizeNetApiUrl;

        public string Zip4SatoriHttpsAPIEndpoint
        {
            get
            {
                if (_Zip4SatoriHttpsAPIEndpoint == null)
                    _Zip4SatoriHttpsAPIEndpoint = GetVariable(nameof(Zip4SatoriHttpsAPIEndpoint));
                return _Zip4SatoriHttpsAPIEndpoint;
            }
            set
            {
                _Zip4SatoriHttpsAPIEndpoint = value;
            }
        }
        private string _Zip4SatoriHttpsAPIEndpoint;

        public string ApfPayPalApiUserNameStage
        {
            get
            {
                if (_ApfPayPalApiUserNameStage == null)
                    _ApfPayPalApiUserNameStage = GetVariable("apfPayPalApiUserName_Stage");
                return _ApfPayPalApiUserNameStage;
            }
            set
            {
                _ApfPayPalApiUserNameStage = value;
            }
        }
        private string _ApfPayPalApiUserNameStage;

        public string ApfPayPalApiUserNameLive
        {
            get
            {
                if (_ApfPayPalApiUserNameLive == null)
                    _ApfPayPalApiUserNameLive = GetVariable("apfPayPalApiUserName_Live");
                return _ApfPayPalApiUserNameLive;
            }
            set
            {
                _ApfPayPalApiUserNameLive = value;
            }
        }
        private string _ApfPayPalApiUserNameLive;

        public string APFCOPayPalLockEmailEndPointLive
        {
            get
            {
                if (_APFCOPayPalLockEmailEndPointLive == null)
                    _APFCOPayPalLockEmailEndPointLive = GetVariable("APFCOPayPalLockEmailEndPoint_Live");
                return _APFCOPayPalLockEmailEndPointLive;
            }
            set
            {
                _APFCOPayPalLockEmailEndPointLive = value;
            }
        }
        private string _APFCOPayPalLockEmailEndPointLive;

        public string APFCOPayPalLockEmailEndPointStage
        {
            get
            {
                if (_APFCOPayPalLockEmailEndPointStage == null)
                    _APFCOPayPalLockEmailEndPointStage = GetVariable("APFCOPayPalLockEmailEndPoint_Stage");
                return _APFCOPayPalLockEmailEndPointStage;
            }
            set
            {
                _APFCOPayPalLockEmailEndPointStage = value;
            }
        }
        private string _APFCOPayPalLockEmailEndPointStage;

        public string APFCOPayPalLockPhoneNumberEndPointLive
        {
            get
            {
                if (_APFCOPayPalLockPhoneNumberEndPointLive == null)
                    _APFCOPayPalLockPhoneNumberEndPointLive = GetVariable("APFCOPayPalLockPhoneNumberEndPoint_Live");
                return _APFCOPayPalLockPhoneNumberEndPointLive;
            }
            set
            {
                _APFCOPayPalLockPhoneNumberEndPointLive = value;
            }
        }
        private string _APFCOPayPalLockPhoneNumberEndPointLive;

        public string APFCOPayPalLockPhoneNumberEndPointStage
        {
            get
            {
                if (_APFCOPayPalLockPhoneNumberEndPointStage == null)
                    _APFCOPayPalLockPhoneNumberEndPointStage = GetVariable("APFCOPayPalLockPhoneNumberEndPoint_Stage");
                return _APFCOPayPalLockPhoneNumberEndPointStage;
            }
            set
            {
                _APFCOPayPalLockPhoneNumberEndPointStage = value;
            }
        }
        private string _APFCOPayPalLockPhoneNumberEndPointStage;

        public string ApfEmailApiUserNameStage
        {
            get
            {
                if (_ApfEmailApiUserNameStage == null)
                    _ApfEmailApiUserNameStage = GetVariable("apfEmailApiUserName_Stage");
                return _ApfEmailApiUserNameStage;
            }
            set
            {
                _ApfEmailApiUserNameStage = value;
            }
        }
        private string _ApfEmailApiUserNameStage;

        public string ApfEmailApiUserNameLive
        {
            get
            {
                if (_ApfEmailApiUserNameLive == null)
                    _ApfEmailApiUserNameLive = GetVariable("apfEmailApiUserName_Live");
                return _ApfEmailApiUserNameLive;
            }
            set
            {
                _ApfEmailApiUserNameLive = value;
            }
        }
        private string _ApfEmailApiUserNameLive;

        public string APFCOEmailApiEndPointLive
        {
            get
            {
                if (_APFCOEmailApiEndPointLive == null)
                    _APFCOEmailApiEndPointLive = GetVariable("apfEmailApiEndpointUrl_Live");
                return _APFCOEmailApiEndPointLive;
            }
            set
            {
                _APFCOEmailApiEndPointLive = value;
            }
        }
        private string _APFCOEmailApiEndPointLive;

        public string APFCOEmailApiEndPointStage
        {
            get
            {
                if (_APFCOEmailApiEndPointStage == null)
                    _APFCOEmailApiEndPointStage = GetVariable("apfEmailApiEndpointUrl_Stage");
                return _APFCOEmailApiEndPointStage;
            }
            set
            {
                _APFCOEmailApiEndPointStage = value;
            }
        }
        private string _APFCOEmailApiEndPointStage;

        //Methods
        public ApfVariablesService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string GetVariable(string name)
        {
            return UTIL.GetApfVariable(configuration.GetConnectionString("Support"), name);
        }
    }
}
