using System;
using dmweis.ASC.Connector.Scriping;
using System.Threading.Tasks;

namespace dmweis.ASC.Connector
{
   public abstract class ArmBase
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
      public async Task ExecuteCommandAsync( ArmCommand command )
      {
         var delayCommand = command as DelayCommand;
         if (delayCommand != null)
         {
            await Task.Delay(delayCommand.DelayTime);
            return;
         }
         var magnetCommand = command as MagnetCommand;
         if (magnetCommand != null)
         {
            await SetMagnetAsync(magnetCommand.MagnetOn);
            return;
         }
         var moveCommand = command as MoveCommand;
         if (moveCommand != null)
         {
            await MoveToCartesianAsync(moveCommand.Position);
            return;
         }
         throw new NotImplementedException("This command was not implemented");
      }
   }
}
