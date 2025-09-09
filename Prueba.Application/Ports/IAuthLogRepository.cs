using Prueba.Domain.Entities;

namespace Prueba.Application.Ports
{
    public interface IAuthLogRepository
    {
        Task AddAsync(LoginLog log, CancellationToken ct = default);
    }
}
