﻿using System;
using System.IO;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace dmweis.ASC.Connector
{
   public class ArmConfiguration : ICloneable
   {

      public double EndEffectorLength { get; set; }
      public double BaseToShoulderDistance { get; set; }
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

      #region AngleToPwm
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
      #endregion

      #region PwmToAngle
      public double PwmToAngleForBase(double pwm)
      {
         return Map(pwm, BaseMin.Pwm, BaseMax.Pwm, BaseMin.Angle, BaseMax.Angle);
      }

      public double PwmToAngleForShoulder(double pwm)
      {
         return Map(pwm, ShoulderMin.Pwm, ShoulderMax.Pwm, ShoulderMin.Angle, ShoulderMax.Angle);
      }

      public double PwmToAngleForElbow(double pwm)
      {
         return Map(pwm, ElbowMin.Pwm, ElbowMax.Pwm, ElbowMin.Angle, ElbowMax.Angle);
      }
      #endregion

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

      public void SaveArmConfigAsJson(string path)
      {
         File.WriteAllText(path, JsonConvert.SerializeObject(this));
      }

      public object Clone()
      {
         ArmConfiguration newObj = new ArmConfiguration
         {
            EndEffectorLength = EndEffectorLength,
            BaseToShoulderDistance = BaseToShoulderDistance,
            ShoulderLength = ShoulderLength,
            ElbowLength = ElbowLength,
            BaseMin = BaseMin.Clone() as CalibrationPair,
            BaseMax = BaseMax.Clone() as CalibrationPair,
            ShoulderMin = ShoulderMin.Clone() as CalibrationPair,
            ShoulderMax = ShoulderMax.Clone() as CalibrationPair,
            ElbowMin = ElbowMin.Clone() as CalibrationPair,
            ElbowMax = ElbowMax.Clone() as CalibrationPair
         };
         return newObj;
      }
   }

   public class CalibrationPair : ICloneable
   {
      public double Pwm { get; set; }
      public double Angle { get; set; }
      public object Clone()
      {
         CalibrationPair newObj = new CalibrationPair
         {
            Pwm = Pwm,
            Angle = Angle
         };
         return newObj;
      }
   }
}
