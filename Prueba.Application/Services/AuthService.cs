using Microsoft.IdentityModel.Tokens;
using Prueba.Application.Ports;
using Prueba.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace Prueba.Application.Services
{
  
    public class AuthService
    {
        private readonly IExternalAuthClient _external;
        private readonly IAuthLogRepository _repo;
        private readonly JwtOptions _jwtOptions;

        public AuthService(IExternalAuthClient external, IAuthLogRepository repo, JwtOptions jwtOptions)
        {
            _external = external;
            _repo = repo;
            _jwtOptions = jwtOptions;
        }

        public async Task<string?> LoginAndIssueJwtAsync(string username, string password, CancellationToken ct = default)
        {
            var ext = await _external.LoginAsync(username, password, ct);
            if (ext == null) return null;

            // registrar en base de datos
            var log = new LoginLog { Username = username, LoginTime = DateTime.UtcNow, AccessToken = ext.Token };
            await _repo.AddAsync(log, ct);

            // generar JWT propio
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtOptions.Secret);
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username),
            new Claim("external_token", ext.Token)
        };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiryMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _jwtOptions.Issuer,
                Audience = _jwtOptions.Audience
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<IEnumerable<object>> GetExternalUsers(CancellationToken ct = default)
            => await _external.GetUsersAsync(ct);
    }

    public class JwtOptions
    {
        public string Secret { get; set; } = null!;
        public int ExpiryMinutes { get; set; } = 60;
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
    }

}
