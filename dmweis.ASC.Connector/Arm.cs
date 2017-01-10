using System;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;
using dmweis.ASC.Connector.Scriping;
using System.Text;

namespace dmweis.ASC.Connector
{
   public class Arm : IArm
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

      public Task MoveToCartesianAsync( ArmPosition position )
      {
         return MoveToCartesianAsync( position.X, position.Y, position.Z );
      }

      /// <summary>
      /// Main Cartesion move method
      /// </summary>
      /// <param name="x"></param>
      /// <param name="y"></param>
      /// <param name="z"></param>
      /// <returns></returns>
      public Task MoveToCartesianAsync( double x, double y, double z )
      {
         ServoPositions servoPositions = CalculateServosFromPosition( x, y, z );
         ServoPositions convertedServoPositions = ConvertToAbsoluteServoAnglesOrPwm( servoPositions );
         return MoveToConvertedAnglesOrPwm( convertedServoPositions );
      }

      /// <summary>
      /// Main realtive move method
      /// </summary>
      /// <param name="baseAngle"></param>
      /// <param name="distance"></param>
      /// <param name="z"></param>
      /// <returns></returns>
      public Task MoveToRelative( double baseAngle, double distance, double z )
      {
         VerticalServoPositions verticalServoPositions = CalculateVerticalServoPositions( distance, z );
         ServoPositions convertedServoAnglesOrPwm = ConvertToAbsoluteServoAnglesOrPwm( new ServoPositions( baseAngle, verticalServoPositions ) );
         return MoveToConvertedAnglesOrPwm( convertedServoAnglesOrPwm );
      }

      public Task SetMagnet( bool on )
      {
         // 2 is the command number
         return SendMessage( new byte[] { 2, (byte) (on ? 1 : 0) } );
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

      /// <summary>
      /// Function to calculate distance of target
      /// </summary>
      /// <param name="x"></param>
      /// <param name="y"></param>
      /// <returns></returns>
      private static double CalcualteDistance( double x, double y )
      {
         return Math.Sqrt( x.Square() + y.Square() );
      }

      /// <summary>
      /// Function to calculate Angle of base
      /// </summary>
      /// <param name="x"></param>
      /// <param name="y"></param>
      /// <returns></returns>
      private static double CalculateAngleOfBase( double x, double y )
      {
         // base angle
         // Use Atan2 to doctor for cases in which y is negative
         return Math.Atan2( x, y ).RadToDegree();
      }

      /// <summary>
      /// procedure for calculating servo angles for a position
      /// </summary>
      /// <param name="x"></param>
      /// <param name="y"></param>
      /// <param name="z"></param>
      /// <returns></returns>
      private ServoPositions CalculateServosFromPosition( double x, double y, double z )
      {
         double angleBase = CalculateAngleOfBase( x, y );
         double distance = CalcualteDistance( x, y );

         VerticalServoPositions verticalServoPositions = CalculateVerticalServoPositions( distance, z );
         return new ServoPositions( angleBase, verticalServoPositions.Shoulder, verticalServoPositions.Elbow );
      }

      /// <summary>
      /// Procedure to calculate angles for vertical servo positions
      /// </summary>
      /// <param name="distance"></param>
      /// <param name="z"></param>
      /// <returns></returns>
      private VerticalServoPositions CalculateVerticalServoPositions( double distance, double z )
      {
         double Le = m_Configuration.ElbowLength;
         double Ls = m_Configuration.ShoulderLength;
         double endEffectorLenght = m_Configuration.EndEffectorLength;
         // Remove end effector lenght from the relative distance
         double distanceWithouthEndEffector = distance - endEffectorLenght;

         if( distanceWithouthEndEffector > Ls + Le )
         {
            throw new InvalidOperationException( "Distance is furthere than arm can reach" );
         }

         // shoulder angle
         double bottomAngle = Math.Atan( z / distanceWithouthEndEffector ).RadToDegree();
         double cAngle = Math.Sqrt( z.Square() + distanceWithouthEndEffector.Square() );
         double upperAngle = Math.Acos( (Ls.Square() + cAngle.Square() - Le.Square()) / (2.0 * Ls * cAngle) ).RadToDegree();
         double shoulderAngle = bottomAngle + upperAngle;

         // elbow angle
         double elbowAngle = Math.Acos( (Le.Square() + Ls.Square() - cAngle.Square()) / (2.0 * Le * Ls) ).RadToDegree();

         // elbow to ground angle
         double removedAngle = 90.0 - Math.Abs( Math.Atan( distanceWithouthEndEffector / z ).RadToDegree() );
         // end stupid trick
         double gripAngle = Math.Acos( (cAngle.Square() + Le.Square() - Ls.Square()) / (2.0 * Le * cAngle) ).RadToDegree();
         // handle when z is bellow 0 by adding the correction angle
         double elbowToGround = z >= 0 ? gripAngle - removedAngle : gripAngle + removedAngle;
         return new VerticalServoPositions( shoulderAngle, elbowToGround );
      }

      /// <summary>
      /// procedure to convert idel angles to current arm angles
      /// </summary>
      /// <param name="servoPositions"></param>
      /// <returns></returns>
      private ServoPositions ConvertToAbsoluteServoAnglesOrPwm( ServoPositions servoPositions )
      {
         double basePwm = m_Configuration.AngleToPwmForBase( servoPositions.Base );
         double shoulderPwm = m_Configuration.AngleToPwmForShoulder( servoPositions.Shoulder );
         double elbowPwm = m_Configuration.AngleToPwmForElbow( servoPositions.Elbow );
         return new ServoPositions( basePwm, shoulderPwm, elbowPwm );
      }

      private Task MoveToConvertedAnglesOrPwm( ServoPositions servoPwmOrAngles )
      {
         byte[] message = SerializeFullServoCommand( servoPwmOrAngles );
         return SendMessage( message );
      }

      /// <summary>
      /// internal send methood
      /// </summary>
      /// <param name="message"></param>
      /// <returns></returns>
      private Task SendMessage( byte[] message )
      {
         return m_Arduino.WriteBytesAsync( message );
      }

      private byte[] SerializeFullServoCommand( ServoPositions servoPwmOrAngles )
      {
         byte[] byteArray = new byte[ 7 ];
         byteArray[ 0 ] = 1; // command number
         int pwm = servoPwmOrAngles.Base.RoundToInt();
         byteArray[ 1 ] = (byte) ((pwm >> 8) & 0xFF);
         byteArray[ 2 ] = (byte) (pwm & 0xFF);
         pwm = servoPwmOrAngles.Shoulder.RoundToInt();
         byteArray[ 3 ] = (byte) ((pwm >> 8) & 0xFF);
         byteArray[ 4 ] = (byte) (pwm & 0xFF);
         pwm = servoPwmOrAngles.Elbow.RoundToInt();
         byteArray[ 5 ] = (byte) ((pwm >> 8) & 0xFF);
         byteArray[ 6 ] = (byte) (pwm & 0xFF);
         return byteArray;
      }

      private byte[] SerializeSingleServoCommand( int servoIndex, int pwm )
      {
         byte[] byteArray = new byte[ 4 ];
         byteArray[ 0 ] = 0; // command number
         byteArray[ 1 ] = (byte) servoIndex;
         byteArray[ 2 ] = (byte) ((pwm >> 8) & 0xFF);
         byteArray[ 3 ] = (byte) (pwm & 0xFF);
         return byteArray;
      }
   }
}
