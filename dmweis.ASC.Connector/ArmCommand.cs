using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace dmweis.ASC.Connector
{
   public class ArmCommand : INotifyPropertyChanged
   {
      public event PropertyChangedEventHandler PropertyChanged;

      public string Comment { get; set; }
      public int WaitBefore { get; set; }
      public int WaitAfter { get; set; }
      public int? Base { get; set; }
      public int? Shoulder { get; set; }
      public int? Elbow { get; set; }
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

      private void RaisePropertyChanged( [CallerMemberName] string propertyName = "" )
      {
         PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
      }

   }
}
