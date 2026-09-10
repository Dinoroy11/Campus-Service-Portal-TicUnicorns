namespace CampusServicePortal_TicUnicorns.Modules.Auth.Interfaces.Service;

public interface ISmsService
{
    Task SendAsync(string mobileNumber, string message);
}