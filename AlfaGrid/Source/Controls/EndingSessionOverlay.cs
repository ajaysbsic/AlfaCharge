using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace AlfaGrid.Source.Controls
{
    public class EndingSessionOverlay : ContentView
    {
        public static readonly BindableProperty MessageProperty =
            BindableProperty.Create(nameof(Message), typeof(string), typeof(EndingSessionOverlay), "Please wait...");

        public string Message
        {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        public EndingSessionOverlay()
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
                Padding = new Thickness(32, 28),
                WidthRequest = 280,
                HeightRequest = 200,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            var stackLayout = new VerticalStackLayout
            {
                Spacing = 20,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            // Title
            var titleLabel = new Label
            {
                Text = "Ending Session",
                FontSize = 20,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromArgb("#0E2A47"),
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center
            };

            // Separator line
            var separator = new BoxView
            {
                HeightRequest = 1,
                BackgroundColor = Color.FromArgb("#E0E0E0"),
                HorizontalOptions = LayoutOptions.Fill,
                Margin = new Thickness(0, 4, 0, 12)
            };

            // Activity Indicator
            var activityIndicator = new ActivityIndicator
            {
                IsRunning = true,
                Color = Color.FromArgb("#0066FF"),
                HeightRequest = 40,
                WidthRequest = 40,
                HorizontalOptions = LayoutOptions.Center
            };

            // Message label
            var messageLabel = new Label
            {
                FontSize = 14,
                TextColor = Color.FromArgb("#6D7A8A"),
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                LineBreakMode = LineBreakMode.WordWrap,
                MaxLines = 2
            };

            messageLabel.SetBinding(Label.TextProperty, new Binding(nameof(Message), source: this));

            stackLayout.Add(titleLabel);
            stackLayout.Add(separator);
            stackLayout.Add(messageLabel);
            stackLayout.Add(activityIndicator);
            
            border.Content = stackLayout;
            grid.Add(border);

            return grid;
        }
    }
}
