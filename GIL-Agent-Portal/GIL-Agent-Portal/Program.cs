
using GIL_Agent_Portal.DataContext;
using GIL_Agent_Portal.Models;
using GIL_Agent_Portal.Repositories;
using GIL_Agent_Portal.Repositories.Interface;
using GIL_Agent_Portal.Services;
using GIL_Agent_Portal.Services.Intetrface;
using System.Data;
using System.Data.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using GIL_Agent_Portal.Utlity;


namespace GIL_Agent_Portal
{
    public class Program
    {
        public static async Task Main(string[] args) // Marked Main method as async and changed return type to Task
        {
            var builder = WebApplication.CreateBuilder(args);

            // Loads config from appsettings.json and environment variables

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddControllers().AddXmlSerializerFormatters();
            //builder.Services.Configure<PartnerSettings>(builder.Configuration.GetSection("PartnerSettings"));

            builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("EmailSettings"));
            //builder.Services.Configure<SignCsSettings>(builder.Configuration.GetSection("signCs"));

            //// 1️⃣ Bind ApiConfig from appsettings.json
            //builder.Services
            //    .Configure<ApiConfig>(builder.Configuration.GetSection("ApiConfig"));

            //// 2️⃣ Register our NSDL‐API HttpClient + Service
            //builder.Services
            //    .AddHttpClient < INsdlApiService, NsdlApiService>();

            //builder.Services.AddSingleton<SignCsService>();

            //builder.Services.AddScoped<ISessionTokenRepository, SessionTokenRepository>();
            //builder.Services.AddScoped<ISessionTokenService, SessionTokenService>();
            //builder.Services.AddScoped<IBcAgentRegistrationRepository, BcAgentRegistrationRepository>();
            //builder.Services.AddScoped<IBCAgentRepository, BCAgentRepository>();

            builder.Services.AddScoped<SessionTokenService>();
            //test :- sessionToken- step 1
            var service = new SessionTokenService();
            var result = await service.FetchNsdlSessionTokenAsync();

            if (result?.Sessiontokendtls != null)
            {
                Console.WriteLine("✅ TOKEN CREATED:");
                Console.WriteLine("Token Key: " + result.Sessiontokendtls.TokenKey);
                Console.WriteLine("Expires: " + result.Sessiontokendtls.ExpireDate);
                Console.WriteLine("Response: " + result.Sessiontokendtls.Response);
                Console.WriteLine("Token : ", result.Sessiontokendtls.Token);
            }
            else
            {
                Console.WriteLine("❌ Failed to fetch session token.");
            }
            // SignCs step 2
            builder.Services.AddScoped<NsdlBcRegistrationCaller>();  // Add this line to register the NsdlBcRegistrationCaller
            builder.Services.AddScoped<SessionTokenService>();
            builder.Services.AddScoped<NsdlSignCsHelper>();

            // step -  4  Register interfaces + implementations
            builder.Services.AddScoped<IBcAgentRegistrationRepository, BcAgentRegistrationRepository>();
            builder.Services.AddScoped<IBcAgentRegistrationService, BcAgentRegistrationService>();


            builder.Services.AddScoped<IUsersRepository, UsersRepository>();
            builder.Services.AddScoped<IUsersService, UsersService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IDbContext, DbContext>();
            builder.Services.AddHttpClient();

            // Register IDbConnection
            builder.Services.AddScoped<IDbConnection>(sp =>
            {
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                return new SqlConnection(connectionString);
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
            builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

            builder.Services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
                    };
                });

            builder.Services.AddAuthorization();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("AllowAllOrigins");
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }
    }
}
