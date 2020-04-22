using System;
using System.Net;
using System.Threading.Tasks;
using Common;
using Lykke.Common.Api.Contract.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace MAVN.Service.CustomerAPI.Infrastructure.Extensions
{
    public static class HttpContextExtensions
    {
        public static Uri GetUri(this HttpRequest request)
        {
            var hostComponents = request.Host.ToUriComponent().Split(':');

            var builder = new UriBuilder
            {
                Scheme = request.Scheme,
                Host = hostComponents[0],
                Path = request.Path,
                Query = request.QueryString.ToUriComponent()
            };

            if (hostComponents.Length == 2)
            {
                builder.Port = Convert.ToInt32(hostComponents[1]);
            }

            return builder.Uri;
        }

        public static string GetUserAgent(this HttpRequest request)
        {
            return request.Headers["User-Agent"].ToString();
        }

        public static T GetHeaderValueAs<T>(this HttpContext httpContext, string headerName)
        {
            StringValues values;

            if (httpContext?.Request?.Headers?.TryGetValue(headerName, out values) ?? false)
            {
                string rawValues = values.ToString();   // writes out as Csv when there are multiple.

                if (!string.IsNullOrEmpty(rawValues))
                    return (T)Convert.ChangeType(values.ToString(), typeof(T));
            }
            return default(T);
        }
        
        public static Task Write429Async(this HttpContext context, string message)
        {
            context.Response.Clear();
            context.Response.StatusCode = (int) HttpStatusCode.TooManyRequests;
            context.Response.ContentType = "application/json";
            var json = ErrorResponse.Create(message).ToJson();
            
            return context.Response.WriteAsync(json);
        }
    }
}
