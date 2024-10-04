using System.Reflection;
using Asp.Versioning;
using FluentValidation;
using Microsoft.OpenApi.Models;
using Rotation.API.Activities.Features;
using Rotation.Infra.Contracts;

namespace Rotation.API;

public static class APIDIExtensions
{
    /// <summary>
    /// Will register Carter, Swagger, APIVersioning
    /// </summary>
    public static void AddAPI(this WebApplicationBuilder builder)
    {
        var currentAssembly = Assembly.GetExecutingAssembly();
        
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Description = "Rotation API",
                Version = "v1",
                Title = "Rotation API"
            });
        });

        builder.Services.AddEndpoints(currentAssembly);

        builder.Services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ApiVersionReader = new HeaderApiVersionReader("X-Api-Version");
        });
        
        builder.Services.AddMediatR(c => c.RegisterServicesFromAssembly(currentAssembly));
        builder.Services.AddValidatorsFromAssembly(currentAssembly);
        
        builder.Services.AddHostedService<MonitorActivitiesJob>();
    }

    public static void UseAPI(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapEndpoints();
    }
}
