using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace dmweis.ASC.Connector.Scriping
{
   public class ArmCommand : INotifyPropertyChanged
   {
      public event PropertyChangedEventHandler PropertyChanged;

      public string Comment { get; set; }
      public int WaitBefore { get; set; }
      public int WaitAfter { get; set; }
      public ArmPosition Position { get; set; }
      public bool? Magnet { get; set; }

      private bool m_Executed;
      [XmlIgnore]
      public bool Executed
      {
         get
         {
            return m_Executed;
         }
         set
         {
            if( m_Executed == value )
            {
               return;
            }

            m_Executed = value;
            RaisePropertyChanged();
         }
      }

      public ArmCommand()
      {
         Position = new ArmPosition();
      }

      private void RaisePropertyChanged( [CallerMemberName] string propertyName = "" )
      {
         PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
      }

   }
}
