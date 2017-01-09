namespace dmweis.ASC.Connector
{
   class VerticalServoPositions
   {
      public double Shoulder { get; }
      public double Elbow { get; }

      public VerticalServoPositions( double shoulder, double elbow )
      {
         Shoulder = shoulder;
         Elbow = elbow;
      }
   }
}
