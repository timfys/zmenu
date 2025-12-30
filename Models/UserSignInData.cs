using Newtonsoft.Json;

namespace Menu4Tech.Models;

public class UserSignInData
{
    public string Mobile { get; set; }
    public string PhonePrefix { get; set; }
    public string Country { get; set; }
}