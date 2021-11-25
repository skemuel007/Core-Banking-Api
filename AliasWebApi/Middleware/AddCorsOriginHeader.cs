using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace AliasWebApiCore.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class AddCorsOriginHeader
    {
        private readonly RequestDelegate _next;

        public AddCorsOriginHeader(RequestDelegate next)
        {
            _next = next;
        }

        public   Task Invoke(HttpContext context)
        {
            if (context.Request.Path.Value.StartsWith("/swagger"))
            {
               
                 
                    context.Request.Headers.Add("Au", "*");
                return _next(context);
                //return Task.FromResult(0);
            }

            return _next(context);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class AddCorsOriginHeaderExtensions
    {
        public static IApplicationBuilder UseAddCorsOriginHeader(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AddCorsOriginHeader>();
        }
    }
}
