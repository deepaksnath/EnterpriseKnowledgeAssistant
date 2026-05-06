using DPK.EKA.Api.Extensions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", 
                                    description.GroupName.ToUpperInvariant());
        }
    });
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseRateLimiter();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
