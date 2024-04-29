namespace CityInfo.API.Services;


public class LocalMailService : IMailService
{
    private readonly IConfiguration _configuration;

    public LocalMailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public void Send(string subject, string message)
    {
        var mailTo = _configuration["mailSettings:mailToAddress"];

        var mailFrom = _configuration["mailSettings:mailFromAddress"];

        // send mail - output to console window
        Console.WriteLine($"Mail from {mailFrom} to {mailTo} with {nameof(LocalMailService)}");
        Console.WriteLine($"Subject: {subject}");
        Console.WriteLine($"Message: {message}");
    }
}