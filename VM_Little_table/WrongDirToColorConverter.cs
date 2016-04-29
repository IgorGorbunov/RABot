using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace RABot.VM_Little_table
{
    class WrongDirToColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[1] == DependencyProperty.UnsetValue)
            {
                return new SolidColorBrush(Colors.LightGray);
            }
            decimal openValue = (decimal)values[1];
            if (openValue <= 0)
            {
                return new SolidColorBrush(Colors.Yellow);
            }
            bool isLong = (bool) values[0];
            decimal stopValue = (decimal)values[2];
            if (isLong)
            {
                if (stopValue > openValue)
                {
                    return new SolidColorBrush(Colors.Red);
                }
                else
                {
                    return new SolidColorBrush(Colors.LightGray);
                }
            }
            else
            {
                if (stopValue > openValue)
                {
                    return new SolidColorBrush(Colors.LightGray);
                    
                }
                else
                {
                    return new SolidColorBrush(Colors.Red);
                }
            }

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
