﻿using System;
using System.CodeDom;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using dmweis.ASC.Connector.Scriping;
using System.Threading.Tasks;
using dmweis.ASC.Connector.Annotations;

namespace dmweis.ASC.Connector
{
   public abstract class ArmBase : INotifyPropertyChanged
   {
      public abstract double MaxArmReach { get; }
      public abstract Task MoveToCartesianAsync( ArmPosition position  );
      public abstract Task MoveToCartesianAsync( double x, double y, double z );
      public abstract Task MoveToRelativeAsync(double baseAngle, double distance, double z);
      public abstract Task SetMagnetAsync( bool on );
      public async Task ExecuteScriptAsync(ArmScript script)
      {
         foreach (var command in script.Movements)
         {
            await ExecuteCommandAsync(command);
         }
         return;
      }

      public async Task ExecuteCommandAsync(ArmCommand command)
      {
         await ExecuteCommandAsync(command as dynamic);
      }

      private async Task ExecuteCommandAsync( MoveCommand command) => await MoveToCartesianAsync( command.Position );
      private async Task ExecuteCommandAsync(DelayCommand command) => await Task.Delay(command.DelayTime);
      private async Task ExecuteCommandAsync( MagnetCommand command ) => await SetMagnetAsync( command.MagnetOn );

      public event PropertyChangedEventHandler PropertyChanged;
      [NotifyPropertyChangedInvocator]
      protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
      {
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      }
   }
}
