using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace dmweis.ASC.ArmController
{
   /// <summary>
   /// Interaction logic for ArmControllerView.xaml
   /// </summary>
   public partial class ArmControllerView : UserControl
   {

      private readonly Line _line;
      private readonly Ellipse _ArmCircle;
      private readonly Ellipse _EndEffector;
      private Point _lastSentPosition;

      public ArmControllerView()
      {
         InitializeComponent();
         _line = new Line()
         {
            Stroke = Brushes.Gray,
            StrokeThickness = 2
         };
         _ArmCircle = new Ellipse
         {
            Stroke = Brushes.Red,
            StrokeThickness = 1,
            Width = 70,
            Height = 70
         };
         _EndEffector = new Ellipse
         {
            Stroke = Brushes.DarkBlue,
            StrokeThickness = 1,
            Width = 20,
            Height = 20
         };
         ArmCanvas.Children.Add(_line);
         ArmCanvas.Children.Add( _ArmCircle );
         ArmCanvas.Children.Add( _EndEffector );
         _lastSentPosition = new Point(0, 0);
      }

      private void ArmCanvas_OnMouseDown(object sender, MouseButtonEventArgs e)
      {
         
         if (e.LeftButton == MouseButtonState.Pressed)
         {
            Point position = e.GetPosition( ArmCanvas );
            Point relativePosition = new Point();
            relativePosition.X = 100 / ArmCanvas.ActualWidth * position.X - 50;
            relativePosition.Y = -50 / ArmCanvas.ActualHeight * position.Y + 50;
            CheckMoveArm(relativePosition, true);
            _line.X2 = position.X;
            _line.Y2 = position.Y;
            Canvas.SetLeft(_EndEffector, position.X - 10);
            Canvas.SetTop(_EndEffector, position.Y - 10);
         }
      }

      private void ArmCanvas_OnMouseMove(object sender, MouseEventArgs e)
      {
         if( e.LeftButton == MouseButtonState.Pressed )
         {
            Point position = e.GetPosition( ArmCanvas );
            Point relativePosition = new Point();
            relativePosition.X = 100 / ArmCanvas.ActualWidth * position.X - 50;
            relativePosition.Y = -50 / ArmCanvas.ActualHeight * position.Y + 50;
            CheckMoveArm(relativePosition);
            _line.X2 = position.X;
            _line.Y2 = position.Y;
            Canvas.SetLeft( _EndEffector, position.X - 10 );
            Canvas.SetTop( _EndEffector, position.Y - 10 );
         }
      }

      private void ArmCanvas_OnMouseUp(object sender, MouseButtonEventArgs e)
      {
         if (e.LeftButton == MouseButtonState.Released)
         {
            Point position = e.GetPosition( ArmCanvas );
            Point relativePosition = new Point();
            relativePosition.X = 100 / ArmCanvas.ActualWidth * position.X - 50;
            relativePosition.Y = -50 / ArmCanvas.ActualHeight * position.Y + 50;
            CheckMoveArm( relativePosition, true );
         }
      }

      private void ArmCanvas_OnSizeChanged(object sender, SizeChangedEventArgs e)
      {
         Point bottomMiddle = new Point();
         bottomMiddle.X = ArmCanvas.ActualWidth / 2;
         bottomMiddle.Y = ArmCanvas.ActualHeight;
         Canvas.SetTop(_ArmCircle, bottomMiddle.Y - 35);
         Canvas.SetLeft(_ArmCircle, bottomMiddle.X - 35);
         _line.X1 = bottomMiddle.X;
         _line.Y1 = bottomMiddle.Y;
      }

      private void CheckMoveArm(Point relativePosition, bool ignoreCheck = false)
      {
         double x = 25.0 / 50.0 * relativePosition.X;
         double y = 25.0 / 50.0 * relativePosition.Y;
         if ( ignoreCheck || Point.Subtract( _lastSentPosition, new Point( x, y ) ).Length > 0.4 )
         {
            (DataContext as ArmControllerViewModel).MoveArmCommand.Execute( new Position(x, y, 0) );
            _lastSentPosition = new Point(x, y);
         }
      }
   }
}
