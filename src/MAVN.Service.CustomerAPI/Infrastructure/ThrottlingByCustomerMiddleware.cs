using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Falcon.Common.Middleware.Authentication;
using JetBrains.Annotations;
using MAVN.Service.CustomerAPI.Core.Domain;
using MAVN.Service.CustomerAPI.Core.Services;
using MAVN.Service.CustomerAPI.Infrastructure.Extensions;
using Lykke.Service.Sessions.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace MAVN.Service.CustomerAPI.Infrastructure
{
    public class ThrottlingByCustomerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IDistributedLocksService _locksService;
        private readonly ISessionsServiceClient _sessionsServiceClient;
        private readonly IReadOnlyList<RouteThrottlingConfigurationItem> _config;

        public ThrottlingByCustomerMiddleware(RequestDelegate next,
            IThrottlingSettingsService throttlingSettingsService,
            ISessionsServiceClient sessionsServiceClient, 
            IDistributedLocksServiceProvider distributedLocksServiceProvider)
        {
            _next = next;
            _locksService = distributedLocksServiceProvider.Get(DistributedLockPurpose.RouteThrottling);
            _sessionsServiceClient = sessionsServiceClient;
            _config = throttlingSettingsService.GetRouteSettings();
        }

        [UsedImplicitly]
        public async Task InvokeAsync(HttpContext context)
        {
            var configurationItem =
                _config.FirstOrDefault(x => x.Verb == context.Request.Method && x.Route == context.Request.Path);

            if (configurationItem == null)
            {
                await _next.Invoke(context);
                return;
            }

            var lykkeToken = context.GetLykkeToken();

            if (string.IsNullOrEmpty(lykkeToken))
            {
                await _next.Invoke(context);
                return;
            }

            var session = await _sessionsServiceClient.SessionsApi.GetSessionAsync(lykkeToken);

            if (session == null)
            {
                await _next.Invoke(context);
                return;
            }

            var requestAllowed = await _locksService.TryAcquireLockAsync(
                new {customerId = session.ClientId, context.Request.Method, context.Request.Path}.ToJson(),
                DateTime.UtcNow.AddSeconds((double) configurationItem.FrequencyInSeconds),
                session.ClientId, context.Request.Method, context.Request.Path);

            if (requestAllowed)
            {
                await _next.Invoke(context);
                return;
            }

            await context.Write429Async("You have reached requests limit. Please try again later.");
        }
    }

    public static class ThrottlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseThrottling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ThrottlingByCustomerMiddleware>();
        }
    }
}
