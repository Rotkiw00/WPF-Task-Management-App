using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TaskManager.Core.Enums;

namespace TaskManager.Converters;

public class StatusToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not Status status)
            return Brushes.Gray;

        return status switch
        {
            Status.Draft => Brushes.LightGray,
            Status.Assigned => Brushes.Orange,
            Status.InProgress => Brushes.Blue,
            Status.UnderReview => Brushes.Purple,
            Status.Completed => Brushes.Green,
            Status.Rejected => Brushes.Red,
            _ => Brushes.Gray
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}