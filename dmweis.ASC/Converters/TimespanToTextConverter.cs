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
         string parameterString = parameter as string;
         if ( parameterString != null)
         {
            valueString = valueString?.Replace(parameterString, String.Empty);
         }
         int milliseconds;
         if (int.TryParse( valueString, out milliseconds ))
         {
            return TimeSpan.FromMilliseconds(milliseconds);
         }
         return TimeSpan.Zero;
      }
   }
}
