namespace AutoSense.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<bool> LoginAsync(string username, string password);

        bool Login(string username, string password);
    }
}
