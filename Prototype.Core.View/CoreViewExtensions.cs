using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Prototype.Core
{
    public static class CoreViewExtensions
    {
        public static readonly BooleanToVisibilityConverter BooleanToVisibilityConverterInstance = new BooleanToVisibilityConverter();

        public static void SetOneTimeBinding(DependencyObject target, DependencyProperty property, string path, object source, IValueConverter converter = null)
        {
            BindingOperations.SetBinding(target, property, new Binding
            {
                Path = new PropertyPath(path),
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Converter = converter,
                ValidatesOnDataErrors = false,
                ValidatesOnExceptions = false,
                Source = source
            });
        }
    }
}
