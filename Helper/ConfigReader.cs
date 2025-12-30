using Menu4Tech.Configuration;
using SmartWinners.Helpers;

namespace Menu4Tech.Helper;

public class ConfigReader
{
    public static bool ReadFromJsonConfig<T>(out T config, bool throwError = false) where T : MyConfiguration
    {
        try
        {
            var configManager = new ConfigurationBuilder().SetBasePath($"{EnvironmentHelper.Environment.WebRootPath}\\appsettings\\").AddJsonFile(EnvironmentHelper.ConfigName).Build();

            var jsonSection = configManager.GetSection(typeof(T).Name);

            config = jsonSection.Get<T>();

            return true;
        }
        catch (Exception e)
        {
            config = default;

            if (throwError)
                throw e;

            return false;
        }
    }
}