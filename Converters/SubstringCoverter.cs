using System;
using System.Globalization;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace AsadorMoron.Converters
{
    public class SubstringConverter : IValueConverter
    {
        #region IValueConverter Members


        /*
         * Return a substring of the input string using the same notation as SubString()
         *
         * Parameter startIndex | startIndex,length
         *
         * To obtain the functionality of length = (string.length - x) for the length, provide a negative length value
         */

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //Potential Substring parameters
            int startIndex;
            int length;

            //Input as String
            var val = (string)value;

            //Attempt to split parameters by comma
            string[] parameters = ((string)parameter).Split(',');

            //invalid or no parameters, return full string
            if (parameters == null || parameters.Length == 0)
                return val;

            //return remaining string after startIndex
            if (parameters.Length == 1)
            {
                startIndex = int.Parse(parameters[0]);

                return val.Substring(startIndex);
            }


            //return length characters of string after startIndex
            if (parameters.Length >= 2)
            {
                startIndex = int.Parse(parameters[0]);
                length = int.Parse(parameters[1]);

                if (length >= 0)
                    return val.Substring(startIndex, length);


                //negative length was provided
                return val.Substring(startIndex, val.Length + length);
            }

            return val;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        #endregion
    }
}
