namespace Menu4Tech.Configuration;

public class SmtpClientConfiguration : MyConfiguration
{
    public string SmtpHost { get; set; }
    
    public int SmtpPort { get; set; }
    
    public int SmtpSslPort { get; set; }
    
    public bool SmtpUseSsl {get; set; }

    public string SmtpLogin { get; set; }

    public string SmtpPassword { get; set; }

    public string Subject { get; set; }
    
    public string FromName { get; set; }
    
    public string FromAddress { get; set; }
    
    public string ToName { get; set; }
    
    public string ToAddress { get; set; }
    /*
"SmtpPort": 465,
"SmtpUseSsl": true,
"SmtpLogin": "website-umbraco@smart-winners.com",
"SmtpPassword": "cf4Xn45&",
"Subject": "Website error",
"FromName": "SmartWinners new",
"FromAddress": "website-umbraco@smart-winners.com",
"ToName": "admin",
"ToAddress": "website2-errors@smart-winners.com"*/
}