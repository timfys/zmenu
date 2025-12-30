using System.Reflection;
using Menu4Tech.IBusinessAPIservice;
using Menu4Tech.MetaData;
using Newtonsoft.Json;

namespace Menu4Tech.Helper;

public class BusinessManager
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="entityId"></param>
    /// <param name="businessId">Optional param. If null then all businesses will be returned</param>
    public static List<Business>? GetEntityBusiness(int entityId, int? businessId = null)
    {
        var client = new BusinessAPIClient();

        var filter = new Dictionary<string, string>
        {
            {"b.EntityId", $"{entityId}"}
        };

        var apiRequest = new General_Business_GetRequest
        {
            ol_EntityId = 0,
            ol_UserName = "",
            ol_Password = ""
        };

        if (businessId.HasValue)
        {
            filter.Add("b.BusinessID", $"{businessId.Value}");
        }

        apiRequest.FilterFields = filter.Keys.ToArray();
        apiRequest.FilterValues = filter.Values.ToArray();

        var apiResponse = client.General_Business_Get(apiRequest);

        try
        {
            return JsonConvert.DeserializeObject<List<Business>>(apiResponse.@return);
        }
        catch
        {
            return null;
        }
    }

    public static List<Business>? GetBusiness(int? page = null)
    {
        var client = new BusinessAPIClient();

        var apiRequest = new General_Business_GetRequest
        {
            LimitCount = page.HasValue ? 21 : 0,
            LimitFrom = page.HasValue ? 20 * page.Value : 0,
            FilterFields = new[] {"Order by"},
            FilterValues = new[] {"e.EntityCreatedDate desc"}
        };

        var apiResponse = client.General_Business_Get(apiRequest);

        try
        {

            return JsonConvert.DeserializeObject<List<Business>>(apiResponse.@return);
        }
        catch
        {
            return new List<Business>();
        }
    }
}

public class Business
{
    [JsonProperty("entityID")] public int EntityId { get; set; }

    [JsonProperty("name")] public string Name { get; set; }

    [JsonProperty("address")] public string Address { get; set; }

    [JsonProperty("Mobile")] public string Mobile { get; set; }

    [JsonProperty("Phone")] public string Phone { get; set; }

    [JsonProperty("email")] public string Email { get; set; }

    [JsonProperty("CountryISO")] public string CountryIso { get; set; }

    [JsonProperty("CountryName")] public string CountryName { get; set; }

    [JsonProperty("zip")] public string Zip { get; set; }

    [JsonProperty("city")] public string City { get; set; }

    [JsonProperty("BusinessName")] public string BusinessName { get; set; }

    [JsonProperty("BusinessId")] public int BusinessId { get; set; }

    [JsonProperty("VatPCT")] public int VatPct { get; set; }

    [JsonProperty("EIN")] public string Ein { get; set; }

    [JsonProperty("languagesISO")] public string LanguagesIso { get; set; }

    [JsonProperty("CurrenciesISO")] public string CurrenciesIso { get; set; }

    [JsonProperty("Currency_rate_pct")] public int CurrencyRatePct { get; set; }

    [JsonProperty("CurrencyISO_default")] public string CurrencyIsoDefault { get; set; }

    [JsonProperty("DocumentLanguage")] public string DocumentLanguage { get; set; }

    [JsonProperty("profile_image")] public string[] ProfileImage { get; set; }

    [JsonProperty("ExecuteTime")] public int ExecuteTime { get; set; }
    public static List<string> GetFields()
    {
        var type = typeof(Business);
        var props = type.GetProperties();

        var fields = new List<string>();
        
        foreach (var prop in props)
        {
            if(prop.GetCustomAttributes<JsonIgnoreAttribute>() != null)
                continue;
            
            fields.Add(prop.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName ?? prop.Name);
        }
        
        return fields.ToList();
    }
    [JsonIgnore]

    private List<CountriesExtended>? _supportedCountries { get; set; } = null;
    [JsonIgnore]
    public List<CountriesExtended>? SupportedCountries
    {
        get
        {
            if (_supportedCountries is not null)
            {
                return _supportedCountries;
            }

            _supportedCountries = new List<CountriesExtended>();

            LanguagesIso.Replace("HEB", "ISR");
            LanguagesIso.Replace("ENG", "USA");

            var supportedIso = LanguagesIso.Split(",");
            for (int i = 0; i < supportedIso.Length; i++)
            {
                if (supportedIso[i] is "HEB" or "ENG")
                {
                    supportedIso[i] = supportedIso[i] is "HEB" ? "ISR" : "USA";
                }
            }

            try
            {

            }
            catch (Exception e)
            {
            }
            var countries = MasterDataManger.CountriesExtended;

            if (supportedIso.ToList().TrueForAll(x => x.Equals(string.Empty)))
                return null;

            foreach (var iso in supportedIso)
            {
                var country =
                    countries?.FirstOrDefault(x =>
                        x.ThreeLetterCountryIso.Equals(iso, StringComparison.OrdinalIgnoreCase));


                if (country is not null)
                {
                    _supportedCountries.Add(country);
                }
            }

            return _supportedCountries;
            
        }
    }
}