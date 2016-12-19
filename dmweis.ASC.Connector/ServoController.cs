using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dmweis.ASC.Connector
{
   public class ServoController
   {
      private SerialPort m_Arduino;

      public ServoController( string comPort )
      {
         m_Arduino = new SerialPort( comPort, 9600 )
         {
            DtrEnable = false
         };
         m_Arduino.Open();
      }

      public Task SetServoAsync( int numberOfServo, int pwm )
      {
         byte[] byteArray = new byte[ 3 ];
         byteArray[ 0 ] = (byte) numberOfServo;
         byteArray[ 1 ] = (byte) ((pwm >> 8) & 0xFF);
         byteArray[ 2 ] = (byte) (pwm & 0xFF);
         return m_Arduino.WriteBytesAsync( byteArray );
      }
   }
}
