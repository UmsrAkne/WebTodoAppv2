namespace WebTodoAppv2.Models.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class TimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return TimeSpan.FromSeconds(Math.Floor(((TimeSpan)value).TotalSeconds));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}