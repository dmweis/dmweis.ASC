using System.IO;
using System.Xml.Serialization;

namespace dmweis.ASC.Connector
{
   public class ArmConfiguration
   {

      public double EndEffectorLength { get; set; }
      public double ShoulderLength { get; set; }
      public double ElbowLength { get; set; }

      public CalibrationPair BaseMin { get; set; }
      public CalibrationPair BaseMax { get; set; }
      public CalibrationPair ShoulderMin { get; set; }
      public CalibrationPair ShoulderMax { get; set; }
      public CalibrationPair ElbowMin { get; set; }
      public CalibrationPair ElbowMax { get; set; }

      public ArmConfiguration()
      {
         BaseMin = new CalibrationPair();
         BaseMax = new CalibrationPair();
         ShoulderMin = new CalibrationPair();
         ShoulderMax = new CalibrationPair();
         ElbowMin = new CalibrationPair();
         ElbowMax = new CalibrationPair();
      }

      public double AngleToPwmForBase( double angle )
      {
         return Map( angle, BaseMin.Angle, BaseMax.Angle, BaseMin.Pwm, BaseMax.Pwm );
      }

      public double AngleToPwmForShoulder( double angle )
      {
         return Map( angle, ShoulderMin.Angle, ShoulderMax.Angle, ShoulderMin.Pwm, ShoulderMax.Pwm );
      }

      public double AngleToPwmForElbow( double angle )
      {
         return Map( angle, ElbowMin.Angle, ElbowMax.Angle, ElbowMin.Pwm, ElbowMax.Pwm );
      }

      private static double Map( double value, double inMin, double inMax, double outMin, double outMax )
      {
         return ( value - inMin ) * ( outMax - outMin ) / ( inMax - inMin ) + outMin;
      }

      public static ArmConfiguration LoadArmConfig( string path )
      {
         using( FileStream file = File.OpenRead( path ) )
         {
            XmlSerializer serializer = new XmlSerializer( typeof( ArmConfiguration ) );
            return serializer.Deserialize( file ) as ArmConfiguration;
         }
      }

      public static void SaveArmConfig( string path, ArmConfiguration configuration )
      {
         using( FileStream file = File.Create( path ) )
         {
            XmlSerializer serializer = new XmlSerializer( typeof( ArmConfiguration ) );
            serializer.Serialize( file, configuration );
         }
      }

   }

   public class CalibrationPair
   {
      public double Pwm { get; set; }
      public double Angle { get; set; }
   }
}
