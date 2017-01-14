using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dmweis.ASC.Connector.HardwareConnection
{
   class ArmDataUpdateEvent
   {
      public ServoPositions Current { get; set; }
      public ServoPositions LastSent { get; set; }

      public ArmDataUpdateEvent( ServoPositions current, ServoPositions lastSent )
      {
         Current = current;
         LastSent = lastSent;
      }
   }
}
