using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace AsadorMoron.Converters
{
    internal class ListObjectToBoolConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;

            return ((IEnumerable<object>)value).Any();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
