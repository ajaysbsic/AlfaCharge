using System;
using Microsoft.Maui;
using Microsoft.Extensions.DependencyInjection;

namespace AlfaGrid.Source.Extensions
{
    public static class MauiContextExtensionsSafe
    {
        // Safe resolver for IDispatcher that avoids throwing when the underlying
        // IServiceProvider has already been disposed (defensive workaround).
        public static Microsoft.Maui.Dispatching.IDispatcher? GetSafeDispatcher(this IMauiContext? context)
        {
            if (context?.Services == null)
                return null;

            try
            {
                return context.Services.GetService<Microsoft.Maui.Dispatching.IDispatcher>();
            }
            catch (ObjectDisposedException)
            {
                // Service provider already disposed during teardown — avoid throwing
                return null;
            }
        }
    }
}
