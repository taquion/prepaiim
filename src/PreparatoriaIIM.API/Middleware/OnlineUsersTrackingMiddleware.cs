using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace PreparatoriaIIM.API.Middleware
{
    public class OnlineUsersTrackingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;
        private readonly ILogger<OnlineUsersTrackingMiddleware> _logger;
        private const string CacheKey = "OnlineUsers";
        private static readonly ConcurrentDictionary<string, DateTime> _onlineUsers = new();
        private static readonly TimeSpan _userTimeout = TimeSpan.FromMinutes(5);
        private static DateTime _lastCleanup = DateTime.UtcNow;

        public OnlineUsersTrackingMiddleware(
            RequestDelegate next,
            IMemoryCache cache,
            ILogger<OnlineUsersTrackingMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Solo rastrear usuarios autenticados
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                var userId = context.User.Identity.Name;
                var now = DateTime.UtcNow;

                // Actualizar el tiempo de actividad del usuario
                _onlineUsers[userId] = now;

                // Limpiar usuarios inactivos periódicamente
                if ((now - _lastCleanup).TotalMinutes >= 5)
                {
                    CleanupInactiveUsers(now);
                    _lastCleanup = now;
                }

                // Actualizar la caché con los usuarios en línea
                _cache.Set(CacheKey, _onlineUsers.Keys, new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(10)
                });

                _logger.LogInformation("Usuario en línea: {UserId}", userId);
            }

            await _next(context);
        }

        private void CleanupInactiveUsers(DateTime currentTime)
        {
            foreach (var userId in _onlineUsers.Keys)
            {
                if (_onlineUsers.TryGetValue(userId, out var lastSeen) && 
                    (currentTime - lastSeen) > _userTimeout)
                {
                    _onlineUsers.TryRemove(userId, out _);
                    _logger.LogInformation("Usuario desconectado por inactividad: {UserId}", userId);
                }
            }
        }
    }

    public static class OnlineUsersTrackingMiddlewareExtensions
    {
        public static IApplicationBuilder UseOnlineUsersTracking(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<OnlineUsersTrackingMiddleware>();
        }
    }
}
