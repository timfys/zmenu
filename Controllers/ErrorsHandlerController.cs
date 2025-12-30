using Menu4Tech.Configuration;
using Menu4Tech.Helper;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SmartWinners.Helpers;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Website.Controllers;

namespace Menu4Tech.Controllers;

[Route("Error")]
public class ErrorsHandlerController : SurfaceController
{
    private readonly IWebHostEnvironment _environment;

    public ErrorsHandlerController(IUmbracoContextAccessor umbracoContextAccessor,
        IUmbracoDatabaseFactory databaseFactory, ServiceContext services, AppCaches appCaches,
        IProfilingLogger profilingLogger, IPublishedUrlProvider publishedUrlProvider, IWebHostEnvironment environment) :
        base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
    {
        _environment = environment;
    }

    [Route("Handler")]
    public IActionResult Index()
    {
        var context = HttpContext.Features.Get<IExceptionHandlerFeature>();

        if (context is null)
            return NotFound();

        Response.Cookies.Append("finwion98nf29o238fn2943nf8793bf43845bf8", "n32fo2ui3fn2390fn43u2fn42032fn302fn32n");

        var langIso = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

        FileLogger.Log("500ErrorLog.txt", context.Error.ToString());
        
        langIso = langIso.Equals("en") ? "" : $"/{langIso}";
        
        /*return Content($"{context.Error.Message}\n {context.Error.StackTrace} \n {context.Error.InnerException?.Message}\n {context.Error.InnerException?.StackTrace}");*/
        
        if (ConfigReader.ReadFromJsonConfig<SmtpClientConfiguration>(out var configuration))
        {
            if (SmtpSender.Send(configuration, $"Message: {context.Error.Message} \n\n Stack Trace: {context.Error.StackTrace}", false))
            {
                return Redirect($"{langIso}/error");
            }
        }

        if (HttpContext.Request.Headers.Any(x => x.Key == "X-Fetch-Indicator"))
        {
            return BadRequest(TranslationHelper.GetTranslatedMessage("Error Page"));
        }
        
        return Redirect($"{langIso}/error");
    }

}