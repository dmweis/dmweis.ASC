using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dmweis.ASC.Connector
{
   class VerticalServoPositions
   {
      public double Shoulder { get; }
      public double Elbow { get; }

      public VerticalServoPositions( double shoulder, double elbow )
      {
         Shoulder = shoulder;
         Elbow = elbow;
      }
   }
}
