using System.Drawing;
using System.Globalization;
using System.ServiceModel;
using Menu4Tech.Helper;
using Menu4Tech.IBusinessAPIservice;
using Menu4Tech.Models;
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

[Route("JoinBusiness")]
public class JoinBusinessController : SurfaceController
{
    private readonly IWebHostEnvironment _environment;

    public JoinBusinessController(IUmbracoContextAccessor umbracoContextAccessor,
        IUmbracoDatabaseFactory databaseFactory, ServiceContext services, AppCaches appCaches,
        IProfilingLogger profilingLogger, IPublishedUrlProvider publishedUrlProvider, IWebHostEnvironment environment) :
        base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
    {
        _environment = environment;
    }
    

    [HttpGet]
    [Route("/{langIso}/{controller}/{action}")]
    [Route("/{controller}/{action}")]
    public IActionResult Logout()
    {
        Cooking.Delete(Cooking.MainUserData);
        return Redirect(TranslationHelper.OverridePathWithCurrentCulture("/"));
    }

    public IActionResult Index()
    {
        var currentPage = UmbracoContext.PublishedRequest.PublishedContent;

        return View("/Views/JoinBusiness.cshtml", currentPage);
    }

    [HttpGet]
    [Route("/{langIso}/{controller}/{action}")]
    [Route("/{controller}/{action}")]
    public IActionResult Verification()
    {
        var signInData = Cooking.GetCookie<UserSignInData>(Cooking.SignInUserData);

        if (signInData is null || string.IsNullOrEmpty(signInData.Country) || string.IsNullOrEmpty(signInData.Mobile))
        {
            Cooking.Delete(Cooking.SignInUserData);
            return Redirect(TranslationHelper.OverridePathWithCurrentCulture("/JoinBusiness/Login"));
        }
        var currentPage = UmbracoContext.PublishedRequest.PublishedContent;
        return View("/Views/Verification.cshtml", currentPage);
    }
    
    [HttpGet]
    [Route("/{langIso}/{controller}/{action}")]
    [Route("/{controller}/{action}")]

    public IActionResult Login()
    {
        var signInData = Cooking.GetCookie<UserSignInData>(Cooking.SignInUserData);

        var currentPage = UmbracoContext.PublishedRequest.PublishedContent;
        if (signInData is null || string.IsNullOrEmpty(signInData.Country) || string.IsNullOrEmpty(signInData.Mobile))
        {
            Cooking.Delete(Cooking.SignInUserData);
            return View("/Views/Login.cshtml", currentPage);
        }
        
        var existingUser = UserManager.Get(new Dictionary<string, string>{{"Mobile", signInData.Mobile}, {"Country", signInData.Country.ToUpper()}});

        if (existingUser is null)
        {
            Cooking.Delete(Cooking.SignInUserData);
            return View("/Views/Login.cshtml", currentPage);
        }

        if (!existingUser.MobileVerified)
        {
            return Redirect(TranslationHelper.OverridePathWithCurrentCulture("/JoinBusiness/Verification"));
        }

        return View("/Views/Password.cshtml", currentPage);
    }
    
    [HttpGet]
    [Route("/{langIso}/{controller}/{action}")]
    [Route("/{controller}/{action}")]

    public IActionResult SetPassword([FromQuery(Name = "t")] string token, [FromQuery(Name = "u")] string username)
    {
        var currentPage = UmbracoContext.PublishedRequest.PublishedContent;

        ViewBag.Token = token;
        ViewBag.Username = username;

        return View("/Views/NewPassword.cshtml", currentPage);
    }
    

    [NonAction]
    public DateTime ScheduleTime(string dateTime, int hoursToAdd, int timeZoneOffset)
    {
        timeZoneOffset /= 60;
        
        var localClientDateTime = DateTime.Parse(dateTime);

        var messageSendHour = localClientDateTime + TimeSpan.FromHours(2);

        if (messageSendHour.Hour >= 18)
        {
            messageSendHour = localClientDateTime + TimeSpan.FromDays(1);
            messageSendHour = new DateTime(messageSendHour.Year, messageSendHour.Month, messageSendHour.Day, 9, 0, 0);
            
        }else if (messageSendHour.Hour < 9)
        {
            messageSendHour = new DateTime(messageSendHour.Year, messageSendHour.Month, messageSendHour.Day, 9, 0, 0);
        }

        messageSendHour = messageSendHour + TimeSpan.FromHours(timeZoneOffset) + TimeSpan.FromHours(hoursToAdd);
        return messageSendHour;
    }
}