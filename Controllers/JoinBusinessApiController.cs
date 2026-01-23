using System.Drawing;
using System.Globalization;
using System.ServiceModel;
using Azure;
using Menu4Tech.Helper;
using Menu4Tech.IBusinessAPIservice;
using Menu4Tech.Models;
using Menu4Tech.Models.Api;

using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Common.UmbracoContext;
using Umbraco.Cms.Web.Website.Controllers;

namespace Menu4Tech.Controllers;

[Route("JoinBusinessApi")]
public class JoinBusinessApiController : Controller
{

    [HttpPost]
    [Route("{action}")]
    public IActionResult TryFindUser([FromBody] LoginModel model)
    {
        var existingUser = UserManager.Get(new Dictionary<string, string>{{"Mobile", model.Mobile}, {"Country", model.Country.ToUpper()}});

        var entityId = existingUser?.Id;

        if (existingUser is null)
        {
            entityId = UserManager.Add("", "", "", model.Mobile, model.Country).EntityId;

            /*var config = EnvironmentHelper.BusinessApiConfiguration;
            
            var apiRequest = new General_Business_UpdateRequest
            {
                ol_Username = "website-admin",
                ol_Password = "website-admin",
                ol_EntityID = 6173,
                BusinessId = 0,
                NamesArray = new[]
                {
                    "EntityId",
                    "BusinessName",
                    "EIN",
                    "Address",
                    "City"
                },
                ValuesArray = new[]
                {
                    entityId.ToString(),
                    "",
                    "",
                    "",
                    ""
                }
            };

            var apiResponse = config.InitClient().General_Business_Update(apiRequest);*/
        }

        if (existingUser is null || !existingUser.MobileVerified)
        {
            var verifyResp = UserManager.SendVerifyCode(entityId!.Value);
            
            Cooking.SetCookie(Cooking.SignInUserData, new UserSignInData
            {
                Country = model.Country,
                Mobile = model.Mobile,
                PhonePrefix = model.PhonePrefix
            });
            
            Response.Headers.Add("Location", TranslationHelper.OverridePathWithCurrentCulture("/JoinBusiness/Verification"));
            return Ok();
        }
        
        Cooking.SetCookie(Cooking.SignInUserData, new UserSignInData
        {
            Country = model.Country,
            Mobile = model.Mobile,
            PhonePrefix = model.PhonePrefix
        });
        
        Cooking.SetCookie(Cooking.LastUsedSignInIso, model.Country);
        
        Response.Headers.Add("Location", TranslationHelper.OverridePathWithCurrentCulture("/JoinBusiness/Login"));
        return Ok();
    }
    
    [HttpPost]
    [Route("{action}")]
    public IActionResult TryLogin([FromBody] LoginModel model)
    {
        if(string.IsNullOrEmpty(model.Password))
            return BadRequest(TranslationHelper.GetTranslatedMessage("Password isn't provided"));
        
        var existingUser = UserManager.Get(new Dictionary<string, string>{{"Mobile", model.Mobile}, {"Country", model.Country.ToUpper()}});
        
        if (existingUser is null)
            return BadRequest(TranslationHelper.GetTranslatedMessage("User not found"));

        var loginResponse = UserManager.Login(existingUser.Username, model!.Password);
        
        if (!loginResponse!.IsSuccess)
            return BadRequest(TranslationHelper.GetTranslatedMessage("User not found"));

        existingUser.Lid = loginResponse.Lid;
        
        Cooking.SetCookie(Cooking.MainUserData, existingUser);
        
        Response.Headers.Add("Location", TranslationHelper.OverridePathWithCurrentCulture("/"));
        return Ok();
    }
    
    [HttpPost]
    [Route("{action}")]
    public IActionResult TryVerifyPhone([FromBody] VerifyPhoneModel model)
    {
        if(string.IsNullOrEmpty(model.VerificationCode))
            return BadRequest(TranslationHelper.GetTranslatedMessage("Verification code isn't provided"));
        
        var existingUser = UserManager.Get(new Dictionary<string, string>{{"Mobile", model.Mobile}, {"Country", model.Country.ToUpper()}});
        
        if (existingUser is null)
            return BadRequest(TranslationHelper.GetTranslatedMessage("User not found"));

        var verifyResponse = UserManager.VerifyPhone(existingUser.Id, model.VerificationCode);
        
        if (verifyResponse.ResultCode == -14)
            return BadRequest(TranslationHelper.GetTranslatedMessage("Verification code is incorrect"));
        
        if (!verifyResponse.IsSuccess)
            return BadRequest(TranslationHelper.GetTranslatedMessage("We have encountered a problem verifying the phone number"));

        var loginResponse = UserManager.Login(existingUser.Username, existingUser!.Password);
        
        existingUser.Lid = loginResponse.Lid;
        
        Cooking.SetCookie(Cooking.MainUserData, existingUser);
        
        Response.Headers.Add("Location", TranslationHelper.OverridePathWithCurrentCulture("/"));
        return Ok();
    }
    
