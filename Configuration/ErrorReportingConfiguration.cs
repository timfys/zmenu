namespace Menu4Tech.Configuration
{
    public class ErrorReportingConfiguration
    {
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public bool SmtpUseSsl { get; set; }
        public string SmtpLogin { get; set; }
        public string SmtpPassword { get; set; }

        public string Subject { get; set; }
        public string FromName { get; set; }
        public string FromAddress { get; set; }
        public string ToName { get; set; }
        public string ToAddress { get; set; }

    }
}
