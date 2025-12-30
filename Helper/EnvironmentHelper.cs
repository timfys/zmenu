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
            return context.Request.Host.Value.Contains("beta") || context.Request.Host.Value.Contains("89.23.5.24") ? "ApiConfig.json" : "ApiConfig-Live.json";
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