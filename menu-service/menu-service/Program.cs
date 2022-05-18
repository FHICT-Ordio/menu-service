#pragma warning disable CS8602

using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Reflection;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;

using MvcJsonOptions = Microsoft.AspNetCore.Mvc.JsonOptions;

using DAL;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<MenuContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("MenuContext"));
});

// JSON Serializer Configuration
builder.Services.Configure<JsonOptions>(o =>
{
    o.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.Configure<MvcJsonOptions>(o =>
{
    o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// CORS Configuration
builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(builder =>
    {
        builder
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt => {
    opt.AddServer(new OpenApiServer
    {
        Url = "https://86.92.40.132:1000",
        Description = "Main production server"
    });
    opt.AddServer(new OpenApiServer
    {
        Url = "https://localhost:1001",
        Description = "Internal testing server"
    });

    opt.SwaggerDoc("v1", new OpenApiInfo
    {        
        Version = "v1",
        Title = "Menu API",
        Description = "An API used for handling restaurant menus"
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    opt.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    opt.DescribeAllParametersInCamelCase();
});

// OAuth2 Services
string domain = $"https://{builder.Configuration["Auth0:Domain"]}/";
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = domain;
        options.Audience = builder.Configuration["Auth0:Audience"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = ClaimTypes.NameIdentifier
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("read:current_user", policy => policy.Requirements.Add(new menu_service.AuthConfig.HasScopeRequirement("read:current_user", domain)));
});

builder.Services.AddSingleton<IAuthorizationHandler, menu_service.AuthConfig.HasScopeHandler>();


// Application Building
var app = builder.Build();

// EF migration
try
{
    using (IServiceScope serviceScope = app.Services.GetService<IServiceScopeFactory>().CreateScope())
    {
        DbContext context = serviceScope.ServiceProvider.GetRequiredService<MenuContext>();
        context.Database.Migrate();
    }
}
catch 
{
    Console.WriteLine("An error occured during EF Migration, migration aborted");
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseCors();

app.MapControllers();

app.Run();