using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using dmweis.ASC.Connector.Annotations;

namespace dmweis.ASC.Connector
{
   public class ArmPosition : ICloneable, INotifyPropertyChanged
   {
      private double _x;
      public double X
      {
         get { return _x; }
         set
         {
            _x = value;
            RaisePropertyChange();
         }
      }

      private double _y;
      public double Y
      {
         get { return _y; }
         set
         {
            _y = value;
            RaisePropertyChange();
         }
      }

      private double _z;

      public double Z
      {
         get { return _z; }
         set
         {
            _z = value;
            RaisePropertyChange();
         }
      }

      public ArmPosition() { }

      public ArmPosition( double x, double y, double z )
      {
         X = x;
         Y = y;
         Z = z;
      }

      public object Clone()
      {
         ArmPosition newObj = new ArmPosition
         {
            X = X,
            Y = Y,
            Z = Z
         };
         return newObj;
      }

      public event PropertyChangedEventHandler PropertyChanged;

      [NotifyPropertyChangedInvocator]
      protected virtual void RaisePropertyChange([CallerMemberName] string propertyName = null)
      {
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      }
   }
}
