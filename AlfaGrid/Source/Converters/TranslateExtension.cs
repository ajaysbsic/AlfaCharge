using AlfaGrid.Resources.Localization;
using AlfaGrid.Source.Helpers;
using AlfaGrid.Source.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace AlfaGrid.Source.Converters
{
    [ContentProperty(nameof(Key))]
    public class TranslateExtension : IMarkupExtension<BindingBase>
    {
        public string Key { get; set; } = string.Empty;

        public BindingBase ProvideValue(IServiceProvider serviceProvider)
        {
            if (string.IsNullOrWhiteSpace(Key))
                return new Binding();

            var localizationService = ServiceHelper.GetRequiredService<ILocalizationService>();

            return new Binding(path: $"[{Key}]")
            {
                Source = localizationService,
                Mode = BindingMode.OneWay
            };
        }

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
            => ProvideValue(serviceProvider);
    }
}
