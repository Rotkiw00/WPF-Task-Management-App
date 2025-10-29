using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TaskManager.Core.Enums;

namespace TaskManager.Converters;

public class PriorityToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not Priority priority)
            return Brushes.Gray;

        return priority switch
        {
            Priority.Low => Brushes.Green,
            Priority.Medium => Brushes.Orange,
            Priority.High => Brushes.OrangeRed,
            Priority.Critical => Brushes.Red,
            _ => Brushes.Gray
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}