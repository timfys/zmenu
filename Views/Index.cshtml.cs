using System.Globalization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Localization;

namespace Menu4Tech.Views
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly ITempDataDictionaryFactory _tempDataDictionaryFactory;
        private readonly IStringLocalizer<IndexModel> _stringLocalizer;


        public IndexModel(ILogger<IndexModel> logger,
            IHttpContextAccessor httpContextAccessor, ITempDataDictionaryFactory tempDataDictionaryFactory,
            IStringLocalizer<IndexModel> stringLocalizer)
        {
            _httpContextAccessor = httpContextAccessor;
            var culture = "en";

            if (_httpContextAccessor.HttpContext.Request.Path != null &&
                !_httpContextAccessor.HttpContext.Request.Path.Value.Contains("Files"))
            {
                var parameters = _httpContextAccessor.HttpContext.Request.Path.ToString().Split("/");
                if (parameters == null || parameters.Length < 1) return;

                // Culture should be en by default, ignore it if no culture is passed
                if (parameters.Length > 3) 
                    culture = (parameters.Length >= 2) ? parameters[1] : "en";
            }

            if (!string.IsNullOrWhiteSpace(culture))
            {
                CultureInfo.CurrentCulture = new CultureInfo(culture);
                CultureInfo.CurrentUICulture = new CultureInfo(culture);

            }
            _logger = logger;
            _tempDataDictionaryFactory = tempDataDictionaryFactory;
            _stringLocalizer = stringLocalizer;
            var httpContext = _httpContextAccessor.HttpContext;

            var tempData = _tempDataDictionaryFactory.GetTempData(httpContext);

            // use tempData as usual
            tempData["FooterText"] = _stringLocalizer["FooterText"].Value;

        }

        public void OnGet()
        {

        }
    }
}
