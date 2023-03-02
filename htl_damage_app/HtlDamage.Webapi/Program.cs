using HtlDamage.Application.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using HtlDamage.Application.Dto;
using HtlDamage.Webapi;
using Microsoft.Extensions.Logging;
using HtlDamage.Webapi.Services;

var builder = WebApplication.CreateBuilder(args);

// JWT Authentication ******************************************************************************

byte[] secret = Convert.FromBase64String(builder.Configuration["JwtSecret"]);
builder.Services
    .AddAuthentication(options => options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(secret),
            ValidateAudience = false,
            ValidateIssuer = false
        };
    });

// *************************************************************************************************

builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddHttpContextAccessor();  // Required to access the http context in the authService.
builder.Services.AddTransient<DamageService>();  
builder.Services.AddDbContext<DamageContext>(opt =>
{
    opt.UseSqlServer(
        builder.Configuration.GetConnectionString("Default"),
        o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery));
});

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(
            builder =>
            {
                builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            });
    });
}

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    try
    {
        await app.UseSqlServerContainer(
            containerName: "htldamage_sqlserver2019", version: "latest",
            connectionString: app.Configuration.GetConnectionString("Default"),
            deleteAfterShutdown: true);
    }
    catch (Exception e)
    {
        app.Logger.LogError(e.Message);
        return;
    }

    app.UseCors();
}

// Creating the database.
using (var scope = app.Services.CreateScope())
{
    using (var db = scope.ServiceProvider.GetRequiredService<DamageContext>())
    {
        await db.CreateDatabase(isDevelopment: app.Environment.IsDevelopment());
    }
}

app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.MapFallbackToFile("index.html");
app.Run();
