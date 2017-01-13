namespace dmweis.ASC.Connector.Scriping
{
   class MoveCommand : ArmCommand
   {
      public ArmPosition Position { get; set; }

      public MoveCommand( double x, double y, double z )
      {
         Position = new ArmPosition(x, y, z);
      }

      public MoveCommand()
      {
      }
   }
}
