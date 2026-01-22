using System.Globalization;
using System.Text;
using Menu4Tech.Helper;
using Menu4Tech.Models;
using Menu4Tech.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Website.Controllers;
using IViewHelper = Menu4Tech.Models.Interfaces.IViewHelper;

namespace Menu4Tech.Controllers;

[Route("")]
public class HomeController : SurfaceController
{
    private readonly ICredential _credential;
    private readonly ILogRequest _logRequest;
    private readonly IMemoryCache _memoryCache;
    private readonly IStringLocalizer<HomeController> _stringLocalizer;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IViewHelper _viewHelper;
    public HomeController(IUmbracoContextAccessor umbracoContextAccessor, IUmbracoDatabaseFactory databaseFactory,
        ServiceContext services, AppCaches appCaches, IProfilingLogger profilingLogger,
        IPublishedUrlProvider publishedUrlProvider, IViewHelper viewHelper, IWebHostEnvironment webHostEnvironment, IStringLocalizer<HomeController> stringLocalizer, ILogRequest logRequest, IMemoryCache memoryCache, ICredential credential) : base(umbracoContextAccessor, databaseFactory, services, appCaches,
        profilingLogger, publishedUrlProvider)
    {
        _viewHelper = viewHelper;
        _webHostEnvironment = webHostEnvironment;
        _stringLocalizer = stringLocalizer;
        _logRequest = logRequest;
        _memoryCache = memoryCache;
        _credential = credential;
    }

    [HttpGet]
    [Route("/")]
    [Route("/ru")]
    [Route("/he")]
    [Route("/{page:int}")]
    [Route("/ru/{page:int}")]
    [Route("/he/{page:int}")]
    [Route("/digital-menus/{page:int}")]
    [Route("/ru/digital-menus/{page:int}")]
    [Route("/he/digital-menus/{page:int}")]
    public IActionResult Index([FromRoute] int? page)
    {
        string? culture = "";

        var currentPage = UmbracoContext.PublishedRequest.PublishedContent;

        if (HttpContext.Request.Host.Value.StartsWith("m."))
            return View("/Views/DigitalMenu.cshtml", currentPage);
        else if(HttpContext.Request.Host.Value.StartsWith("www."))
            return Redirect(TranslationHelper.OverridePathWithCurrentCulture("/Pos-For-a-Restaurant"));
        else
            return View("/Views/Home.cshtml", currentPage);
    }

