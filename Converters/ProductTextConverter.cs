using System;
using System.Globalization;
using System.Windows.Data;

namespace SessionProject.Converters
{
    /// <summary>
    /// Cuts given text to fit given length or to default length
    /// </summary>
    public class TextLengthConverter : IValueConverter
    {
        private const int _defaultTextLength = 255;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string text == false)
                return null;

            int textLength;
            if (parameter != null
                && int.TryParse(parameter.ToString(), out var number) is true)
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
            throw new NotImplementedException();
        }
    }
}