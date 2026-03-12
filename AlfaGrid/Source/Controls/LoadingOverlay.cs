using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;

namespace AlfaGrid.Source.Controls
{
    public class LoadingOverlay : ContentView
    {
        public static readonly BindableProperty LoadingTextProperty =
            BindableProperty.Create(nameof(LoadingText), typeof(string), typeof(LoadingOverlay), "Loading...");

        public string LoadingText
        {
            get => (string)GetValue(LoadingTextProperty);
            set => SetValue(LoadingTextProperty, value);
        }

        public LoadingOverlay()
        {
            Content = CreateContent();
        }

        private Microsoft.Maui.Controls.View CreateContent()
        {
            var grid = new Grid
            {
                BackgroundColor = Color.FromArgb("#80000000"),
                InputTransparent = false
            };

            var border = new Border
            {
                BackgroundColor = Colors.White,
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 16 },
                Padding = new Thickness(32, 24),
                WidthRequest = 140,
                HeightRequest = 140,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            var stackLayout = new VerticalStackLayout
            {
                Spacing = 16,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            var activityIndicator = new ActivityIndicator
            {
                IsRunning = true,
                Color = Color.FromArgb("#0066FF"),
                HeightRequest = 48,
                WidthRequest = 48,
                HorizontalOptions = LayoutOptions.Center
            };

            var label = new Label
            {
                FontSize = 14,
                TextColor = Color.FromArgb("#333333"),
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center
            };

            label.SetBinding(Label.TextProperty, new Binding(nameof(LoadingText), source: this));

            stackLayout.Add(activityIndicator);
            stackLayout.Add(label);
            border.Content = stackLayout;
            grid.Add(border);

            return grid;
        }
    }
}
