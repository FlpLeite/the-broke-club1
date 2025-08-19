using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TheBrokeClub.API.Data;
using TheBrokeClub.API.Infrastructure.Quotes;
using TheBrokeClub.API.Options;
using TheBrokeClub.API.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// DB
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// App Services
builder.Services.AddScoped<IInvestimentosService, InvestimentosService>();

// AlphaVantage provider + limiter (para o worker usar)
builder.Services.Configure<AlphaVantageOptions>(builder.Configuration.GetSection("AlphaVantage"));
builder.Services.AddScoped<IQuoteCache, DbQuoteCache>();
builder.Services.AddScoped<IQuoteLimiter, DbQuoteLimiter>();
builder.Services.AddHttpClient<IQuoteProvider, AlphaVantageQuoteProvider>(c =>
{
    c.Timeout = TimeSpan.FromSeconds(10);
});

// Cache por ticker + discovery + worker
builder.Services.AddScoped<ITickerPriceCache, DbTickerPriceCache>();
builder.Services.AddScoped<ITrackedTickers, DbTrackedTickers>();
builder.Services.Configure<B3ScheduleOptions>(builder.Configuration.GetSection("B3Schedule"));
builder.Services.AddHostedService<QuoteIngestionWorker>();

// Infra
builder.Services.AddMemoryCache();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "http://127.0.0.1:5173",
                "https://localhost:5173",
                "https://127.0.0.1:5173"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// JWT
var jwtKey = builder.Configuration["Jwt:Key"] ?? "uma_chave_muito_segura_para_dev";
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();