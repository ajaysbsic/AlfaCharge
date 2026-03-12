using System.Globalization;

namespace AlfaGrid.Source.Converters
{
    public class RatingToColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int minRating && parameter is string ratingStr && int.TryParse(ratingStr, out int rating))
            {
                return minRating >= rating ? Color.FromArgb("#F79A1B") : Color.FromArgb("#CCCCCC");
            }
            return Color.FromArgb("#CCCCCC");
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
