using Prueba.Application.Ports;
using Prueba.Domain.Entities;
using Prueba.Infrastructure.Data;

namespace Prueba.Infrastructure.Repository
{
    public class AuthLogRepository : IAuthLogRepository
    {
        private readonly AppDbContext _db;
        public AuthLogRepository(AppDbContext db) => _db = db;
        public async Task AddAsync(LoginLog log, CancellationToken ct = default)
        {
            await _db.LoginLogs.AddAsync(log, ct);
            await _db.SaveChangesAsync(ct);
        }
    }
}
