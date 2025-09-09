namespace Prueba.Application.Ports
{
    public record ExternalLoginResponse(string Token, string Username);
    public interface IExternalAuthClient
    {
        Task<ExternalLoginResponse?> LoginAsync(string username, string password, CancellationToken ct = default);
        Task<IEnumerable<object>> GetUsersAsync(CancellationToken ct = default);
    }
}
