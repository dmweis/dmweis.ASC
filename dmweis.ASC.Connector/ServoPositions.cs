namespace dmweis.ASC.Connector
{
   class ServoPositions
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
   }
}
