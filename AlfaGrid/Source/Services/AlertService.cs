namespace AlfaGrid.Source.Services
{
    public sealed class AlertService : IAlertService
    {
        public Task Info(string title, string message, string ok = "OK")
            => InvokeOnMainPageAsync(async page =>
            {
                await page.DisplayAlertAsync(title, message, ok);
            });

        public Task<bool> Confirm(string title, string message, string accept = "OK", string cancel = "Cancel")
            => InvokeOnMainPageAsync(page => page.DisplayAlertAsync(title, message, accept, cancel));

        private static async Task<T> InvokeOnMainPageAsync<T>(Func<Page, Task<T>> func)
        {
            var page = GetVisiblePage()
                ?? throw new InvalidOperationException("No page is available to display the alert.");

            if (MainThread.IsMainThread)
                return await func(page);

            return await MainThread.InvokeOnMainThreadAsync(() => func(page));
        }

        private static async Task InvokeOnMainPageAsync(Func<Page, Task> func)
        {
            var page = GetVisiblePage()
                ?? throw new InvalidOperationException("No page is available to display the alert.");

            if (MainThread.IsMainThread)
                await func(page);
            else
                await MainThread.InvokeOnMainThreadAsync(() => func(page));
        }

        /// <summary>
        /// Tries to get the currently visible page:
        /// - Modal stack top if present
        /// - Shell.Current (if using Shell)
        /// - Window.Page
        /// </summary>
        private static Page? GetVisiblePage()
        {
            var app = Application.Current;
            var mainPage = app?.Windows?.FirstOrDefault()?.Page;

            // Prefer the top-most modal page if any
            if (mainPage is not null)
            {
                var nav = mainPage.Navigation;
                if (nav?.ModalStack?.Count > 0)
                    return nav.ModalStack.Last();

                // If it's a Shell, use the current page
                if (mainPage is Shell shell)
                    return shell.CurrentPage ?? mainPage;

                return mainPage;
            }

            // Fallback to Shell if available (some apps create Shell without setting MainPage yet)
            if (Shell.Current is not null)
                return Shell.Current.CurrentPage ?? Shell.Current;

            return null;
        }
    }
}