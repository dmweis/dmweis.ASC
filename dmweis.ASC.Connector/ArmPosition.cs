using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dmweis.ASC.Connector
{
   public class ArmPosition
   {
      public string Comment { get; set; }
      public int WaitBefore { get; set; }
      public int WaitAfter { get; set; }
      public int? Base { get; set; }
      public int? Shoulder { get; set; }
      public int? Elbow { get; set; }
      public bool? Magnet { get; set; }
   }
}
