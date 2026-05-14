
using EagleMES.Api.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace EagleMES.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer(); 
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "EagleMES API",
                    Version = "v1",
                    Description = "Mini MES demo system"
                });
            });

            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowVueApp", policy =>
                {
                    policy
                        .WithOrigins(
                            "http://localhost:5173", // Vue dev server
                            "http://127.0.0.1:5173"
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            // SignalR
            builder.Services.AddSignalR();
            builder.Services.AddHostedService<BackgroundServices.DeviceSimulationService>();

            // Database
            builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Auth
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
                    };
                });

            builder.Services.AddSingleton<Services.RabbitMqPublisher>();
            builder.Services.AddHostedService<BackgroundServices.WorkOrderCreatedConsumer>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger(); 
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Smart Factory API v1");
                    options.RoutePrefix = "swagger";
                });
            }

            app.UseHttpsRedirection();

            // CORS
            app.UseCors("AllowVueApp");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapHub<Hubs.DeviceHub>("/devicehub");

            app.Run();
        }
    }
}
