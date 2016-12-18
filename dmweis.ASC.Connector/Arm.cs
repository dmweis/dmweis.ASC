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
      private const int c_ElbowIndex = 2;
      private const int c_MagnetOn = 20;
      private const int c_MagnetOff = 21;

      private SerialPort m_Arduino;
      private ArmConfiguration m_Configuration;

      public Arm( string port, string configurationFilePath )
      {
         m_Configuration = ArmConfiguration.LoadArmConfig( configurationFilePath );
         m_Arduino = new SerialPort( port )
         {
            DtrEnable = false
         };
         m_Arduino.Open();
      }
      public void MoveToCartesian( double x, double yOrigin, double z )
      {
         double Le = m_Configuration.ElbowLength;
         double Ls = m_Configuration.ShoulderLength;

         // base angle
         double angleBase = Math.Atan( x / yOrigin ).RadToDegree();
         double yOffset = Math.Sqrt( x.Square() + yOrigin.Square() );

         // shoulder angle
         double bottomAngle = Math.Atan( z / yOffset ).RadToDegree();
         double cAngle = Math.Sqrt( z.Square() + yOffset.Square() );
         double upperAngle = Math.Acos( (Ls.Square() + cAngle.Square() - Le.Square()) / (2.0 * Ls * cAngle) ).RadToDegree();
         double shoulderAngle = bottomAngle + upperAngle;

         // elbow angle
         double elbowAngle = Math.Acos( (Le.Square() + Ls.Square() - cAngle.Square()) / (2.0 * Le * Ls) ).RadToDegree();

         // elbow to ground angle
         double removedAngle = 90.0 - Math.Abs( Math.Atan( yOffset / z ).RadToDegree() );
         // end stupid trick
         double gripAngle = Math.Acos( (cAngle.Square() + Le.Square() - Ls.Square()) / (2.0 * Le * cAngle) ).RadToDegree();
         double elbowToGround = z >= 0 ? gripAngle - removedAngle : gripAngle + removedAngle;
         AnglesToPwms( angleBase, shoulderAngle, elbowToGround );
      }

      private void AnglesToPwms( double baseAngle, double shoulderAngle, double elbowAngle )
      {
         double basePwm = m_Configuration.AngleToPwmForBase( baseAngle );
         double shoulderPwm = m_Configuration.AngleToPwmForShoulder( shoulderAngle );
         double elbowPwm = m_Configuration.AngleToPwmForElbow( elbowAngle );
         byte[] message = new byte[ 0 ];
         message = message.
            Concat( CommandToMessage( c_BaseIndex, (int) Math.Round( basePwm ) ) ).
            Concat( CommandToMessage( c_ShoulderIndex, (int) Math.Round( shoulderPwm ) ) ).
            Concat( CommandToMessage( c_ElbowIndex, (int) Math.Round( elbowPwm ) ) ).ToArray();
         SendMessage( message ).Wait();
      }

      private Task SetServoAsync( int servoIndex, int pwm )
      {
         byte[] byteArray = new byte[ 3 ];
         byteArray[ 0 ] = (byte) servoIndex;
         byteArray[ 1 ] = (byte) ((pwm >> 8) & 0xFF);
         byteArray[ 2 ] = (byte) (pwm & 0xFF);
         return m_Arduino.WriteBytesAsync( byteArray );
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

      public Task SetMagnet( bool on )
      {
         return SetServoAsync( on ? c_MagnetOn : c_MagnetOff, 300 );
      }

      #region pwm commands
      public async Task<Arm> ExecuteScriptAsync( ArmScript script )
      {
         foreach( ArmCommand position in script.Movements )
         {
            await MoveToAsync( position );
         }
         return this;
      } 

      public async Task<Arm> MoveToAsync( ArmCommand position, bool ignoreTimeouts = false )
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

      #endregion

   }
}
