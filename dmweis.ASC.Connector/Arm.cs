using System;
using System.IO.Ports;
using System.Threading.Tasks;
using dmweis.ASC.Connector.HardwareConnection;
using dmweis.ASC.Connector.Scriping;

namespace dmweis.ASC.Connector
{
   public class Arm : IArm
   {

      private IArmConnector m_ArmConnector;
      private ArmConfiguration m_Configuration;

      public Arm( SerialPortAddress portAddress, string configurationFilePath ) : this( portAddress.Name, configurationFilePath )
      {
         
      }

      public Arm( string portAddress, string configurationFilePath )
      {
         m_Configuration = ArmConfiguration.LoadArmConfig( configurationFilePath );
         m_ArmConnector = new ArmConnector( portAddress );
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
         return MoveToConvertedAnglesOrPwmAsync( convertedServoPositions );
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
         return MoveToConvertedAnglesOrPwmAsync( convertedServoAnglesOrPwm );
      }

      public async Task SetMagnetAsync( bool on )
      {
         // 2 is the command number
         await m_ArmConnector.SetMagnetAsync(on);
      }

      public async Task<Arm> ExecuteScriptAsync( ArmScript script )
      {
         foreach( ArmCommand position in script.Movements )
         {
            await ExecuteCommandAsync( position );
         }
         return this;
      }

      public async Task<Arm> ExecuteCommandAsync( ArmCommand command, bool ignoreTimeouts = false )
      {
         command.Executed = true;
         if( !ignoreTimeouts )
         {
            await Task.Delay( command.WaitBefore );
         }
         if (command.Magnet.HasValue)
         {
            await SetMagnetAsync(command.Magnet.Value);
         }
         await MoveToCartesianAsync(command.Position);
         if( !ignoreTimeouts )
         {
            await Task.Delay( command.WaitAfter );
         }
         command.Executed = false;
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

      private async Task MoveToConvertedAnglesOrPwmAsync( ServoPositions servoPwmOrAngles )
      {
         await m_ArmConnector.MoveAllServosAsync( servoPwmOrAngles.Base.RoundToInt(),
            servoPwmOrAngles.Shoulder.RoundToInt(),
            servoPwmOrAngles.Elbow.RoundToInt() );
      }

   }
}
