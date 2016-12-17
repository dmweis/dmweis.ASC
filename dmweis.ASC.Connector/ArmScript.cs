using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace dmweis.ASC.Connector
{
   public class ArmScript
   {
      public List<ArmPosition> Movements { get; set; }
      public ArmScript()
      {
         Movements = new List<ArmPosition>();
      }

      public static ArmScript ReadArmScript( string path )
      {
         ArmScript script;
         using( FileStream file = File.OpenRead( path ) )
         {
            XmlSerializer serializer = new XmlSerializer( typeof( ArmScript ) );
            script = serializer.Deserialize( file ) as ArmScript;
         }
         return script;
      }

      public static void WriteArmScript( string path, ArmScript script )
      {
         using( FileStream file = File.Create( path ) )
         {
            XmlSerializer serializer = new XmlSerializer( typeof( ArmScript ) );
            serializer.Serialize( file, script );
         }
      }
   }
}
