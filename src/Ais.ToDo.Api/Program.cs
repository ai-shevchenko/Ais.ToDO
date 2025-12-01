using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Ais.Commons.DependencyInjection;
using Ais.Commons.EntityFramework.Contracts;
using Ais.ToDo.Api.Converters;
using Ais.ToDo.Application;
using Ais.ToDo.Infrastructure;
using Ais.ToDo.ServiceDefaults;
using Asp.Versioning;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseDefaultServiceProvider(options =>
{   
    options.ValidateScopes = builder.Environment.IsDevelopment();
    options.ValidateOnBuild = builder.Environment.IsDevelopment();
});

builder.AddServiceDefaults();

builder.Services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = true;
})
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = true;
    
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.Converters.Add(new JsonTrimConverter());
        options.JsonSerializerOptions.Converters.Add(new DateTimeOffsetConverter());
        options.JsonSerializerOptions.Converters.Add(new NullableDateTimeOffsetConverter());
    });

builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "ToDo API"
    });
    
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddProblemDetails();
builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1.0);
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
})
    .AddMvc()
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'V";
        options.SubstituteApiVersionInUrl = true;
    });

builder.Services.AddModule<ApplicationModule>(builder.Configuration);
builder.Services.AddModule<InfrastructureModule>(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseDeveloperExceptionPage();

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var cfgProvider = scope.ServiceProvider.GetRequiredService<AutoMapper.IConfigurationProvider>();
    cfgProvider.AssertConfigurationIsValid();
    
    var migrator = scope.ServiceProvider.GetRequiredService<IDatabaseMigrator>();
    await migrator.MigrateAsync();
}

app.Run();
