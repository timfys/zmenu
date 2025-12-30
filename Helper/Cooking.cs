using System.Text;
using Newtonsoft.Json;

namespace Menu4Tech.Helper;

public static class Cooking
{
    public const string CurrentCulture = "Culture";
    public const string SignInUserData = "SData";
    public const string MainUserData = "MData";
    public const string LastUsedSignInIso = "SIso";
    public static void SetCookie(string key, string value)
    {
        EnvironmentHelper.HttpContextAccessor.HttpContext.Response.Cookies.Append(key, Convert.ToBase64String(Encoding.UTF8.GetBytes(CryptoUtility.EncryptString(value))));
    }

    public static void SetCookie(string key, object value)
    {
        if(!value.GetType().IsClass)
            throw new ArgumentException("value must be a class in order to append cookie");
        
        EnvironmentHelper.HttpContextAccessor.HttpContext.Response.Cookies.Append(key, Convert.ToBase64String(Encoding.UTF8.GetBytes(CryptoUtility.EncryptString(JsonConvert.SerializeObject(value)))));
    }

    public static string? GetCookie(string key, bool secured = true)
    {
        var cookies = EnvironmentHelper.HttpContextAccessor.HttpContext.Request.Cookies;
        
        return !secured ? cookies[key] : cookies.TryGetValue(key, out var value) ? CryptoUtility.DecryptString(Encoding.UTF8.GetString(Convert.FromBase64String(value!))) : null;
    }
    
    public static T? GetCookie<T>(string key) where T : class
    {
        return EnvironmentHelper.HttpContextAccessor.HttpContext.Request.Cookies.TryGetValue(key, out var value) ? JsonConvert.DeserializeObject<T>(CryptoUtility.DecryptString(Encoding.UTF8.GetString(Convert.FromBase64String(value!)))) : default;
    }

    public static void Delete(string key)
    {
        EnvironmentHelper.HttpContextAccessor.HttpContext.Response.Cookies.Delete(key);
    }
}