using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using dmweis.ASC.Connector.Annotations;

namespace dmweis.ASC.Connector.Scriping
{
   [XmlInclude(typeof(MoveCommand))]
   [XmlInclude(typeof(MagnetCommand))]
   [XmlInclude(typeof(DelayCommand))]
   public abstract class ArmCommand : INotifyPropertyChanged
   {
      public string Comment { get; set; }

      public event PropertyChangedEventHandler PropertyChanged;

      [NotifyPropertyChangedInvocator]
      protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
      {
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      }
   }
}
