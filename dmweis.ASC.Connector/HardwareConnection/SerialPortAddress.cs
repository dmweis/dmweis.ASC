using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace dmweis.ASC.Connector.HardwareConnection
{
   public class SerialPortAddress : INotifyPropertyChanged
   {
      public event PropertyChangedEventHandler PropertyChanged;

      private string m_Name;
      public string Name
      {
         get { return m_Name; }
         set
         {
            m_Name = value;
            OnPropertyChanged();
         }
      }

      private string m_Description;
      public string Description
      {
         get { return m_Description; }
         set
         {
            m_Description = value;
            OnPropertyChanged();
         }
      }

      public SerialPortAddress( string name )
      {
         Name = name;
      }

      protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
      {
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      }
   }
}
