using System.Net;
using Menu4Tech.Helper;
using Menu4Tech.IBusinessAPIservice;
using Menu4Tech.Models;
using Menu4Tech.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Serilog;

namespace Menu4Tech.Controllers
{
    // Todo Add Exception Middleware
    public class MenuController : Controller
    {


        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStringLocalizer<MenuController> _stringLocalizer;
        private readonly ITempDataDictionaryFactory _tempDataDictionaryFactory;
        private readonly ILogRequest _logRequest;
        private readonly ICredential _credential;
        private readonly IMenu _menu;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public MenuController(ICredential credential, IHttpContextAccessor httpContextAccessor,
                IStringLocalizer<MenuController> stringLocalizer,
                ITempDataDictionaryFactory tempDataDictionaryFactory, ILogRequest logRequest, IMenu menu, IWebHostEnvironment webHostEnvironment)
        {


            _httpContextAccessor = httpContextAccessor;
            _tempDataDictionaryFactory = tempDataDictionaryFactory;
            _stringLocalizer = stringLocalizer;
            _logRequest = logRequest;
            _credential = credential;
            _menu = menu;
            _webHostEnvironment = webHostEnvironment;
            var culture = "ENG";

            if (_httpContextAccessor.HttpContext.Request.Path != null &&
                !_httpContextAccessor.HttpContext.Request.Path.Value.Contains("Files"))
            {
                var parameters = _httpContextAccessor.HttpContext.Request.Path.ToString().Split("/");
                if (parameters == null || parameters.Length < 1) return;

                // Culture should be en by default, ignore it if no culture is passed
                if (parameters.Length > 5)
                    culture = parameters[5];
            }




            var tempData = _tempDataDictionaryFactory.GetTempData(_httpContextAccessor.HttpContext);

            // use tempData as usual
            tempData["FooterText"] = _stringLocalizer["FooterText"].Value;
            _menu = menu;
        }

        [Route("[controller]/GetEntityNameAndAddres/{entityId}/{businessId}/")]
        public async Task<Entity> GetEntityNameAndAddres(int entityId, int businessId)
        {
            string contentPath = _webHostEnvironment.ContentRootPath;
            var folderName = Path.Combine(contentPath, "Files", businessId.ToString()) + @"\"; //$@"Files\{businessId}\";

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
            else
            {
                var config = EnvironmentHelper.BusinessApiConfiguration;
                var client = config.InitClient();
                
                var result = await client.Entity_FindAsync(new Entity_FindRequest(entityId, userName, password, businessId, false, new string[] { "Company", "Address", "City", "Zip", "Country", "Phone" },
                    new string[] { "entityID" }, new string[] { entityId.ToString() }, 0, 0));
                if (!Directory.Exists(folderName))
                {
                    Directory.CreateDirectory(folderName);
                }
                var entityDetailsObj = JsonConvert.DeserializeObject<IList<Entity>>(result.@return);
                using (StreamWriter file = System.IO.File.CreateText(folderName + "Entity" + ".json"))
                {
                    JsonSerializer serializer = new JsonSerializer();

                    //serialize object directly into file stream
                    serializer.Serialize(file, entityDetailsObj);
                }

                returnResult = entityDetailsObj[0];
            }
            ViewBag.MetaDescription = returnResult.Address + "" + returnResult.City;
            return returnResult;
        }
        
        [Route("[controller]/GetMenu")]
        public async Task<ActionResult> GetMenu(int businessId, int entityId, string culture)
        {
            TempData["FooterText"] = _stringLocalizer["FooterText"].Value;

            if (businessId == 0 || entityId == 0)
            {
                return new ContentResult { StatusCode = (int)HttpStatusCode.BadRequest };
                //return string.Empty;
            }
            if (string.IsNullOrWhiteSpace(culture))
                culture = "ENG";

            var contentPath = _webHostEnvironment.ContentRootPath;

            var folderName = $@"Files\{businessId}\";
            var businessFolderPath = $"{contentPath}\\{folderName}".TrimEnd('\\');
            if (!Directory.Exists(businessFolderPath))
                Directory.CreateDirectory(businessFolderPath);
            var folderPath = Path.Combine(contentPath, folderName);
            MenuResponse menuResponse = new MenuResponse();
            try
            {
                if (System.IO.File.Exists(folderPath + "Menu" + culture + ".json"))
                {
                    menuResponse = await _menu.GenerateMenuJsonAsync(entityId, businessId, folderName, culture);
                }
                else
                {
                    // Generate Menu
                    menuResponse = await _menu.GenerateMenuAsync(entityId, businessId, folderName, culture);
                }               

                //return menuResponse.Content;
                return Ok(menuResponse);
            }
            catch (Exception ex)
            {
                if (System.IO.File.Exists(folderPath + "Menu" + culture + ".html"))
                {
                    menuResponse = _menu.ReadHTML(folderPath, culture);
                    return Ok(menuResponse);
                    //return string.Empty;
                }
                // Log it and should not throw error
                menuResponse.Content = $"<div class=\"alert alert-danger\">Menu doesn't exist or could not be loaded. Reason: {ex.Message}</div>";
                menuResponse.IsUpdated = true;
                Log.Fatal(ex.Message.ToString());
                return Ok(menuResponse);
            }
        }



    }
}
