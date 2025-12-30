using Newtonsoft.Json;

namespace Menu4Tech.Models
{
    public class Category
    {
        [JsonProperty("CategoryId")]
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public IList<Product> Products { get; set; } = new List<Product>();

        [JsonProperty("smallimage")]
        public byte[] Logo { get; set; }

        [JsonExtensionData]
        public Dictionary<string, object> JsonTranslations { get; set; }

    }
}