    [HttpPost]
    [Route("{action}")]
    public IActionResult SendAgain([FromBody] VerifyPhoneModel model)
    {
        var existingUser = UserManager.Get(new Dictionary<string, string>{{"Mobile", model.Mobile}, {"Country", model.Country.ToUpper()}});
        
        if (existingUser is null)
            return BadRequest(TranslationHelper.GetTranslatedMessage("User not found"));

        var verifyResponse = UserManager.SendVerifyCode(existingUser.Id);

        if (verifyResponse.ResultCode == -17)
            return BadRequest(TranslationHelper.GetTranslatedMessage("Verification code already sent"));
        
        if (!verifyResponse.IsSuccess)
            return BadRequest(TranslationHelper.GetTranslatedMessage("We have encountered a problem trying to send verification code"));

        return Ok(TranslationHelper.GetTranslatedMessage("Verification code have been sent"));
    }
    
    [HttpPost]
    [Route("{action}")]
    public IActionResult ForgotPassword([FromBody] LoginModel model)
    {
        var existingUser = UserManager.Get(new Dictionary<string, string>{{"Mobile", model.Mobile}, {"Country", model.Country.ToUpper()}});
        
        if (existingUser is null)
            return BadRequest(TranslationHelper.GetTranslatedMessage("User not found"));

        var verifyResponse = UserManager.SendForgotPassword(existingUser.Username, Cooking.GetCookie(Cooking.CurrentCulture, false));
        
        if (!verifyResponse.IsSuccess)
            return BadRequest(TranslationHelper.GetTranslatedMessage("We have encountered a problem trying to send restore message"));

        return Ok(TranslationHelper.GetTranslatedMessage("Message have been sent"));
    }
    
    [HttpPost]
    [Route("{action}")]
    public IActionResult ChangePasswordAndLogin([FromBody] ChangePasswordModel model)
    {
        var existingUser = UserManager.Get(new Dictionary<string, string>{{"ol_username", model.Username}});
        
        if (existingUser is null)
            return BadRequest(TranslationHelper.GetTranslatedMessage("User not found"));

        var verifyResponse = UserManager.UpdatePassword(existingUser.Username, Cooking.GetCookie(Cooking.CurrentCulture, false), model.Token, model.Password);
        
        if (!verifyResponse.IsSuccess)
            return BadRequest(TranslationHelper.GetTranslatedMessage("We have encountered a problem trying to change password"));

        Cooking.SetCookie(Cooking.MainUserData, existingUser);
        Response.Headers.Add("Location", TranslationHelper.OverridePathWithCurrentCulture("/"));
        return Ok(TranslationHelper.GetTranslatedMessage("Password have been changed"));
    }

    [HttpPost]
    [Route("{action}")]
    public IActionResult CreateCompany([FromBody] JoinBusinessRequest model)
    {
        var config = EnvironmentHelper.BusinessApi2Configuration;
        var currentUser = Cooking.GetCookie<User>(Cooking.MainUserData);

        var client = config.InitClient();
        
        var businessId = JsonConvert.DeserializeObject<GeneralBusinessGetResponse>(client.General_Business_Get(new General_Business_GetRequest
        {
            ol_Password = currentUser.Password,
            ol_UserName = currentUser.Username,
            ol_EntityId = currentUser.Id,
        }).@return.Replace("[","").Replace("]",""))?.BusinessId;
        // if (businessId != null)
        // {
        //     client.Entity_Update(new Entity_UpdateRequest
        //     {
        //         ol_Password = config.ol_Password,
        //         ol_Username = config.ol_UserName,
        //         ol_EntityID = config.ol_EntityId,
        //         EntityId = currentUser.Id,
        //         NamesArray = new[]
        //         {
        //             "isBusiness",
        //         },
        //         ValuesArray = new[]
        //         {
        //             "1",
        //         }
        //     });
        //     client.General_Business_Update(new General_Business_UpdateRequest
        //     {
        //         ol_Password = config.ol_Password,
        //         ol_Username = config.ol_UserName,
        //         ol_EntityID = config.ol_EntityId,
        //         BusinessId = businessId ?? 1,
        //         NamesArray = new[]
        //         {
        //             "entityId",
        //             "BusinessName",
        //             "Address",
        //             "City",
        //         },
        //         ValuesArray = new[]
        //         {
        //             $"{currentUser.Id}",
        //             model.CompanyName,
        //             model.BrandAddress,
        //             model.City,
        //         }
        //     });
        // }
        // if (businessId == null)
        // {
            var resp1 = client.General_Business_Update(new General_Business_UpdateRequest
            {
                ol_Password = config.ol_Password,
                ol_Username = config.ol_UserName,
                ol_EntityID = config.ol_EntityId,
                BusinessId = 0,
                NamesArray = new[]
                {
                    "ein",
                    "BusinessName",
                    "Address",
                    "City",
                    "entityId",
                    "CurrencyISO_default",
                    "languagesISO"
                },
                ValuesArray = new[]
                {
                    $"{currentUser.Id}",
                    model.CompanyName,
                    model.BrandAddress,
                    model.City,
                    $"{currentUser.Id}",
                    "ILS",
                    "ILS"
                }
            });
        //}
        return Ok();
    }
    
}