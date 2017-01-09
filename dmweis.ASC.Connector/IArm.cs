using dmweis.ASC.Connector.Scriping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dmweis.ASC.Connector
{
   interface IArm
   {
      Task MoveToCartesianAsync( ArmPosition position  );
      Task MoveToCartesianAsync( double x, double y, double z );
      Task SetMagnet( bool on );
      Task<Arm> ExecuteScriptAsync( ArmScript script );
      Task<Arm> MoveToAsync( ArmCommand position, bool ignoreTimeouts = false );
   }
}
