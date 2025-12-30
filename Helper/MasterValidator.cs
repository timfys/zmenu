using System.Text.RegularExpressions;

namespace Menu4Tech.Helper;

public static class MasterValidator
{
    public static  bool ValidateIp(string ip)
    {
        //Would be good to add ipV6 regex too
        var ipV4 = new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b");

        return ipV4.IsMatch(ip);
    }
}