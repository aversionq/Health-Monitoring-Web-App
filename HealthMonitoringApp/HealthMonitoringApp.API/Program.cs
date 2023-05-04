using HealthMonitoringApp.Application.Interfaces;
using HealthMonitoringApp.Business.Implementations;
using HealthMonitoringApp.Business.Interfaces;
using HealthMonitoringApp.Data.DatabaseContext;
using HealthMonitoringApp.Data.Implementations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddDbContext<HealthMonitoringDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("MSSQLConnection"))
    );
builder.Services.AddScoped<IPressureBusiness, PressureBusiness>();
builder.Services.AddScoped<IPressureRepository, PressureRepository>();
builder.Services.AddScoped<IHeartRateBusiness, HeartRateBusiness>();
builder.Services.AddScoped<IHeartRateRepository, HeartRateRepository>();
builder.Services.AddScoped<IBloodSugarBusiness, BloodSugarBusiness>();
builder.Services.AddScoped<IBloodSugarRepository, BloodSugarRepository>();
builder.Services.AddScoped<HealthMonitoringDbContext>();

// Swagger settings
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Insert JWT",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[]{}
        }
    });
});

// CORS settings
builder.Services.AddCors(options => options.AddPolicy("HealthMonitoringService", b =>
{
    b.AllowAnyOrigin()
     .AllowAnyMethod()
     .AllowAnyHeader();
}));

// JWT Settings
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JWT:Audience"],
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
        };
    });

builder.Services.AddAuthorization();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger for release mode (temporarily)
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "HealthMonitoringAPI V1");
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "HealthMonitoringAPI V1");
    });
}

//app.UseHttpsRedirection();

app.UseCors("HealthMonitoringService");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
