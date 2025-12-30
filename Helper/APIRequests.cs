using Menu4Tech.IBusinessAPIservice;
using Our.Umbraco.GMaps.Models;

namespace Menu4Tech.Helper;

public class EntityAddRequest
{
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Mobile { get; set; }
    public string Password { get; set; }
    public string CountryISO { get; set; }
    public string ConfirmPassword { get; set; }
    public string Culture { get; set; }
    public int? OrderProductId { get; set; }
}

public class ApiResponse
{
    public int ResultCode { get; set; }
    public string ResultMessage { get; set; }
    public int ExecuteTime { get; set; }
}

public class EntityAddResponse : ApiResponse
{
    public int EntityId { get; set; }
    public string ol_username { get; set; }
}

public class VerifyPhoneRequest
{
    public string Code { get; set; }
    public string Phone { get; set; }
}

public class ApiLoginResponse : ApiResponse
{
    public int isBusiness { get; set; }
    
    public int MobileConfirm { get; set; }
    
    public int EntityId { get; set; }
    public string Lid { get; set; }
}

public class JoinBusinessRequest
{
    public string CompanyName { get; set; }
    public string CompanyNumber { get; set; }
    public string BrandName { get; set; }
    public string BrandAddress { get; set; }
    public string City { get; set; }
    
    public string ScheduleDateTime { get; set; }
}

public class UserApiAccessData
{
    public int EntityId { get; set; }
    public string Password { get; set; }
    public string Username { get; set; }
}

public static class RequestMapper
{
    public static Entity_AddRequest Map(EntityAddRequest request)
    {
        return new Entity_AddRequest
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Mobile = request.Mobile,
            Password = request.Password,
            CountryISO = request.CountryISO,
            CategoryID = 8,
            Email = request.Email,
            ol_Password = "menu4u",
            ol_UserName = "menu4u",
            ol_EntityId = 6116,
            BusinessId = 1
        };
    }
}