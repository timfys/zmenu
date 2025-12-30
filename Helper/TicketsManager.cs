using System.ServiceModel;
using Menu4Tech.IBusinessAPIservice;
using Menu4Tech.Models;

namespace Menu4Tech.Helper;

public static class TicketsManager
{
    private static BusinessAPIClient _api;

    static TicketsManager()
    {
        _api = new BusinessAPIClient(BusinessAPIClient.EndpointConfiguration.IBusinessAPIPort,
            new EndpointAddress("http://lease4.mekashron.com:33322/soap/IBusinessAPI"));
    }

    public static Entity_VerifyContactInfoResponse CreateTicket(int entityId, string password, string username)
    {
        var apiRequest = new Entity_VerifyContactInfoRequest
        {
            entityID = entityId,
            businessId = 1,
            VerifyType = 0,
            ol_Username = username,
            ol_Password = password,
            ol_EntityID = entityId
        };

        return _api.Entity_VerifyContactInfo(apiRequest);
    }
    public static Entity_VerifyContactInfoResponse VerifyTicket(int entityId, string password, string username, string code)
    {
        var apiRequest = new Entity_VerifyContactInfoRequest
        {
            entityID = entityId,
            businessId = 1,
            VerifyType = 0,
            ol_Username = username,
            ol_Password = password,
            ol_EntityID = entityId,
            VerificationCode = code
        };

        return _api.Entity_VerifyContactInfo(apiRequest);
    }
}