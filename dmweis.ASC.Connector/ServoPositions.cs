using System;
using dmweis.ASC.Connector.Annotations;

namespace dmweis.ASC.Connector
{
   class ServoPositions : IEquatable<ServoPositions>
   {
      public double Base { get; }
      public double Shoulder { get; }
      public double Elbow { get; }

      public ServoPositions( double @base, double shoulder, double elbow )
      {
         Base = @base;
         Shoulder = shoulder;
         Elbow = elbow;
      }

      public ServoPositions( double @base, VerticalServoPositions verticalServoPositions )
      {
         Base = @base;
         Shoulder = verticalServoPositions.Shoulder;
         Elbow = verticalServoPositions.Elbow;
      }

      public override string ToString()
      {
         return $"Base: {Base} Elbow: {Elbow} Shoulder: {Shoulder}";
      }

      public bool RelativeEquals(ServoPositions other)
      {
         if( ReferenceEquals( null, other ) ) return false;
         if( ReferenceEquals( this, other ) ) return true;
         return Math.Abs(Base - other.Base) <= 1 && Math.Abs(Shoulder - other.Shoulder) <= 1 &&
                Math.Abs(Elbow - other.Elbow) <= 1;
      }

      public bool Equals(ServoPositions other)
      {
         if (ReferenceEquals(null, other)) return false;
         if (ReferenceEquals(this, other)) return true;
         return Base.Equals(other.Base) && Shoulder.Equals(other.Shoulder) && Elbow.Equals(other.Elbow);
      }

      public override bool Equals(object obj)
      {
         if (ReferenceEquals(null, obj)) return false;
         if (ReferenceEquals(this, obj)) return true;
         if (obj.GetType() != this.GetType()) return false;
         return Equals((ServoPositions) obj);
      }

      public override int GetHashCode()
      {
         unchecked
         {
            var hashCode = Base.GetHashCode();
            hashCode = (hashCode * 397) ^ Shoulder.GetHashCode();
            hashCode = (hashCode * 397) ^ Elbow.GetHashCode();
            return hashCode;
         }
      }

      public static bool operator ==(ServoPositions left, ServoPositions right)
      {
         if (ReferenceEquals(left, null))
         {
            if (ReferenceEquals(right, null))
            {
               return true;
            }
            return false;
         }
         return left.Equals(right);
      }

      public static bool operator !=(ServoPositions left, ServoPositions right)
      {
         return !(left == right);
      }
   }
}
