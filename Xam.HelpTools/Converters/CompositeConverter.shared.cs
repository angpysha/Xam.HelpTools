using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace Xam.HelpTools.Converters
{
    [ContentProperty(nameof(Converters))]
    public class CompositeConverter : IValueConverter
    {
        public List<IValueConverter> Converters { get; set; } = new();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Func<object, Type, object, CultureInfo, object> del = Converters.FirstOrDefault().Convert;
            for (int i = 1; i < Converters.Count; i++)
            {
                del += Converters[i].Convert;
            }

            return del(value, targetType, parameter, culture);
            //  return Converters.Aggregate(value, (x, y) => y.Convert(value, targetType, parameter, culture));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