    [Route("{culture}/{businessId:int}/{entityId:int}")]
    public async Task<IActionResult> Index(string culture, int businessId, int entityId)
    {
        //TempData["FooterText"] = _stringLocalizer["FooterText"].Value;
        //TempData["FooterLink"] = _stringLocalizer["FooterLink"].Value;

        LogEntity(entityId);

        if (culture == "he")
        {
            culture = "heb";
        }

        if (culture == "en")
        {
            culture = "eng";
        }

        if (culture == "ru")
        {
            culture = "rus";
        }

        HttpContext.Response.Cookies.Append("RestaurantUrl", $"/{culture}/{businessId}/{entityId}",
            new CookieOptions { Expires = DateTime.Now.AddDays(7) });
        _viewHelper.SetBusinessValues(entityId, businessId, culture);
        await SetCultureAndLocalization(culture, entityId);

        var metaData = GetEntityNameAndAddres(entityId, businessId);


        if (metaData.Result != null)
        {
            if (metaData.Result.Customfield1 != "")
            {
                ViewBag.MetaTitle = metaData.Result.Customfield1;
                ViewBag.MetaPageTitle = metaData.Result.Customfield1;
            }
            else
            {
                ViewBag.MetaTitle = metaData.Result.Company;
                ViewBag.MetaPageTitle = metaData.Result.Company;
            }

            ViewBag.MetaDescription = metaData.Result.Address + "," + metaData.Result.City + "," +
                                      metaData.Result.Country + "," + metaData.Result.Phone + "," + metaData.Result.Zip;
        }            
        else
        {
            ViewBag.MetaTitle = "";
            ViewBag.MetaPageTitle = "";
        }

        StringBuilder fileContent = new StringBuilder();

        var folderName = Path.Combine(_webHostEnvironment.ContentRootPath, $@"Files\{businessId}\");
        if (System.IO.File.Exists(folderName + "Menu" + culture + ".html"))
        {
            using (StreamReader streamReader =
                   new StreamReader(folderName + "Menu" + culture + ".html", System.Text.Encoding.UTF8))
            {
                string contents = await streamReader.ReadToEndAsync();
                fileContent.Append(contents);
                _memoryCache.Set("MenuHtml", contents);
                //menuHtml = contents;
            }

            ViewBag.PageData = fileContent.ToString();
        }

        return View();
    }
    public async Task<Entity> GetEntityNameAndAddres(int entityId, int businessId)
    {
        var folderName = $@"Files\{businessId}\";

        string userName = _credential.GetUserName();
        string password = _credential.GetPassword();
        Entity returnResult = null;
        if (System.IO.File.Exists(folderName + "Entity" + ".json"))
        {
            using (StreamReader reader = new StreamReader(folderName + "Entity" + ".json"))
            {
                string entityDetails = reader.ReadToEnd();

                var entityDetailsObj = JsonConvert.DeserializeObject<IList<Entity>>(entityDetails);

                returnResult = entityDetailsObj[0];
            }
        }
     
        return returnResult;
    }
    public async Task<bool> LogEntity(int entityId)
    {
        _logRequest.EntityLogAsync(entityId,"");
        return true;
    }

    private async Task SetCultureAndLocalization(string culture, int entityId)
    {
        var languages = await _viewHelper.GetLanguages(entityId);
        var cultureLang = languages.FirstOrDefault(x => x?.Iso?.ToUpper() == culture.ToUpper());
        if (cultureLang == null)
        {
            culture = "eng";
        }

        if (!string.IsNullOrWhiteSpace(culture))
        {
            CultureInfo.CurrentCulture = new CultureInfo(culture);
            CultureInfo.CurrentUICulture = new CultureInfo(culture);
        }

        _viewHelper.SetBusinessValues(entityId, _viewHelper.BusinessId, culture);

        var folderName = Path.Combine(_webHostEnvironment.ContentRootPath, $@"Files\{_viewHelper.BusinessId}\");
        if (System.IO.File.Exists(folderName + $@"\logo.jpg"))
        {
            ViewBag.LogoImg = "/Static/Files/" + _viewHelper.BusinessId + "/logo.jpg";
            ViewBag.LogoText = "none";
            ViewBag.LogoImageVis = "flex";
        }
        else
        {
            ViewBag.LogoImg = "/Static/Files/" + _viewHelper.BusinessId + "/logo.jpg";
            ViewBag.LogoText = "flex";
            ViewBag.LogoImageVis = "none";
        }

        ViewBag.Languages = languages;
        ViewBag.Entityid = entityId;
        ViewBag.BusinessId = _viewHelper.BusinessId;
        ViewBag.Culture = culture;
        ViewBag.FooterText1 = _stringLocalizer["FooterText1"].Value;
        ViewBag.FooterText = _stringLocalizer["FooterText"].Value;
        ViewBag.FooterLink = _stringLocalizer["FooterLink"].Value;
        ViewBag.JoinText = _stringLocalizer["JoinText"].Value;
        ViewBag.Logout = _stringLocalizer["Logout"].Value;
        ViewBag.MyBenefits = _stringLocalizer["MyBenefits"].Value;
        ViewBag.Menu = _stringLocalizer["Menu"].Value;
        ViewBag.LanguagesText = _stringLocalizer["LanguageText"].Value;
        ViewBag.Welcome = _stringLocalizer["Welcome"].Value;
    }
    public IActionResult About()
    {
        return View();
    }


    [HttpPost]
    public IActionResult CJoinBusiness()
    {
        throw new NotImplementedException();
    }
}