using Prueba.Application.Ports;
using System.Net.Http.Json;
using System.Text.Json;

namespace Prueba.Infrastructure.External
{
    public class DummyJsonClient : IExternalAuthClient
    {
        private readonly HttpClient _http;
        public DummyJsonClient(HttpClient http) => _http = http;

        public async Task<ExternalLoginResponse?> LoginAsync(string username, string password, CancellationToken ct = default)
        {
            var payload = new { username, password };
            var resp = await _http.PostAsJsonAsync("api/users/login", payload, ct);
            if (!resp.IsSuccessStatusCode) return null;

            var doc = await resp.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: ct);

            if (doc.TryGetProperty("token", out var tokenProp))
            {
                var token = tokenProp.GetString()!;
                string usr = username;
                if (doc.TryGetProperty("username", out var u)) usr = u.GetString() ?? username;
                return new ExternalLoginResponse(token, usr);
            }
            return null;
        }

        public async Task<IEnumerable<object>> GetUsersAsync(CancellationToken ct = default)
        {
            var resp = await _http.GetAsync("api/users", ct);
            resp.EnsureSuccessStatusCode();
            var json = await resp.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: ct);
            if (json.ValueKind == JsonValueKind.Object && json.TryGetProperty("users", out var arr) && arr.ValueKind == JsonValueKind.Array)
            {
                var list = new List<object>();
                foreach (var e in arr.EnumerateArray()) list.Add(e);
                return list;
            }
            return new object[] { json };
        }
    }
}
