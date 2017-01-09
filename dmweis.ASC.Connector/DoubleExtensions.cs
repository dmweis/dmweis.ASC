using System;

namespace dmweis.ASC.Connector
{
   static class DoubleExtensions
   {
      public static double DegreeToRad( this double @this )
      {
         return Math.PI * @this / 180.0;
      }

      public static double RadToDegree( this double @this )
      {
         return @this * ( 180.0 / Math.PI );
      }

      public static double Square( this double @this )
      {
         return @this * @this;
      }

      public static int RoundToInt( this double @this )
      {
         return (int) Math.Round( @this );
      }
   }
}
