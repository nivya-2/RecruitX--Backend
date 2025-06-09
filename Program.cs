using System;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using RecruitX.Interfaces;
using RecruitX.Models;
using RecruitX.Repositories;
using Microsoft.Identity.Web;
using RecruitX;
using Swashbuckle.AspNetCore.SwaggerGen;
using RecruitX.AI;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);
// Allow CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDev", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Use the SAME authentication setup as your working login
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"))
    .EnableTokenAcquisitionToCallDownstreamApi()
    .AddMicrosoftGraph(builder.Configuration.GetSection("Graph"))
    .AddInMemoryTokenCaches();

//builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
//   .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"), "Bearer"); 


// Keep your existing cookie configurations that work with login
builder.Services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    options.CorrelationCookie.SameSite = SameSiteMode.None;
    options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
    options.NonceCookie.SameSite = SameSiteMode.None;
    options.NonceCookie.SecurePolicy = CookieSecurePolicy.Always;
    
});

builder.Services.Configure<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "RecruitX API", Version = "v1" });
    options.DocInclusionPredicate((docName, apiDesc) =>
    {
        var authAttr = apiDesc.CustomAttributes().OfType<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>().Any();
        return !authAttr;
    });
});

//builder.Services.AddSwaggerGen(options =>
//{
//    options.SwaggerDoc("v1", new() { Title = "RecruitX API", Version = "v1" });

//    // Azure AD OAuth2 setup
//    options.AddSecurityDefinition("oauth2", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
//    {
//        Type = Microsoft.OpenApi.Models.SecuritySchemeType.OAuth2,
//        Flows = new Microsoft.OpenApi.Models.OpenApiOAuthFlows
//        {
//            AuthorizationCode = new Microsoft.OpenApi.Models.OpenApiOAuthFlow
//            {
//                AuthorizationUrl = new Uri("https://login.microsoftonline.com/483630a0-0bc9-4d0f-a111-df4a37015334/oauth2/v2.0/authorize"),
//                TokenUrl = new Uri("https://login.microsoftonline.com/483630a0-0bc9-4d0f-a111-df4a37015334/oauth2/v2.0/token"),
//                Scopes = new Dictionary<string, string>
//                {
//                    { "api://5901a83f-24d3-4f86-9fa1-397011ced2fe/.default", "RecruitX API" }
//                }
//            }
//        }
//    });

//options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
//    {
//        {
//            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
//            {
//                Reference = new Microsoft.OpenApi.Models.OpenApiReference
//                {
//                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
//                    Id = "oauth2"
//                }
//            },
//            new[] { "api://5901a83f-24d3-4f86-9fa1-397011ced2fe/.default" }
//        }
//    });
//});


// Add services to the container.
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IUploadJobRequisitionService, JobRequisitionService>();
builder.Services.AddScoped<IJrAssignmentService, JrAssignmentService>();
builder.Services.AddScoped<ITrackJdService, TrackJobDescriptionService>();
// Add this line with your other services
builder.Services.AddScoped<IInterviewPanelService, InterviewPanelService>();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiResponseWrapperFilter>();  // Add the filter here
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddHttpClient(); // Required for IHttpClientFactory
builder.Services.AddScoped<GeminiJobDescriptionGenerator>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpClient();

// Or if you want a named client specifically for Graph API
builder.Services.AddHttpClient("GraphApiClient", client =>
{
    client.BaseAddress = new Uri("https://graph.microsoft.com/");
    client.DefaultRequestHeaders.Add("User-Agent", "YourApp/1.0");
});

var app = builder.Build();

// Configure the HTTP request pipeline - SAME ORDER as your working setup
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "RecruitX API v1");
});
//app.UseSwaggerUI(c =>
//{
//    c.SwaggerEndpoint("/swagger/v1/swagger.json", "RecruitX API v1");

//    c.OAuthClientId("5901a83f-24d3-4f86-9fa1-397011ced2fe");
//    c.OAuthUsePkce(); // Required for Authorization Code Flow with PKCE
//    c.OAuthScopes("api://5901a83f-24d3-4f86-9fa1-397011ced2fe/.default");
//    c.OAuthAppName("RecruitX Swagger UI");
//});


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowAngularDev");
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();