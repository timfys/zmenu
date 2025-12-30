namespace Menu4Tech.Models
{
    public class GeneralBusiness: EntityResponse
    {
        public int entityID { get; set; }
        public string name { get; set; }
        public string CountryISO { get; set; }
        public string BusinessName { get; set; }
        public int BusinessId { get; set; }
        public string languagesISO { get; set; }
        public string CurrenciesISO { get; set; }
        public string CurrencyISO_default { get; set; }
    }
}
