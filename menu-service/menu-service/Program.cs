#pragma warning disable CS8602

using Microsoft.EntityFrameworkCore;

using System.Text.Json.Serialization;
using System.Reflection;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Http.Json;

using MvcJsonOptions = Microsoft.AspNetCore.Mvc.JsonOptions;

using DAL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<MenuContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("MenuContext"));
});



builder.Services.Configure<JsonOptions>(o =>
{
    o.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    o.SerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});
builder.Services.Configure<MvcJsonOptions>(o =>
{
    o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt => {
    opt.AddServer(new OpenApiServer
    {
        Url = "http://86.92.40.132:1000",
        Description = "Main production server"
    });
    opt.AddServer(new OpenApiServer
    {
        Url = "http://localhost:1001",
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

var app = builder.Build();

// Startup EF migration
try
{
    using (IServiceScope serviceScope = app.Services.GetService<IServiceScopeFactory>().CreateScope())
    {
        DbContext context = serviceScope.ServiceProvider.GetRequiredService<MenuContext>();
        context.Database.Migrate();
    }
}
catch { }


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseAuthorization();

app.MapControllers();

app.Run();
