using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace dmweis.ASC.ArmController
{
   class BoolToColorConverter : IValueConverter
   {
      public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
      {
         bool? arg = value as bool?;
         if( arg == true )
         {
            return Brushes.Red;
         }
         return Brushes.WhiteSmoke; 
      }

      public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
      {
         throw new NotImplementedException();
      }
   }
}
