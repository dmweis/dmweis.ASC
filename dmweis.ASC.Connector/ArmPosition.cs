using System;

namespace dmweis.ASC.Connector
{
   public class ArmPosition : ICloneable
   {
      public double X { get; set; }
      public double Y { get; set; }
      public double Z { get; set; }

      public ArmPosition() { }

      public ArmPosition( double x, double y, double z )
      {
         X = x;
         Y = y;
         Z = z;
      }

      public object Clone()
      {
         ArmPosition newObj = new ArmPosition
         {
            X = X,
            Y = Y,
            Z = Z
         };
         return newObj;
      }
   }
}
