namespace dmweis.ASC.Connector.Scriping
{
   public class MoveCommand : ArmCommand
   {
      private ArmPosition m_Position;
      public ArmPosition Position
      {
         get { return m_Position; }
         set
         {
            m_Position = value;
            RaisePropertyChanged();
         }
      }

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
