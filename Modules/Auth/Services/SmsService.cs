using CampusServicePortal_TicUnicorns.Modules.Auth.Interfaces.Service;
using System.Text.Json;

namespace CampusServicePortal_TicUnicorns.Modules.Auth.Services;

public class SmsService : ISmsService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public SmsService(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task SendAsync(string mobileNumber, string message)
    {
        var userId = _configuration["Notify:UserId"];
        var apiKey = _configuration["Notify:ApiKey"];
        var senderId = _configuration["Notify:SenderId"];

        if (string.IsNullOrWhiteSpace(userId))
            throw new InvalidOperationException(
                "Notify UserId is not configured.");

        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException(
                "Notify API Key is not configured.");

        if (string.IsNullOrWhiteSpace(senderId))
            throw new InvalidOperationException(
                "Notify Sender ID is not configured.");

        var to = NormalizeSriLankanMobile(mobileNumber);

        var content = new FormUrlEncodedContent(
            new Dictionary<string, string>
            {
                ["user_id"] = userId,
                ["api_key"] = apiKey,
                ["sender_id"] = senderId,
                ["to"] = to,
                ["message"] = message
            });

        var response = await _httpClient.PostAsync(
            "https://app.notify.lk/api/v1/send",
            content);

        var responseBody =
            await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"SMS sending failed: {responseBody}");
        }
    }

    private static string NormalizeSriLankanMobile(
        string mobileNumber)
    {
        var digits = new string(
            mobileNumber.Where(char.IsDigit).ToArray());

        if (digits.StartsWith("0") && digits.Length == 10)
        {
            return "94" + digits[1..];
        }

        if (digits.StartsWith("94") && digits.Length == 11)
        {
            return digits;
        }

        throw new ArgumentException(
            "Invalid Sri Lankan mobile number.");
    }
}