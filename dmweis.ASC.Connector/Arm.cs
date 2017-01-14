using System;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using dmweis.ASC.Connector.HardwareConnection;
using dmweis.ASC.Connector.Scriping;

namespace dmweis.ASC.Connector
{
   public class Arm : ArmBase
   {
      public override double MaxArmReach { get; }

      private readonly IArmConnector m_ArmConnector;
      private readonly ArmConfiguration m_Configuration;

      public Arm( SerialPortAddress portAddress, string configurationFilePath ) : this( portAddress.Name, ArmConfiguration.LoadArmConfig( configurationFilePath ) )
      {

      }

      public Arm( string portName, string configurationFilePath ) : this( portName, ArmConfiguration.LoadArmConfig( configurationFilePath ) )
      {

      }

      public Arm( SerialPortAddress portAddress, ArmConfiguration configuration ) : this( portAddress.Name, configuration )
      {

      }

      public Arm( string portName, ArmConfiguration configuration )
      {
         m_Configuration = (ArmConfiguration) configuration.Clone();
         m_ArmConnector = new ArmConnector( portName );
         MaxArmReach = m_Configuration.ElbowLength + m_Configuration.ShoulderLength + m_Configuration.EndEffectorLength;
      }

      public override async Task MoveToCartesianAsync( ArmPosition position )
      {
         await MoveToCartesianAsync( position.X, position.Y, position.Z );
      }

      /// <summary>
      /// Main Cartesion move method
      /// </summary>
      /// <param name="x"></param>
      /// <param name="y"></param>
      /// <param name="z"></param>
      /// <returns></returns>
      public override async Task MoveToCartesianAsync( double x, double y, double z )
      {
         ServoPositions servoPositions = CalculateServosFromPosition( x, y, z );
         ServoPositions convertedServoPositions = ConvertToAbsoluteServoAnglesOrPwm( servoPositions );
         await MoveToConvertedAnglesOrPwmAsync( convertedServoPositions );
      }

      /// <summary>
      /// Main realtive move method
      /// </summary>
      /// <param name="baseAngle"></param>
      /// <param name="distance"></param>
      /// <param name="z"></param>
      /// <returns></returns>
      public override async Task MoveToRelativeAsync( double baseAngle, double distance, double z )
      {
         VerticalServoPositions verticalServoPositions = CalculateVerticalServoPositions( distance, z );
         ServoPositions convertedServoAnglesOrPwm = ConvertToAbsoluteServoAnglesOrPwm( new ServoPositions( baseAngle, verticalServoPositions ) );
         await MoveToConvertedAnglesOrPwmAsync( convertedServoAnglesOrPwm );
      }

      public override async Task SetMagnetAsync( bool on )
      {
         await m_ArmConnector.SetMagnetAsync( on );
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

      /// <summary>
      /// procedure to convert from information from the arm to wanted angles
      /// </summary>
      /// <param name="servoPositions"></param>
      /// <returns></returns>
      private ServoPositions ConvertToAnglesFromServoOrPwm( ServoPositions servoPositions )
      {
         double baseAngle = m_Configuration.PwmToAngleForBase( servoPositions.Base );
         double shoulderAngle = m_Configuration.PwmToAngleForShoulder( servoPositions.Shoulder );
         double elbowAngle = m_Configuration.PwmToAngleForElbow( servoPositions.Elbow );
         return new ServoPositions( baseAngle, shoulderAngle, elbowAngle );
      }

      private ArmPosition CalculateArmPositionFromServoAngles(double baseAngle, double shoulderAngle,
         double groundToLevelAngle)
      {
         double Le = m_Configuration.ElbowLength;
         double Ls = m_Configuration.ShoulderLength;
         double endEffectorLenght = m_Configuration.EndEffectorLength;

         double levelLen1 = Math.Cos(shoulderAngle) * Ls;
         double levelLen2 = Math.Cos(groundToLevelAngle) * Le;
         double distance = levelLen1 + levelLen2 + endEffectorLenght;
         throw new NotImplementedException();
      }

      private async Task MoveToConvertedAnglesOrPwmAsync( ServoPositions servoPwmOrAngles )
      {
         TaskCompletionSource<bool> completionSource = new TaskCompletionSource<bool>();
         EventHandler<ArmDataUpdateEvent> callback = null;
         callback = ( sender, armData ) =>
         {
            if (!armData.LastSent.RelativeEquals(servoPwmOrAngles))
            {
               completionSource.SetResult( false );
               m_ArmConnector.NewServoPosition -= callback;
            }
            else if( armData.Current.RelativeEquals( armData.LastSent ) )
            {
               completionSource.SetResult(true);
               m_ArmConnector.NewServoPosition -= callback;
            }
         };
         await m_ArmConnector.MoveAllServosAsync( servoPwmOrAngles );
         m_ArmConnector.NewServoPosition += callback;
         await completionSource.Task;
      }

   }
}
