namespace Menu4Tech.Models.Api;

public class Response
{
    public string ResultMessage { get; set; }
    public int ResultCode { get; set; }
    public bool IsSuccess => ResultCode >= 0;
}