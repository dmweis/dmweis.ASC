namespace dmweis.ASC.Connector
{
   public class ArmPosition
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
   }
}
