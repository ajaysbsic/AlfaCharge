using System.Globalization;

namespace AlfaGrid.Source.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var colors = (parameter as string)?.Split('|');
            var on = colors?[0] ?? "#0E2A47";
            var off = colors?.Length > 1 ? colors[1] : "#6D7A8A";
            return (value as bool? == true) ? Color.FromArgb(on) : Color.FromArgb(off);
        }
        public object? ConvertBack(object? v, Type t, object? p, CultureInfo c) => throw new NotImplementedException();
    }
}
