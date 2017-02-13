using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace dmweis.ASC.Converters
{
   class TimespanToTextConverter : IValueConverter
   {
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      {
         TimeSpan? timeSpan = value as TimeSpan?;
         return timeSpan?.TotalMilliseconds?? double.NaN;
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
      {
         string valueString = value as string;
         valueString = valueString?.Replace(parameter as string ?? string.Empty, string.Empty);
         int milliseconds;
         return int.TryParse(valueString, out milliseconds) ? TimeSpan.FromMilliseconds(milliseconds) : TimeSpan.Zero;
         // C# 7.0 is not out yet :'(
         //return int.TryParse( valueString, out int milliseconds ) ? TimeSpan.FromMilliseconds(milliseconds) : TimeSpan.Zero;
      }
   }
}
