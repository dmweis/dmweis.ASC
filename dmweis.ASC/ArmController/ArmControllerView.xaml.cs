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
using dmweis.ASC.Converters;

namespace dmweis.ASC.ArmController
{
   class BoolToColorConverter : BoolToValueConverter<Brush> { }
   /// <summary>
   /// Interaction logic for ArmControllerView.xaml
   /// </summary>
   public partial class ArmControllerView : UserControl
   {

      private readonly Line _line;
      private readonly Ellipse[] _ArmCircles;
      private readonly Ellipse _EndEffector;
      private readonly TextBlock _CoordinatesLabel;
      private Point _lastSentPosition;
      private double _lastSentZ;

      public ArmControllerView()
      {
         InitializeComponent();
         _line = new Line()
         {
            Stroke = Brushes.Gray,
            StrokeThickness = 2
         };
         _ArmCircles = new Ellipse[6];
         for (int i = 0; i < _ArmCircles.Length; i++)
         {
            _ArmCircles[i] = new Ellipse
            {
               Stroke = Brushes.Red,
               StrokeThickness = 1,
               Width = 120 + i * 100,
               Height = 120 + i * 100
            };
         }
         _EndEffector = new Ellipse
         {
            Stroke = Brushes.DarkBlue,
            StrokeThickness = 1,
            Width = 20,
            Height = 20
         };
         _CoordinatesLabel = new TextBlock()
         {
            Visibility = Visibility.Collapsed
         };
         ArmCanvas.Children.Add(_line);
         foreach (var circle in _ArmCircles)
         {
            ArmCanvas.Children.Add( circle );
         }
         ArmCanvas.Children.Add( _EndEffector );
         ArmCanvas.Children.Add(_CoordinatesLabel);
         _lastSentPosition = new Point(0, 0);
      }

      private void ArmCanvas_OnMouseDown(object sender, MouseButtonEventArgs e)
      {
         if (e.LeftButton == MouseButtonState.Pressed)
         {
            if (_CoordinatesLabel.Visibility == Visibility.Collapsed)
            {
               _CoordinatesLabel.Visibility = Visibility.Visible;
            }
            Point position = e.GetPosition( ArmCanvas );
            Point relativePosition = new Point();
            relativePosition.X = 100 / ArmCanvas.ActualWidth * position.X - 50;
            relativePosition.Y = -50 / ArmCanvas.ActualHeight * position.Y + 50;
            CheckMoveArm(relativePosition, true);
            _line.X2 = position.X;
            _line.Y2 = position.Y;
            Canvas.SetLeft(_EndEffector, position.X - 10);
            Canvas.SetTop(_EndEffector, position.Y - 10);
            Canvas.SetLeft( _CoordinatesLabel, position.X + 15 );
            Canvas.SetTop( _CoordinatesLabel, position.Y - 30 );
         }
         if (e.RightButton == MouseButtonState.Pressed)
         {
            (DataContext as ArmControllerViewModel)?.SwitchMagnetCommand.Execute( null );
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
            Canvas.SetLeft( _CoordinatesLabel, position.X + 15 );
            Canvas.SetTop( _CoordinatesLabel, position.Y - 30 );
         }
      }

      private void ArmCanvas_OnMouseUp(object sender, MouseButtonEventArgs e)
      {
         if (e.LeftButton == MouseButtonState.Released && e.ChangedButton == MouseButton.Left)
         {
            Point position = e.GetPosition( ArmCanvas );
            Point relativePosition = new Point();
            relativePosition.X = 100 / ArmCanvas.ActualWidth * position.X - 50;
            relativePosition.Y = -50 / ArmCanvas.ActualHeight * position.Y + 50;
            Canvas.SetLeft( _CoordinatesLabel, position.X + 15 );
            Canvas.SetTop( _CoordinatesLabel, position.Y - 30 );
            CheckMoveArm( relativePosition, true );
         }
      }

      private void ArmCanvas_OnSizeChanged(object sender, SizeChangedEventArgs e)
      {
         Point bottomMiddle = new Point();
         bottomMiddle.X = ArmCanvas.ActualWidth / 2;
         bottomMiddle.Y = ArmCanvas.ActualHeight;
         foreach (var circle in _ArmCircles)
         {
            Canvas.SetTop(circle, bottomMiddle.Y - circle.ActualHeight/2);
            Canvas.SetLeft(circle, bottomMiddle.X - circle.ActualWidth/2);
         }
         _line.X1 = bottomMiddle.X;
         _line.Y1 = bottomMiddle.Y;
      }

      private void CheckMoveArm(Point relativePosition, bool ignoreCheck = false)
      {
         double x = 35.0 / 50.0 * relativePosition.X;
         double y = 35.0 / 50.0 * relativePosition.Y;
         _CoordinatesLabel.Text = $"X: {x:F} \nY: {y:F}";
         if( ignoreCheck || Point.Subtract( _lastSentPosition, new Point( x, y ) ).Length > 0.4 )
         {
            (DataContext as ArmControllerViewModel)?.MoveArmCommand.Execute( new Position( x, y, _lastSentZ ) );
            _lastSentPosition = new Point(x, y);
         }
      }

      private void RangeBase_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
      {
         double newValueAdjusted = e.NewValue / 5.0;
         if (Math.Abs( _lastSentZ - newValueAdjusted) > 0.4)
         {
            Position newPosition = new Position(_lastSentPosition.X, _lastSentPosition.Y, newValueAdjusted );
            ( DataContext as ArmControllerViewModel)?.MoveArmCommand.Execute( newPosition );
            _lastSentZ = newValueAdjusted;
            ZValueTextBox.Text = $"Z: {newValueAdjusted:F}";
         }

      }

      private void ArmCanvas_OnMouseWheel(object sender, MouseWheelEventArgs e)
      {
         ArmSlider.Value += e.Delta / 240.0;
      }

      private void AddPositionCommandButton(object sender, RoutedEventArgs e)
      {
         (DataContext as ArmControllerViewModel)?.AddPositionCommand.Execute( new Position(_lastSentPosition.X, _lastSentPosition.Y, _lastSentZ));
      }
   }
}
