using FastEndpoints;
using FastEndpoints.Swagger;
using Fe.Api.Common;
using Infrastructure;
using Microsoft.AspNetCore.Http.Features;

var bld = WebApplication.CreateBuilder();

bld.Services.AddInfrastructure();

bld.Services.AddProblemDetails(
    opt =>
    {
        opt.CustomizeProblemDetails = ctx =>
        {
            ctx.ProblemDetails.Instance = $"{ctx.HttpContext.Request.Method} {ctx.HttpContext.Request.Path}";
            ctx.ProblemDetails.Extensions.TryAdd("requestId", ctx.HttpContext.TraceIdentifier);
            
            var activity = ctx.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
            ctx.ProblemDetails.Extensions.TryAdd("traceId", activity?.TraceId);
        };
    }
);
bld.Services.AddExceptionHandler<ProblemExceptionHandler>();

bld.Services.AddFastEndpoints()
   .SwaggerDocument();

var app = bld.Build();
// app.UseDefaultExceptionHandler();
app.UseExceptionHandler();

app.UseFastEndpoints(
       cfg =>
       {
           cfg.Endpoints.RoutePrefix = "api";
           cfg.Endpoints.Configurator = ep => { ep.AllowAnonymous(); };

           // cfg.Errors.ProducesMetadataType = typeof(ProblemDetails);
           // cfg.Errors.UseProblemDetails(
           //     pCfg =>
           //     {
           //         pCfg.AllowDuplicateErrors = false;  // not allows duplicate errors for the same error name
           //         pCfg.IndicateErrorCode = false;     // serializes the fluentvalidation error code
           //         pCfg.IndicateErrorSeverity = false; // serializes the fluentvalidation error severity
           //         pCfg.TypeValue = "https://www.rfc-editor.org/rfc/rfc9110#name-400-bad-request";
           //         pCfg.TitleValue = "One or more validation errors occurred.";
           //         pCfg.TitleTransformer = pd => pd.Status switch
           //         {
           //             400 => "Validation Error",
           //             404 => "Not Found",
           //             _   => "One or more errors occurred!"
           //         };
           //         pCfg.ResponseBuilder = (failures, ctx, statusCode) =>
           //         {
           //             return new ValidationProblemDetails(
           //                 failures.GroupBy(f => f.PropertyName)
           //                         .ToDictionary(
           //                             keySelector: e => e.Key,
           //                             elementSelector: e => e.Select(m => m.ErrorMessage).ToArray()))
           //             {
           //                 Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
           //                 Title = "One or more validation errors occurred.",
           //                 Status = statusCode,
           //                 Instance = ctx.Request.Path,
           //                 Extensions = { { "traceId", ctx.TraceIdentifier } }
           //             };
           //         };
           //     });
       }
   )
   .UseSwaggerGen();

await app.RunAsync();

public partial class Program;
