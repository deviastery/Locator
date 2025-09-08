using Locator.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProgramDependencies(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "Locator"));
}

app.MapControllers();

app.Run();