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
         if( parameter is string parameterString )
         {
            valueString = valueString?.Replace( parameterString, string.Empty );
         }
         return int.TryParse( valueString, out int milliseconds ) ? TimeSpan.FromMilliseconds(milliseconds) : TimeSpan.Zero;
      }
   }
}
