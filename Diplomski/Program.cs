global using Diplomski.Data;
global using Diplomski.Service;
global using Microsoft.EntityFrameworkCore;
using Diplomski;
using Diplomski.Helpers;
using Diplomski.Middlewares;
using dotenv.net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

DotEnv.Load(options: new DotEnvOptions(envFilePaths: new[] { "C:\\Users\\Fosilio\\source\\repos\\Diplomski\\Diplomski\\Config.env" }));

Console.WriteLine($"Issuer: {Environment.GetEnvironmentVariable("JWT_ISSUER")}");
Console.WriteLine($"Audience: {Environment.GetEnvironmentVariable("JWT_AUDIENCE")}");
Console.WriteLine($"Secret: {Environment.GetEnvironmentVariable("JWT_SECRET")}");

builder.Services.AddAuthentication(options => 
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})

.AddJwtBearer(options => 
{
    options.RequireHttpsMetadata = false; 
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
        ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET")!)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder() 
            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser()
            .Build();

});


builder.Services.AddControllers();
   
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization header using the Bearer scheme (\"Bearer {token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<JwtMiddleware>();

builder.Services.AddSingleton<VerifyLoginCodeVerification>();
builder.Services.AddSingleton<ResetPasswordVerification>();
builder.Services.AddScoped<EmailResetPasswordBody>();
builder.Services.AddScoped<EmailCodeVerificationBody>();
builder.Services.AddScoped<EmailVerificationBody>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPermissionSetService, PermissionSetService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<JwtSecurityTokenHandlerWrapper>();
builder.Services.AddScoped<PermissionChecker>();


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<JwtMiddleware>();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();