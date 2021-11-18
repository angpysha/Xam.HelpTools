using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace Xam.HelpTools.Converters
{
    [ContentProperty(nameof(Converters))]
    public class PipeConverter : IValueConverter
    {
        public List<IValueConverter> Converters { get; set; } = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object toReturn = value;
            foreach (var converter in Converters)
            {
                toReturn = converter.Convert(value, targetType, parameter, culture);
            }

            return toReturn;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
