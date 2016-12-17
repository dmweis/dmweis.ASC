using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dmweis.ASC.Connector
{
   public class Arm
   {
      private const int c_BaseIndex = 0;
      private const int c_ShoulderIndex = 1;
      private const int c_ElbowIndex = 3;
      private const int c_MagnetOn = 20;
      private const int c_MagnetOff = 21;

      private SerialPort m_Arduino;

      public Arm( string port )
      {
         m_Arduino = new SerialPort( port )
         {
            DtrEnable = false
         };
         m_Arduino.Open();
      }

      public async Task<Arm> ExecuteScriptAsync( ArmScript script )
      {
         foreach( ArmPosition position in script.Movements )
         {
            await MoveToAsync( position );
         }
         return this;
      } 

      public async Task<Arm> MoveToAsync( ArmPosition position, bool ignoreTimeouts = false )
      {
         if( !ignoreTimeouts )
         {
            await Task.Delay( position.WaitBefore );
         }
         if( position.Base != null )
         {
            await SetServoAsync( c_BaseIndex, position.Base.Value );
         }
         if( position.Shoulder != null )
         {
            await SetServoAsync( c_ShoulderIndex, position.Shoulder.Value );
         }
         if( position.Elbow != null )
         {
            await SetServoAsync( c_ElbowIndex, position.Elbow.Value );
         }
         if( position.Magnet == true )
         {
            await SetServoAsync( c_MagnetOn, 300 );
         }
         else if( position.Magnet == false )
         {
            await SetServoAsync( c_MagnetOff, 300 );
         }
         if( !ignoreTimeouts )
         {
            await Task.Delay( position.WaitAfter );
         }
         return this;
      }

      private Task SetServoAsync( int servoIndex, int pwm )
      {
         byte[] byteArray = new byte[ 3 ];
         byteArray[ 0 ] = (byte) servoIndex;
         byteArray[ 1 ] = (byte) ((pwm >> 8) & 0xFF);
         byteArray[ 2 ] = (byte) (pwm & 0xFF);
         return m_Arduino.WriteBytesAsync( byteArray );
      }
   }
}
