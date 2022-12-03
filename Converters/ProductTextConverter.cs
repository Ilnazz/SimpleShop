using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SessionProject.Converters
{
    public class ProductTextConverter : IValueConverter
    {
        private const int _defaultTextLength = 255;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string text == false)
                return null;

            int textLength;
            if (parameter != null && int.TryParse(parameter.ToString(), out var number) == true)
                textLength = number;
            else
                textLength = _defaultTextLength;

            var textWithoutNewLines = text.Replace('\n', ' ');
            if (textWithoutNewLines.Length < textLength)
                return textWithoutNewLines;
            return textWithoutNewLines.Substring(0, textLength) + "...";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
