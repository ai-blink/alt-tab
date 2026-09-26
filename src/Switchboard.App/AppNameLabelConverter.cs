using System.Globalization;
using System.Windows.Data;
using Switchboard.Core.Services;

namespace Switchboard.App;

// [AppName, Title, max length] -> short card label, or empty when it would repeat the title.
public sealed class AppNameLabelConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) =>
        values is [string appName, string title, int maxLength]
            ? AppNameLabel.Format(appName, title, maxLength)
            : string.Empty;

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
