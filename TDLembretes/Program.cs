﻿using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using TDLembretes.Models;
using TDLembretes.Repositories;
using TDLembretes.Repositories.Data;
using TDLembretes.Services;

namespace TDLembretes
{
    public class Program
    {
        private static void ConfigureSwagger(IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TDL-Lembretes API", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
        }

        private static void InitializeSwagger(WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        private static void InjectRepositoryDependency(IHostApplicationBuilder builder)
        {
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<tdlDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
        }

        private static void Authentication(IHostApplicationBuilder builder)
        {
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SECRET_KEY"]!))
                    };
                });

            builder.Services.AddAuthorization();
        }

        private static void SeedOnInitialize(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<tdlDbContext>();
                SeedData.Initialize(services);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "Erro ao executar seed no banco de dados.");
            }
        }

        private static void AddControllersAndDependencies(IHostApplicationBuilder builder)
        {
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); 
                });

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddScoped<AuthService>();
            builder.Services.AddScoped<AuthRepository>();

            builder.Services.AddScoped<TarefaOficialService>();
            builder.Services.AddScoped<TarefaOficialRepository>();

            builder.Services.AddScoped<TarefaPersonalizadaService>();
            builder.Services.AddScoped<TarefaPersonalizadaRepository>();

            builder.Services.AddScoped<UsuarioService>();
            builder.Services.AddScoped<UsuarioRepository>();

            builder.Services.AddScoped<ProdutoService>();
            builder.Services.AddScoped<ProdutoRepository>();

            builder.Services.AddScoped<UsuarioTarefasOficialRepository>();
            builder.Services.AddScoped<UsuarioTarefasOficialService>();

            builder.Services.AddScoped<CompraService>();
            builder.Services.AddScoped<TokenService>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("MinhaPoliticaCors", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
            });

            builder.Services.AddRazorPages();
        }

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            AddControllersAndDependencies(builder);
            InjectRepositoryDependency(builder);
            Authentication(builder);
            ConfigureSwagger(builder.Services);

            builder.WebHost.UseUrls("http://localhost:5083", "http://0.0.0.0:7008");

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                InitializeSwagger(app);
            }

            SeedOnInitialize(app);

            app.UseCors("MinhaPoliticaCors");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.MapRazorPages();

            app.Run();
        }
    }
}

