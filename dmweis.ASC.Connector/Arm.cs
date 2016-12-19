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

      public Task<bool> MoveToCartesianAsync( ArmPosition position )
      {
         return MoveToCartesianAsync( position.X, position.Y, position.Z );
      }

      public async Task<bool> MoveTo2DCartesianAsync( double angleBase, double distance, double z )
      {
         double Le = m_Configuration.ElbowLength;
         double Ls = m_Configuration.ShoulderLength;

         if( distance > Ls + Le )
         {
            return false;
         }

         // shoulder angle
         double bottomAngle = Math.Atan( z / distance ).RadToDegree();
         double cAngle = Math.Sqrt( z.Square() + distance.Square() );
         double upperAngle = Math.Acos( (Ls.Square() + cAngle.Square() - Le.Square()) / (2.0 * Ls * cAngle) ).RadToDegree();
         double shoulderAngle = bottomAngle + upperAngle;

         // elbow angle
         double elbowAngle = Math.Acos( (Le.Square() + Ls.Square() - cAngle.Square()) / (2.0 * Le * Ls) ).RadToDegree();

         // elbow to ground angle
         double removedAngle = 90.0 - Math.Abs( Math.Atan( distance / z ).RadToDegree() );
         // end stupid trick
         double gripAngle = Math.Acos( (cAngle.Square() + Le.Square() - Ls.Square()) / (2.0 * Le * cAngle) ).RadToDegree();
         // handle when z is bellow 0 by adding the correction angle
         double elbowToGround = z >= 0 ? gripAngle - removedAngle : gripAngle + removedAngle;
         await MoveToAnglesAsync( angleBase, shoulderAngle, elbowToGround );
         return true;
      }

      public async Task<bool> MoveToCartesianAsync( double x, double y, double z )
      {
         double Le = m_Configuration.ElbowLength;
         double Ls = m_Configuration.ShoulderLength;

         // base angle
         // Use Atan2 to doctor for cases in which y is negative
         double angleBase = Math.Atan2( x, y ).RadToDegree();
         double distance = Math.Sqrt( x.Square() + y.Square() );

         if( distance > Ls + Le )
         {
            return false;
         }

         // shoulder angle
         double bottomAngle = Math.Atan( z / distance ).RadToDegree();
         double cAngle = Math.Sqrt( z.Square() + distance.Square() );
         double upperAngle = Math.Acos( (Ls.Square() + cAngle.Square() - Le.Square()) / (2.0 * Ls * cAngle) ).RadToDegree();
         double shoulderAngle = bottomAngle + upperAngle;

         // elbow angle
         double elbowAngle = Math.Acos( (Le.Square() + Ls.Square() - cAngle.Square()) / (2.0 * Le * Ls) ).RadToDegree();

         // elbow to ground angle
         double removedAngle = 90.0 - Math.Abs( Math.Atan( distance / z ).RadToDegree() );
         // end stupid trick
         double gripAngle = Math.Acos( (cAngle.Square() + Le.Square() - Ls.Square()) / (2.0 * Le * cAngle) ).RadToDegree();
         // handle when z is bellow 0 by adding the correction angle
         double elbowToGround = z >= 0 ? gripAngle - removedAngle : gripAngle + removedAngle;
         await MoveToAnglesAsync( angleBase, shoulderAngle, elbowToGround );
         return true;
      }

      private Task MoveToAnglesAsync( double baseAngle, double shoulderAngle, double gripToPlaneAngle )
      {
         double basePwm = m_Configuration.AngleToPwmForBase( baseAngle );
         double shoulderPwm = m_Configuration.AngleToPwmForShoulder( shoulderAngle );
         double elbowPwm = m_Configuration.AngleToPwmForElbow( gripToPlaneAngle );
         byte[] message = new byte[ 0 ];
         message = message.
            Concat( CommandToMessage( c_BaseIndex, basePwm.RoundToInt() ) ).
            Concat( CommandToMessage( c_ShoulderIndex, shoulderPwm.RoundToInt() ) ).
            Concat( CommandToMessage( c_ElbowIndex, elbowPwm.RoundToInt() ) ).ToArray();
         return SendMessage( message );
      }

      public Task SetMagnet( bool on )
      {
         return SetServoAsync( on ? c_MagnetOn : c_MagnetOff, 300 );
      }

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
         
         if( !ignoreTimeouts )
         {
            await Task.Delay( position.WaitAfter );
         }
         position.Executed = false;
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

   }
}
