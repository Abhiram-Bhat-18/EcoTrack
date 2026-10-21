
using System.Text;
using EcoTrack.API.Data;
using EcoTrack.API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using EcoTrack.API.Security;
using Microsoft.AspNetCore.Authorization;
using EcoTrack.API.Services;
using System.Security.Claims;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IPasswordHasher<AppUser>, PasswordHasher<AppUser>>();

var jwt = builder.Configuration.GetSection("Jwt");
var keyValue = jwt["Key"]
    ?? throw new InvalidOperationException("JWT Key is missing from configuration.");

byte[] keyBytes;

try
{
    keyBytes = Convert.FromBase64String(keyValue);
}
catch (FormatException)
{
    throw new InvalidOperationException(
        "JWT Key must be a valid Base64-encoded random key.");
}

if (keyBytes.Length < 32)
{
    throw new InvalidOperationException(
        "JWT Key must contain at least 32 random bytes.");
}

var signingKey = new SymmetricSecurityKey(keyBytes);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt["Issuer"],

            ValidateAudience = true,
            ValidAudience = jwt["Audience"],

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),

            RoleClaimType = System.Security.Claims.ClaimTypes.Role,
            NameClaimType = System.Security.Claims.ClaimTypes.NameIdentifier
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ActiveUser", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.AddRequirements(new ActiveUserRequirement());
    });
});

builder.Services.AddScoped<IAuthorizationHandler, ActiveUserHandler>();
builder.Services.AddScoped<AuditLogService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
