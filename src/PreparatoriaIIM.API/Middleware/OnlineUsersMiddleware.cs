using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace PreparatoriaIIM.API.Middleware
{
    public class OnlineUsersMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;
        private readonly ILogger<OnlineUsersMiddleware> _logger;
        private const string OnlineUsersCacheKey = "OnlineUsers";
        private const int CacheExpirationMinutes = 5;

        public OnlineUsersMiddleware(RequestDelegate next, IMemoryCache cache, ILogger<OnlineUsersMiddleware> logger)
        {
            _next = next;
            _cache = cache;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                var userId = context.User.FindFirst("uid")?.Value;
                if (!string.IsNullOrEmpty(userId))
                {
                    var onlineUsers = _cache.GetOrCreate(OnlineUsersCacheKey, entry =>
                    {
                        entry.SlidingExpiration = TimeSpan.FromMinutes(CacheExpirationMinutes);
                        return new ConcurrentDictionary<string, DateTime>();
                    });

                    onlineUsers[userId] = DateTime.UtcNow;
                    _cache.Set(OnlineUsersCacheKey, onlineUsers);
                    
                    // Agregar el conteo de usuarios en línea a los headers de respuesta
                    context.Response.OnStarting(() =>
                    {
                        context.Response.Headers.Add("X-Online-Users", onlineUsers.Count.ToString());
                        return Task.CompletedTask;
                    });
                }
            }

            await _next(context);
        }
    }

    public static class OnlineUsersMiddlewareExtensions
    {
        public static IApplicationBuilder UseOnlineUsersTracking(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<OnlineUsersMiddleware>();
        }
    }
}
