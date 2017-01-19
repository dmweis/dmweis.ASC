using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Aruco.Net;
using dmweis.ASC.Connector;
using OpenCV.Net;

namespace dmweis.ASC.CameraTracker
{
   public class CameraController
   {
      private const int _MarkerIndex = 42;

      private double armHeight = 9.8;
      private double armAngle = 0.0;
      private double armDistance = 20.0;

      private ArmBase _Arm;
      private NamedWindow _Window;
      private readonly CancellationTokenSource cancellationTokenSource =
      new CancellationTokenSource();

      public CameraController( ArmBase arm )
      {
         _Arm = arm;
         _Window = new NamedWindow( "Arm camera", WindowFlags.KeepRatio );
         _Arm?.MoveToRelativeAsync( armAngle, armDistance, armHeight );
         Task.Factory.StartNew( CameraLoop );
         _Window.SetMouseCallback(OnMOuseCallback );
      }

      private void OnMOuseCallback(MouseEvent evt, int x, int y, MouseEventFlags flags)
      {
         if (flags == MouseEventFlags.LButton)
         {
            
         }
      }

      public void Close()
      {
         _Arm = null;
         cancellationTokenSource.Cancel();
         _Window = null;
         NamedWindow.DestroyAllWindows();
      }

      private void CameraLoop()
      {
         CancellationToken cancellationToken = cancellationTokenSource.Token;
         var parameters = new CameraParameters();
         Size size;
         var cameraMatrix = new Mat( 3, 3, Depth.F32, 1 );
         var distortion = new Mat( 1, 4, Depth.F32, 1 );
         parameters.CopyParameters( cameraMatrix, distortion, out size );
         using( var detector = new MarkerDetector() )
         {
            detector.ThresholdMethod = ThresholdMethod.AdaptiveThreshold;
            detector.Param1 = 7.0;
            detector.Param2 = 7.0;
            detector.MinSize = 0.04f;
            detector.MaxSize = 0.5f;
            detector.CornerRefinement = CornerRefinementMethod.Lines;
            var markerSize = 10;
            using( var capture = Capture.CreateCameraCapture( 0 ) )
            {
               while( !cancellationToken.IsCancellationRequested )
               {
                  IplImage image = capture.QueryFrame();
                  var detectedMarkers = detector.Detect( image, cameraMatrix, distortion, markerSize );
                  foreach( var marker in detectedMarkers )
                  {
                     if( marker.Id == _MarkerIndex )
                     {
                        double sideOffset = Map( marker.Center.X, 46, 590, -8, 8 );
                        double hightOffset = Map( marker.Center.Y, 46, 435, 6, -6 );
                        double height = Map( marker.Area, 6280, 10150, 18.5, 14 );
                        armDistance += hightOffset > 1.0 ? 0.4 : (hightOffset < -1.0 ? -0.4 : 0);
                        armAngle += sideOffset > 1.0 ? 0.7 : (sideOffset < -1.0 ? -0.7 : 0);
                        //armDistance += hightOffset;
                        _Arm?.MoveToRelativeAsync( armAngle, armDistance, armHeight );
                        marker.Draw( image, Scalar.Rgb( 1, 0, 0 ) );
                     }
                     else
                     {
                        marker.Draw( image, Scalar.Rgb( 0, 1, 0 ) );
                     }
                  }
                  WindwoDisplay( image );
               }
            }
         }
      }

      private void WindwoDisplay( IplImage image )
      {
         Task.Run( () =>
          {
             IplImage imageCopy = image.Clone();
             _Window?.ShowImage( imageCopy );
             CV.WaitKey( 1 );
          } );
      }

      private static double Map( double value, double inMin, double inMax, double outMin, double outMax )
      {
         return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
      }
   }
}
