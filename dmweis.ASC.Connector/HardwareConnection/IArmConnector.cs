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
      Task SetServoSpeed(byte speed);
      event EventHandler<ArmDataUpdateEvent> NewServoPosition;
   }
}
