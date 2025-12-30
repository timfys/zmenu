using System.Reflection;
using Newtonsoft.Json;
using uSync;

namespace Menu4Tech.Models;

public class User
{
    [JsonProperty("EntityId")]
    public int Id { get; set; }
    [JsonProperty("ol_password")]
    public string Password { get; set; }
    public string Mobile { get; set; }
    [JsonProperty("ol_username")]
    public string Username { get; set; }
    [JsonProperty("mobile_verified")] 
    public bool MobileVerified { get; set; }
    [JsonIgnore]
    public string Lid { get; set; }


    public static List<string> GetFields()
    {
        var type = typeof(User);
        var props = type.GetProperties();

        var fields = new List<string>();
        
        foreach (var prop in props)
        {
            if(prop.GetCustomAttribute<JsonIgnoreAttribute>() != null)
                continue;
            
            fields.Add(prop.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName ?? prop.Name);
        }
        
        return fields.ToList();
    }
}