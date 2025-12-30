using Menu4Tech.Models.Interfaces;

namespace Menu4Tech.Services
{
    public class CredentialService : ICredential
    {
        private readonly IConfiguration _configuration;

        public CredentialService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetUserName()
        {
            if (_configuration["use_passwod"] != null && bool.Parse(_configuration["use_passwod"]) == true)
            {
                return _configuration["Ol_Username"];
            }
            return string.Empty;
        }

        public string GetPassword()
        {
            if (_configuration["use_passwod"] != null && bool.Parse(_configuration["use_passwod"]) == true)
            {
                return _configuration["Ol_Password"];
            }
            return string.Empty;
        }
        public string GetWebServiceUrl()
        {
            if (string.IsNullOrWhiteSpace(_configuration["WebServiceUrl"]))
            {
                return string.Empty;
            }
            return _configuration["WebServiceUrl"];
        }

        public string GetEnv()
        {
            if (string.IsNullOrWhiteSpace(_configuration["isDevEnv"]))
            {
                return string.Empty;
            }
            return _configuration["isDevEnv"];
        }
        public string GetEntityId()
        {
            if (_configuration["Ol_EntityId"] != null)
            {
                return _configuration["Ol_EntityId"];
            }
            return string.Empty;
        }
    }
}
