namespace Menu4Tech.Helper;

public class FileLogger
{
    public static IWebHostEnvironment Environment { get; set; }

    public static void Log(string fileName, string text)
    {
        var logString = DateTime.Now + text + "\r\n \r\n \r\n \r\n";

        System.IO.File.AppendAllText(Environment.WebRootPath.Replace("wwwroot", "") + fileName, logString);
    }
}