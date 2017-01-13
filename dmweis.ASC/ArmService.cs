using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dmweis.ASC.Connector;
using dmweis.ASC.Connector.HardwareConnection;

namespace dmweis.ASC
{
   class ArmService
   {
      private static readonly Lazy<ArmService> Lazy =
         new Lazy<ArmService>(() => new ArmService());

      public static ArmService Default => Lazy.Value;

      public ArmBase Arm { get; private set; }

      public void Connect(SerialPortAddress port, string configurationPath )
      {
         ArmConfiguration config = ArmConfiguration.LoadArmConfig(configurationPath);
         Arm = new Arm(port, config);
      }

      public static SerialPortAddress[] GetSerialPorts()
      {
         return HardwareService.GetSerialPorts();
      }

   }
}
