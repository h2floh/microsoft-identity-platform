using AuthorizationPoliciesSample.Policies.Requirements;
using AuthorizationPoliciesSample.Policies.Handlers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add authentication services
// For EntraID see https://learn.microsoft.com/en-us/entra/identity-platform/scenario-protected-web-api-app-configuration?tabs=aspnetcore#using-a-custom-app-id-uri-for-a-web-api
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidIssuer = "https://sts.windows.net/ed7e92da-c902-4646-82b7-81cfa187d25e/",
            ValidAudience = "api://mydemo",
        };
        options.Authority = "https://sts.windows.net/ed7e92da-c902-4646-82b7-81cfa187d25e/";
        //options.MetadataAddress = "https://sts.windows.net/ed7e92da-c902-4646-82b7-81cfa187d25e/v2.0/.well-known/openid-configuration";
    });

//Special claim check
builder.Services.AddSingleton<IAuthorizationHandler, AdditionalClaimsHandler>();
builder.Services.AddAuthorizationBuilder()
  .AddPolicy("weatherforecast", policy =>
        policy.Requirements.Add(new AdditionalClaimsRequirement()));

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi()
//.RequireAuthorization();
.RequireAuthorization("weatherforecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
