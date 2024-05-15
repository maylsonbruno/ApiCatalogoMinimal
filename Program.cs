using ApiCatalogoMinimal.ApiEndpoints;
using ApiCatalogoMinimal.AppServicesExtensions;
using ApiCatalogoMinimal.Context;
using ApiCatalogoMinimal.Models;
using ApiCatalogoMinimal.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MySqlConnector;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//builder.Services.AddSwaggerGen();

AdcionandoJwtSwagger(builder);

string? connectionsString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
options
.UseMySql(connectionsString,
    ServerVersion.AutoDetect(connectionsString)));

// Referencia do Serviço de Autenticação
builder.Services.AddSingleton<ITokenServices>(new TokenService());

#region JWT Authentication Minimal APIs
builder.Services.AddAuthentication
                 (JwtBearerDefaults.AuthenticationScheme)
                 .AddJwtBearer(options =>
                 {
                     options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                     {

                         ValidateIssuer = true,
                         ValidateAudience = true,
                         ValidateLifetime = true,
                         ValidateIssuerSigningKey = true,

                         ValidIssuer = builder.Configuration["Jwt:Issuer"],
                         ValidAudience = builder.Configuration["Jwt:Audience"],
                         IssuerSigningKey = new SymmetricSecurityKey
                         (Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))


                     };

                 });

builder.Services.AddAuthorization();
#endregion

// Setando o banco Sql Server
//string? connectionsString = builder.Configuration.GetConnectionString("DefaultConnection");
//builder.Services.AddDbContext<AppDbContext>(options =>
//       options.UseSqlServer(connectionsString));



var app = builder.Build();

app.MapAutenticacaoEndpoint();

app.MapCategoriasEndpoint();

app.MapProdutosEndpoints();


// Configure the HTTP request pipeline.

var environment = app.Environment;

app.UseExceptionHandling(environment)
     .UseSwaggerMiddleware()
     .UseAppCors();

app.UseAuthentication();
app.UseAuthorization();

app.Run();

static void AdcionandoJwtSwagger(WebApplicationBuilder builder)
{
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "apiagenda", Version = "v1" });

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = @"JWT Authorization header using the Bearer scheme
                     'Bearer'
                    Example: \'Bearer 12345abcdef\'",
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
                         new string[] {}
                    }
                });
    });
}