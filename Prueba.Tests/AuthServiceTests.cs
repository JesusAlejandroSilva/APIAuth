using Moq;
using Prueba.Application.Ports;
using Prueba.Application.Services;
using Prueba.Domain.Entities;

namespace Prueba.Tests
{
    public class AuthServiceTests
    {
        [Fact]
        public async Task LoginAndIssueJwtAsync_WhenExternalReturnsToken_ShouldReturnJwtAndLog()
        {
            var mockExternal = new Mock<IExternalAuthClient>();
            mockExternal.Setup(m => m.LoginAsync("user", "pass", It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new ExternalLoginResponse("ext-token", "user"));

            var mockRepo = new Mock<IAuthLogRepository>();
            mockRepo.Setup(r => r.AddAsync(It.IsAny<LoginLog>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask)
                    .Verifiable();

            var opts = new JwtOptions { Secret = "test_secret_0123456789_test", ExpiryMinutes = 30, Issuer = "iss", Audience = "aud" };
            var svc = new AuthService(mockExternal.Object, mockRepo.Object, opts);

            var jwt = await svc.LoginAndIssueJwtAsync("user", "pass");

            Assert.False(string.IsNullOrEmpty(jwt));
            mockRepo.Verify(r => r.AddAsync(It.Is<LoginLog>(l => l.Username == "user" && l.AccessToken == "ext-token"), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
