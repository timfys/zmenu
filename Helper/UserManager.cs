using System.Net;
using System.ServiceModel;
using Menu4Tech.IBusinessAPIservice;
using Menu4Tech.Models;
using Menu4Tech.Models.Api;
using Newtonsoft.Json;

namespace Menu4Tech.Helper;

public static class UserManager
{

    public static string GetIp(HttpContext context)
    {
        var ip = context.Request.HttpContext.Connection.RemoteIpAddress.ToString();

        return ip;
    }

    public static User? Get(Dictionary<string, string> filter)
    {
        var config = EnvironmentHelper.BusinessApiConfiguration;
        
        var client = config.InitClient();

        var resp = client.Entity_Find(new Entity_FindRequest(config.ol_EntityId, config.ol_UserName, config.ol_Password,
            config.BusinessId, false, User.GetFields().ToArray(), filter.Keys.ToArray(), filter.Values.ToArray(), 0, 0));

        try
        {
            return JsonConvert.DeserializeObject<List<User>>(resp.@return)?.FirstOrDefault();
        }
        catch (Exception e)
        {
            return null;
        }
    }
    
    public static EntityLoginResponse? Login(string username, string password)
    {
        var config = EnvironmentHelper.BusinessApiConfiguration;
        
        var client = config.InitClient();

        var resp = client.Ol_login(new Ol_loginRequest(username, password, GetUserIp(), GetPageCulture(), 4, null, null));

        return JsonConvert.DeserializeObject<EntityLoginResponse>(resp.@return);
    }
    
    public static EntityAddResponse Add(string? firstName, string? lastName, string? email, string phone, string country, string? password = null)
    {
        var config = EnvironmentHelper.BusinessApiConfiguration;
        
        var client = config.InitClient();
        
        var resp = client.Entity_Add(new Entity_AddRequest
        {
            FirstName = firstName,
            LastName = lastName,
            Mobile = phone,
            Password = string.IsNullOrEmpty(password) ? new Random().Next(100000, 999999).ToString() : password,
            CountryISO = country.ToUpper(),
            CategoryID = 8,
            Email = email,
            ol_Password = config.ol_Password,
            ol_UserName = config.ol_UserName,
            ol_EntityId = config.ol_EntityId,
            BusinessId = config.BusinessId,
        });

        return JsonConvert.DeserializeObject<EntityAddResponse>(resp.@return)!;
    }

    public static Response VerifyPhone(int entityId, string verificationCode)
    {
        var config = EnvironmentHelper.BusinessApiConfiguration;
        
        var client = config.InitClient();

        var resp = client.Entity_VerifyContactInfo(new Entity_VerifyContactInfoRequest(config.ol_EntityId,
            config.ol_UserName, config.ol_Password, config.BusinessId, entityId, 0, verificationCode));
        
        return JsonConvert.DeserializeObject<Response>(resp.@return)!;
    }
    
    public static Response SendVerifyCode(int entityId)
    {
        var config = EnvironmentHelper.BusinessApiConfiguration;
        
        var client = config.InitClient();

        var resp = client.Entity_VerifyContactInfo(new Entity_VerifyContactInfoRequest(config.ol_EntityId,
            config.ol_UserName, config.ol_Password, config.BusinessId, entityId, 0, null));
        
        return JsonConvert.DeserializeObject<Response>(resp.@return)!;
    }
    
    public static Response SendForgotPassword(string userName, string language)
    {
        var config = EnvironmentHelper.BusinessApiConfiguration;
        
        var client = config.InitClient();

        var resp = client.Entity_Forgotpassword(new Entity_ForgotpasswordRequest(userName,
            ForgotPasswordRemindKings.Sms.ToInt(), language, config.BusinessId, null, null, null, null));
        
        return JsonConvert.DeserializeObject<Response>(resp.@return)!;
    }
    
    public static Response UpdatePassword(string userName, string language, string token, string newPassword)
    {
        var config = EnvironmentHelper.BusinessApiConfiguration;
        
        var client = config.InitClient();

        var resp = client.Entity_Forgotpassword(new Entity_ForgotpasswordRequest(userName,
            ForgotPasswordRemindKings.Sms.ToInt(), language, config.BusinessId, token, newPassword, null, null));
        
        return JsonConvert.DeserializeObject<Response>(resp.@return)!;
    }
    
    public static string GetUserIp()
    {
        var httpContext = EnvironmentHelper.HttpContextAccessor.HttpContext;

        if (httpContext.Request.Host.Value.Contains("89.23.5.24"))
        {
            return httpContext.Connection.RemoteIpAddress.ToString();
        }
            
        return httpContext.Request.Headers["CF-Connecting-IP"];
    }
    
    public static string GetUserCountryIsoFromCloudFlare()
    {
        var httpContext = EnvironmentHelper.HttpContextAccessor.HttpContext;

        if (httpContext.Request.Host.Value.Contains("89.23.5.24"))
        {
            return "US";
        }
            
        return httpContext.Request.Headers["CF-IPCountry"];
    }
    
    public static string GetPageCulture() =>  TranslationHelper.GetCurrentLang();
}

public enum ForgotPasswordRemindKings
{
    Email,
    Sms
}