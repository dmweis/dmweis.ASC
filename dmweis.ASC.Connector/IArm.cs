using dmweis.ASC.Connector.Scriping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dmweis.ASC.Connector
{
   public interface IArm
   {
      double MaxArmReach { get; }
      Task MoveToCartesianAsync( ArmPosition position  );
      Task MoveToCartesianAsync( double x, double y, double z );
      Task SetMagnetAsync( bool on );
      Task<Arm> ExecuteScriptAsync( ArmScript script );
      Task<Arm> ExecuteCommandAsync( ArmCommand command, bool ignoreTimeouts = false );
   }
}
