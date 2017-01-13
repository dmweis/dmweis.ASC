namespace dmweis.ASC.Connector.Scriping
{
   public class MoveCommand : ArmCommand
   {
      public ArmPosition Position { get; set; }

      public MoveCommand( double x, double y, double z )
      {
         Position = new ArmPosition(x, y, z);
      }

      public MoveCommand( ArmPosition position )
      {
         Position = (ArmPosition) position.Clone();
      }

      public MoveCommand()
      {
      }
   }
}
