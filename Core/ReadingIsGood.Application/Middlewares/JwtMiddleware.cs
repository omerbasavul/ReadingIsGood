using Microsoft.AspNetCore.Http;
using ReadingIsGood.Application.Helpers;

namespace ReadingIsGood.Application.Middlewares;

public sealed class JwtMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            var token = context.Request.Headers.Authorization.FirstOrDefault()?.Split(' ').Last() ?? context.Request.Cookies["accessToken"];

            if (!string.IsNullOrEmpty(token))
                await context.ContextValidateTokenExtension(token);

            await next(context);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}