namespace Menu4Tech.Models
{
    public class MenuResponse
    {
        public bool IsUpdated { get; set; }
        public bool IsSuccess { get; set; }

        public string Content { get; set; } = string.Empty;
        public string Response { get; set; } = string.Empty;
    }
}
