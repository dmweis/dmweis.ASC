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
         position.Executed = true;
         if( !ignoreTimeouts )
         {
            await Task.Delay( position.WaitBefore );
         }
         byte[] message = new byte[ 0 ];
         if( position.Elbow != null )
         {
            byte[] movementCommand = CommandToMessage( c_ElbowIndex, position.Elbow.Value );
            message = message.Concat( movementCommand ).ToArray();
         }
         if( position.Shoulder != null )
         {
            byte[] movementCommand = CommandToMessage( c_ShoulderIndex, position.Shoulder.Value );
            message = message.Concat( movementCommand ).ToArray();
         }
         if( position.Base != null )
         {
            byte[] movementCommand = CommandToMessage( c_BaseIndex, position.Base.Value );
            message = message.Concat( movementCommand ).ToArray();
         }
         if( position.Magnet == true )
         {
            byte[] movementCommand = CommandToMessage( c_MagnetOn, 300 );
            message = message.Concat( movementCommand ).ToArray();
         }
         else if( position.Magnet == false )
         {
            byte[] movementCommand = CommandToMessage( c_MagnetOff, 300 );
            message = message.Concat( movementCommand ).ToArray();
         }
         await SendMessage( message );
         if( !ignoreTimeouts )
         {
            await Task.Delay( position.WaitAfter );
         }
         position.Executed = false;
         return this;
      }

      private byte[] CommandToMessage( int servoIndex, int pwm )
      {
         byte[] byteArray = new byte[ 3 ];
         byteArray[ 0 ] = (byte) servoIndex;
         byteArray[ 1 ] = (byte) ((pwm >> 8) & 0xFF);
         byteArray[ 2 ] = (byte) (pwm & 0xFF);
         return byteArray;
      }

      private Task SendMessage( byte[] message )
      {
         return m_Arduino.WriteBytesAsync( message );
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
