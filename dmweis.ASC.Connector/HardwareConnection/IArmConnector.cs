using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dmweis.ASC.Connector.HardwareConnection
{
   interface IArmConnector
   {
      Task SetMagnetAsync(bool turnOn);
      Task MoveAllServosAsync( ServoPositions position );
      Task MoveOneServoAsync( byte servoIndex, int servoPwm );
      event EventHandler<ArmDataUpdateEvent> NewServoPosition;
   }
}
