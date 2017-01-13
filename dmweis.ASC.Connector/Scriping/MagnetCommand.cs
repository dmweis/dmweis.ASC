using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dmweis.ASC.Connector.Scriping
{
   class MagnetCommand : ArmCommand
   {
      public bool MagnetOn { get; set; }

      public MagnetCommand( bool turnOn )
      {
         MagnetOn = turnOn;
      }

      public MagnetCommand()
      {
      }
   }
}
