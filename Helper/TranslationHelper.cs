using System.Globalization;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Common.UmbracoContext;

namespace Menu4Tech.Helper;

public static  class TranslationHelper
{
    public static string? GetTranslatedMessage(string key)
    {
        return key;
        var umbracoHelperAccessor = EnvironmentHelper.DiContainer.GetRequiredService<IUmbracoHelperAccessor>(); //todo fix
        umbracoHelperAccessor.TryGetUmbracoHelper(out var umbracoHelper);
        
        var initialThreadLang = Thread.CurrentThread.CurrentCulture;
        
        var currentLang = GetCurrentLang();

        var pageCultureInfo = ConvertIsoToCulture(currentLang);
        
        Thread.CurrentThread.CurrentCulture = pageCultureInfo;
        Thread.CurrentThread.CurrentUICulture = pageCultureInfo;

        var message = umbracoHelper.GetDictionaryValue(key);
        
        Thread.CurrentThread.CurrentCulture = initialThreadLang;
        Thread.CurrentThread.CurrentUICulture = initialThreadLang;
        
        return message;
    }

    public static string OverridePathWithCurrentCulture(string path)
    {
        var currentLang = GetCurrentLang();
        
        return currentLang.ToLower() == "en" || path.Contains("blog") ? path : $"/{currentLang}{(path.StartsWith("/") ? path : $"/{path}")}";
    }
    public static string? GetCurrentLang()
    {
        var currentLang = Cooking.GetCookie(Cooking.CurrentCulture, false);

        if (string.IsNullOrEmpty(currentLang))
        {
            var path = EnvironmentHelper.HttpContextAccessor.HttpContext.Request.Path;

            if (path.StartsWithSegments("/he"))
                currentLang = "he";
            else if(path.StartsWithSegments("/ru"))
                currentLang = "ru";
            else
                currentLang = "en";
        }
        
        return currentLang;
    }
    
    private static CultureInfo ConvertIsoToCulture(string twoLetterIso)
    {
        if (string.IsNullOrWhiteSpace(twoLetterIso))
            throw new ArgumentException("ISO code cannot be null or empty.", nameof(twoLetterIso));

        try
        {
            var culture = CultureInfo
                .GetCultures(CultureTypes.AllCultures)
                .FirstOrDefault(c => c.TwoLetterISOLanguageName.Equals(twoLetterIso, StringComparison.OrdinalIgnoreCase));

            if (culture == null)
                throw new CultureNotFoundException($"No culture found for ISO code '{twoLetterIso}'.");

            return culture;
        }
        catch (CultureNotFoundException)
        {
            throw;
        }
    }
}