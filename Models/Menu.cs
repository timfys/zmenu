using Newtonsoft.Json;

namespace Menu4Tech.Models
{
    public class Menu
    {
        [JsonProperty("sync_modified_date")]
        public DateTime SyncModifiedDate { get; set; }

        [JsonProperty("last_view_date")]
        public DateTime LastViewDate { get; set; }
    }
}
