using CsvHelper.Configuration.Attributes;

namespace Menu4Tech.MetaData;

public class CountriesExtended
{
    [Name("CLDR display name")]
    public string CountryName { get; set; }
    
    [Name("Dial")]
    public string CallingCode { get; set; }
    [Name("ISO3166-1-Alpha-2")]
    public string TwoLetterCountryIso { get; set; }
    [Name("ISO3166-1-Alpha-3")]
    public string ThreeLetterCountryIso { get; set; }
    [Name("ISO4217-currency_alphabetic_code")]
    public string CurrencyIso { get; set; }
}