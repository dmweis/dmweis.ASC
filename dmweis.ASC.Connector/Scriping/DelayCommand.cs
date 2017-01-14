using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dmweis.ASC.Connector.Scriping
{
   public class DelayCommand : ArmCommand
   {

      private TimeSpan m_DelayTime;

      public TimeSpan DelayTime
      {
         get { return m_DelayTime; }
         set
         {
            m_DelayTime = value;
            RaisePropertyChanged();
         }
      }


      public DelayCommand( TimeSpan delay )
      {
         DelayTime = delay;
      }

      public DelayCommand()
      {
      }
   }
}
