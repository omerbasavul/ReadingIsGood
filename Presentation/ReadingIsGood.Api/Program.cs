using System.Reflection;
using System.Runtime;
using Microsoft.OpenApi.Models;
using ReadingIsGood.Application.Middlewares;
using ReadingIsGood.BuildingBlocks.Logging;
using ReadingIsGood.Infrastructure;
using ReadingIsGood.Infrastructure.DbInitializer;

GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;

var builder = WebApplication.CreateBuilder();

builder.Host.UseDefaultServiceProvider((_, options) =>
{
    options.ValidateScopes = true;
    options.ValidateOnBuild = true;
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHttpContextAccessor();

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
Console.WriteLine($"environment: {environment}");
var configBuilder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory());
configBuilder.AddJsonFile(!string.IsNullOrWhiteSpace(environment)
        ? $"Common/Configurations/appsettings.{environment}.json"
        : "Common/Configurations/appsettings.json",
    optional: false,
    reloadOnChange: true);

builder.Configuration.AddConfiguration(configBuilder.Build());

await KibanaHelper.CreateKibanaIndexPatternAsync(kibanaUrl: builder.Configuration["Kibana:Host"], patternId: builder.Configuration["Kibana:IndexPatternId"], indexPatternTitle: builder.Configuration["Kibana:IndexPattern"]);
builder.Host.UseCustomizedSerilog();


builder.Services.AddInfrastructureLayer(builder.Configuration);

#region Swagger

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ReadingIsGood API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    try
    {
        c.IncludeXmlComments(xmlPath, true);
    }
    catch
    {
        // ignored
    }
});

#endregion

await using var app = builder.Build();

#region Exception Handling

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

#endregion

app.UseStaticFiles();

app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseHttpsRedirection();
app.UseMiddleware<JwtMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ReadingIsGood API v1");
    c.InjectStylesheet("/css/swagger-dark.css");
});

using (var scope = app.Services.CreateScope())
{
    var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
    await dbInitializer.Initialize();
}

await app.RunAsync();