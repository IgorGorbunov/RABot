using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace RABot.VM_Little_table
{
    class NullValToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Все проверки для краткости выкинул
            return (decimal)value <= 0 ?
                new SolidColorBrush(Colors.Yellow)
                : new SolidColorBrush(Colors.LightGray);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
