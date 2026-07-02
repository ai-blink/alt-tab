using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Switchboard.App;

public sealed class EnumEqualsConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        value?.ToString() == parameter?.ToString();

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not true || parameter is null)
        {
            return System.Windows.Data.Binding.DoNothing;
        }

        return Enum.Parse(targetType, parameter.ToString()!);
    }
}
