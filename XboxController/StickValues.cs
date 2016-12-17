using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XboxController
{
   public class StickValues : IEquatable<StickValues>
   {
      public double LeftX { get; set; }
      public double LeftY { get; set; }
      public double RightX { get; set; }
      public double RightY { get; set; }

      public override int GetHashCode()
      {
         return GetHashCode() + (int) LeftX * 0x01000000 + (int) LeftY * 0x00010000 + (int) RightX * 0x10000000 + (int) RightY * 0x00000010;
      }

      public override bool Equals( object obj )
      {
         return base.Equals( obj as StickValues );
      }

      public bool Equals( StickValues other )
      {
         if ( ReferenceEquals( other, null ) )
         {
            return false;
         }
         if( ReferenceEquals( this, other ) )
         {
            return true;
         }
         if( GetType() != other.GetType() )
         {
            return false;
         }
         return LeftX == other.LeftX &&
            LeftY == other.LeftY &&
            RightX == other.RightX &&
            RightY == other.RightY;
      }

      public static bool operator ==( StickValues left, StickValues right )
      {
         if( ReferenceEquals( left, null ) )
         {
            if( ReferenceEquals( right, null ) )
            {
               return true;
            }
            return false;
         }
         return left.Equals( right );
      }

      public static bool operator !=( StickValues left, StickValues right )
      {
         return !(left == right);
      }

      public override string ToString()
      {
         return $"LX: {LeftX.ToString("F", CultureInfo.InvariantCulture )} " +
            $"LY: {LeftY.ToString( "F", CultureInfo.InvariantCulture )}" +
            $" RX: {RightX.ToString( "F", CultureInfo.InvariantCulture )} " +
            $"RY: {RightY.ToString( "F", CultureInfo.InvariantCulture )}";
      }

   }
}
