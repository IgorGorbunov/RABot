using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace RABot.VM_Little_table
{
    class BigStopToColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] == DependencyProperty.UnsetValue)
            {
                return new SolidColorBrush(Colors.LightGray);
            }
            if ((decimal)values[0] > 0)
            {
                decimal openValue = (decimal)values[0];
                decimal stopValue = (decimal)values[1];
                if (openValue * 9 < stopValue)
                    return new SolidColorBrush(Colors.Red);
                else
                    return new SolidColorBrush(Colors.LightGray);
            }
            else
            {
                return new SolidColorBrush(Colors.Yellow);
            }
            
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
