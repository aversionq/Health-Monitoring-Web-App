using HealthMonitoringApp.Application.Interfaces;
using HealthMonitoringApp.Business.Implementations;
using HealthMonitoringApp.Business.Interfaces;
using HealthMonitoringApp.Data.DatabaseContext;
using HealthMonitoringApp.Data.Implementations;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<HealthMonitoringDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("MSSQLConnection"))
    );
builder.Services.AddScoped<IPressureBusiness, PressureBusiness>();
builder.Services.AddScoped<IPressureRepository, PressureRepository>();
builder.Services.AddScoped<HealthMonitoringDbContext>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
