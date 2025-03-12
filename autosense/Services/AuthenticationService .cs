using AutoSense.Services.Interfaces;

namespace AutoSense.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        public async Task<bool> LoginAsync(string username, string password)
        {
            // Simulación de autenticación - En una app real, aquí iría tu lógica de autenticación
            await Task.Delay(1000); // Simula una llamada a la API
            return username == "admin" && password == "password";
        }

        public bool Login(string username, string password)
        {
            return username == "admin" && password == "password";
        }
    }
}
