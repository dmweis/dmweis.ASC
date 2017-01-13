namespace dmweis.ASC.Connector.Scriping
{
   public class MagnetCommand : ArmCommand
   {
      public bool MagnetOn { get; set; }

      public MagnetCommand( bool turnOn )
      {
         MagnetOn = turnOn;
      }

      public MagnetCommand()
      {
      }
   }
}
