using FastEndpoints;
using FastEndpoints.Swagger;
using Infrastructure;

var bld = WebApplication.CreateBuilder();
bld.Services.AddInfrastructure();
bld.Services.AddFastEndpoints()
   .SwaggerDocument();

var app = bld.Build();
app.UseFastEndpoints(
    cfg =>
    {
        cfg.Endpoints.RoutePrefix = "api";
        cfg.Endpoints.Configurator = ep =>
        {
            ep.AllowAnonymous();
        };
    })
   .UseSwaggerGen();

await app.RunAsync();

public partial class Program;
