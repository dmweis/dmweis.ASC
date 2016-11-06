using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using dmweis.ASC.Connector;

namespace dmweis.ASC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ServoController m_ServoController;

        public MainWindow()
        {
            InitializeComponent();
            m_ServoController = new ServoController( "COM3" );
        }

        private void slider1_ValueChanged( object sender, RoutedPropertyChangedEventArgs<double> e )
        {
            m_ServoController?.SetServo( 0, (int) e.NewValue );
        }

        private void slider2_ValueChanged( object sender, RoutedPropertyChangedEventArgs<double> e )
        {
            m_ServoController?.SetServo( 1, (int) e.NewValue );
        }

        private void slider3_ValueChanged( object sender, RoutedPropertyChangedEventArgs<double> e )
        {
            m_ServoController?.SetServo( 2, (int)e.NewValue );
        }

        private void slider4_ValueChanged( object sender, RoutedPropertyChangedEventArgs<double> e )
        {
            m_ServoController?.SetServo( 3, (int)e.NewValue );
        }

        private void slider5_ValueChanged( object sender, RoutedPropertyChangedEventArgs<double> e )
        {
            m_ServoController?.SetServo( 8, (int)e.NewValue );
        }

        private void slider6_ValueChanged( object sender, RoutedPropertyChangedEventArgs<double> e )
        {
            m_ServoController?.SetServo( 9, (int)e.NewValue );
        }

        private void slider7_ValueChanged( object sender, RoutedPropertyChangedEventArgs<double> e )
        {
            m_ServoController?.SetServo( 10, (int)e.NewValue );
        }
    }
}
