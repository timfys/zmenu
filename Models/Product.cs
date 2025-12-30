using Newtonsoft.Json;

namespace Menu4Tech.Models
{
    public class Product: EntityResponse
    {
        [JsonProperty("productId")]
        public int Id { get; set; }

        [JsonProperty("ProductName")]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [JsonProperty("CategoryID")]
        public int CategoryId { get; set; }

        public decimal Price { get; set; }

        [JsonProperty("smallimage")]
        public byte[] Logo { get; set; }

        [JsonProperty("SellPrice_CurrencyISO")]
        public string CurrencyISO { get; set; } = string.Empty;

        [JsonProperty("Visible_individually")]
        public bool IsVisible { get; set; }

        [JsonExtensionData]
        public Dictionary<string, object> JsonTranslations { get; set; }

    }
}
