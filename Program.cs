using Models;
using Microsoft.EntityFrameworkCore;
using System.Web;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);
//Andjela afsf
// Add services to the container.
//Vexx  
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "Standard Authorization header using the Bearer scheme (\"bearer {token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey

    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();//dotnet add package Swashbuckle.AspNetCore.Filters
});//ovo unutar() samo test ^^^^


builder.Services.AddDbContext<TeamMakerContext>(Options =>
{
    Options.UseSqlServer(builder.Configuration.GetConnectionString("CSteammaker"));
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });//dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer

builder.Services.AddCors(p => p.AddPolicy("CORS", builder =>
    {
        builder.WithOrigins(new string[]
                    {
                        "http://localhost:8080",
                        "https://localhost:8080",
                        "https://127.0.0.1:8080",
                        "http://127.0.0.1:8080",
                        "https://localhost:5001",
                        "http://localhost:5001",
                        "https://127.0.0.1:5500",
                        "https://localhost:7013",
                        "https://127.0.0.1:7013",
                        "http://127.0.0.1:5500",
                        "https://localhost:5500",
                        "http://localhost:5500",
                        "http://localhost:3000",
                        "https://localhost:3000"

                    }).AllowAnyMethod().AllowAnyHeader();
    }));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}

//aaa
app.UseHttpsRedirection();
app.UseRouting();

app.UseCors("CORS");
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

