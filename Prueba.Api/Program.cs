using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Prueba.Application.Ports;
using Prueba.Application.Services;
using Prueba.Infrastructure.Data;
using Prueba.Infrastructure.External;
using Prueba.Infrastructure.Repository;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// ================= JWT options =================
var jwtOptions = configuration.GetSection("JwtOptions").Get<JwtOptions>();

// ================= CORS =================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200") // URL del front Angular
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// ================= Servicios =================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext (SQL Server)
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

// Application services
builder.Services.AddScoped<IAuthLogRepository, AuthLogRepository>();
builder.Services.AddScoped<AuthService>();
builder.Services.Configure<JwtOptions>(configuration.GetSection("JwtOptions"));

// HttpClient for DummyJSON
builder.Services.AddHttpClient<IExternalAuthClient, DummyJsonClient>(client =>
{
    client.BaseAddress = new Uri(configuration["DummyJson:BaseUrl"]
        ?? "https://apps.derechodeautor.gov.co/dummyjson/");
});

// JwtOptions singleton
builder.Services.AddSingleton(jwtOptions);

// ================= JWT Auth =================
var key = Encoding.UTF8.GetBytes(jwtOptions.Secret);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // ?? ponlo en false en desarrollo
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtOptions.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtOptions.Audience,
        ClockSkew = TimeSpan.FromSeconds(30)
    };
});

// ================= App =================
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ?? aplica CORS ANTES de auth
app.UseCors("AllowAngular");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
