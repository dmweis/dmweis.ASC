using System.ComponentModel;
using System.Runtime.CompilerServices;
using dmweis.ASC.Connector.Annotations;

namespace dmweis.ASC.Connector.Scriping
{
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
