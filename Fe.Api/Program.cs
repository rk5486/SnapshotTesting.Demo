using FastEndpoints;
using FastEndpoints.Swagger;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using ProblemDetails = FastEndpoints.ProblemDetails;

var bld = WebApplication.CreateBuilder();
bld.Services.AddInfrastructure();
bld.Services.AddFastEndpoints()
   .SwaggerDocument();

var app = bld.Build();
app.UseDefaultExceptionHandler(useGenericReason: true)
   .UseFastEndpoints(
       cfg =>
       {
           cfg.Endpoints.RoutePrefix = "api";
           cfg.Endpoints.Configurator = ep => { ep.AllowAnonymous(); };
           cfg.Errors.ProducesMetadataType = typeof(ProblemDetails);
           cfg.Errors.UseProblemDetails(
               pCfg =>
               {
                   pCfg.AllowDuplicateErrors = false;  // not allows duplicate errors for the same error name
                   pCfg.IndicateErrorCode = false;     // serializes the fluentvalidation error code
                   pCfg.IndicateErrorSeverity = false; // serializes the fluentvalidation error severity
                   pCfg.TypeValue = "https://www.rfc-editor.org/rfc/rfc7231#section-6.5.1";
                   pCfg.TitleValue = "One or more validation errors occurred.";
                   pCfg.TitleTransformer = pd => pd.Status switch
                   {
                       400 => "Validation Error",
                       404 => "Not Found",
                       _   => "One or more errors occurred!"
                   };
                   pCfg.ResponseBuilder = (failures, ctx, statusCode) =>
                   {
                       return new ValidationProblemDetails(
                           failures.GroupBy(f => f.PropertyName)
                                   .ToDictionary(
                                       keySelector: e => e.Key,
                                       elementSelector: e => e.Select(m => m.ErrorMessage).ToArray()))
                       {
                           Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                           Title = "One or more validation errors occurred.",
                           Status = statusCode,
                           Instance = ctx.Request.Path,
                           Extensions = { { "traceId", ctx.TraceIdentifier } }
                       };
                   };
               });
       }
   )
   .UseSwaggerGen();

await app.RunAsync();

public partial class Program;
