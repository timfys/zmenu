using Menu4Tech.Configuration;
using SmartWinners.Helpers;

namespace Menu4Tech.Helper;

public class EnvironmentHelper
{
    public static IWebHostEnvironment Environment { get; set; }

    public static IHttpContextAccessor HttpContextAccessor => DiContainer.GetService<IHttpContextAccessor>();
    public static string ConfigName
    {
        get
        {
            var context = HttpContextAccessor.HttpContext;
            return "ApiConfig.json";
        }
    }   
    public static BusinessApiConfiguration _businessApiConfiguration;
    public static BusinessApiConfiguration BusinessApiConfiguration
    {
        get
        {
            if (_businessApiConfiguration is null)
            {
                ConfigReader.ReadFromJsonConfig<BusinessApiConfiguration>(out var config);

                _businessApiConfiguration = config;
            }

            return _businessApiConfiguration;
        }
    }
    public static BusinessApi2Configuration _businessApi2Configuration;
    public static BusinessApi2Configuration BusinessApi2Configuration
    {
        get
        {
            if (_businessApi2Configuration is null)
            {
                ConfigReader.ReadFromJsonConfig<BusinessApi2Configuration>(out var config);

                _businessApi2Configuration = config;
            }

            return _businessApi2Configuration;
        }
    }
    public static SeoConfiguration _seoConfiguration;
    
    public static SeoConfiguration SeoConfiguration
    {
        get
        {
            if (_seoConfiguration is null)
            {
                ConfigReader.ReadFromJsonConfig<SeoConfiguration>(out var config);

                _seoConfiguration = config;
            }

            return _seoConfiguration;
        }
    }

    public static ServiceProvider DiContainer { get; set; }
}