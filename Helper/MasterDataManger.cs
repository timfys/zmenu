using System.Globalization;
using System.Reflection;
using CsvHelper;
using Menu4Tech.MenuCTech.Models;
using Menu4Tech.MetaData;
using NUglify.Helpers;

namespace Menu4Tech.Helper;

public static class MasterDataManger
{
    public static List<CountriesExtended> CountriesExtended { get; private set; }

    static MasterDataManger()
    {
        CountriesExtended = GetCountriesListExtended();
    }

    public static List<Countries> GetCountriesList()
    {
        List<Countries> countriesList = new List<Countries>();

        var path = Path.Combine(EnvironmentHelper.Environment.ContentRootPath, "MetaData", "country-codes.csv");
        
        var lines = File.ReadLines(path);

        lines.ForEach(x =>
        {
            var callingCodeAndCountryName = x.Split(": ");
            
            countriesList.Add(new Countries
            {
                CountryName = callingCodeAndCountryName[0],
                CallingCode = callingCodeAndCountryName[1]
            });
        });
        
        return countriesList;
    }
    private static List<CountriesExtended> GetCountriesListExtended()
    {
        var path = Path.Combine(EnvironmentHelper.Environment.ContentRootPath, "MetaData", "country-codes.csv");
        
        using (var reader = new StreamReader(path))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            return csv.GetRecords<CountriesExtended>().ToList();
        }
    }
}