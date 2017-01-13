using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dmweis.ASC.Connector.Scriping
{
   public class DelayCommand : ArmCommand
   {
      public TimeSpan DelayTime { get; set; }

      public DelayCommand( TimeSpan delay )
      {
         DelayTime = delay;
      }

      public DelayCommand()
      {
      }
   }
}
