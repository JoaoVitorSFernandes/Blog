using Blog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Blog.Attibutes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ApiKeyAttribute : Attribute, IAsyncActionFilter
{
    /*private readonly IConfiguration _configuration;

    public ApiKeyAttribute(IConfiguration configuration)
        => _configuration = configuration;*/

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        //_configuration.GetValue<string>("ApiKeyConfigurations:ApiKeyName")
        if (!context.HttpContext.Request.Query.TryGetValue("xQ58hwZqpkm7V3yQbG0eqwi6WD52uKEEq7Np9K3azTVg", out var extractedApiKey))
        {
            context.Result = new ContentResult()
            {
                StatusCode = 401,
                Content = "ApiKey not found"
            };
            return;
        }

        //_configuration.GetValue<string>("ApiKeyConfigurations:ApiKey")
        if (!"curso_api_IlTevUM/z0ey3NwCV/unWg==".Equals(extractedApiKey))
        {
            context.Result = new ContentResult()
            {
                StatusCode = 403,
                Content = "Acess not authorizated"
            };
            return;
        }

        await next();
    }
}