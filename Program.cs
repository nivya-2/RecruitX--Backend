using System;
using System.Text.Json.Serialization;
using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using RecruitX.Interfaces;
using RecruitX.Models;
using RecruitX.Repositories;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

builder.Logging.ClearProviders();
builder.Logging.AddConsole(); // Add Console logger or configure as needed


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDev", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddSingleton<GraphServiceClient>(provider =>
{
    var tenantId = config["AzureAd:TenantId"];
    var clientId = config["AzureAd:ClientId"];
    var clientSecret = config["AzureAd:ClientSecret"];

    var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
    return new GraphServiceClient(credential);
});

builder.Services.AddScoped<IEmailService, GraphEmailService>();

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddScoped<IUploadJobRequisitionService, JobRequisitionService>();
builder.Services.AddScoped<IJrAssignmentService, JrAssignmentService>();
builder.Services.AddScoped<IEmailService, GraphEmailService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(config.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseCors("AllowAngularDev");
app.MapControllers();

app.Run();
