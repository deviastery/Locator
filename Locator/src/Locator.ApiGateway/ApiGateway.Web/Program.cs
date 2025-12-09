using ApiGateway.Web.Middlewares;
using Framework.DependencyInjection;
using Framework.Middlewares;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

builder.Services.AddSwaggerForOcelot(builder.Configuration, config =>
{
    config.GenerateDocsForAggregates = false;
});

builder.Services.AddOcelot().AddCacheManager(x => x.WithDictionaryHandle());

builder.Services.AddControllers();

builder.Services.AddJWTAuthenticationScheme(builder.Configuration);
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(corsPolicyBuilder =>
        corsPolicyBuilder.AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin());
});

var app = builder.Build();

app.UseMiddleware<GlobalException>();
app.UseSwaggerForOcelotUI(opt =>
{
    opt.PathToSwaggerGenerator = "/swagger/docs";
    opt.DownstreamSwaggerHeaders =
    [
        new KeyValuePair<string, string>("Api-Gateway", "Signed")
    ];
});

app.UseHttpsRedirection();
app.UseCors();
app.UseMiddleware<AttachSignatureToRequest>();

app.UseOcelot().Wait();

app.Run();
