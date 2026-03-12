using Microsoft.Extensions.DependencyInjection;

namespace AlfaGrid.Source.Helpers
{
    /// <summary>
    /// Provides access to the built service provider from places where constructor injection is not available
    /// (e.g., markup extensions).
    /// </summary>
    public static class ServiceHelper
    {
        private static IServiceProvider? _services;

        public static void Initialize(IServiceProvider services)
        {
            _services = services;
        }

        public static T GetRequiredService<T>() where T : notnull
        {
            if (_services is null)
            {
                throw new InvalidOperationException("Service provider not initialized. Call ServiceHelper.Initialize during app startup.");
            }

            return _services.GetRequiredService<T>();
        }
    }
}
