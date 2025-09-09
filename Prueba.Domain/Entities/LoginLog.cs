namespace Prueba.Domain.Entities
{
    public class LoginLog
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public DateTime LoginTime { get; set; } = DateTime.Now;
        public string? AccessToken { get; set; }
    }
}

