namespace dmweis.ASC.Connector.Scriping
{
   public class MagnetCommand : ArmCommand
   {
      private bool m_MagnetOn;
      public bool MagnetOn
      {
         get { return m_MagnetOn; }
         set
         {
            m_MagnetOn = value;
            RaisePropertyChanged();
         }
      }

      public MagnetCommand( bool turnOn )
      {
         MagnetOn = turnOn;
      }

      public MagnetCommand()
      {
      }
   }
}
