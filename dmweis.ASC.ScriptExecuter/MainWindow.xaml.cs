using System;
using System.Collections.Generic;
using System.IO;
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
using dmweis.ASC.Connector;

namespace dmweis.ASC.ScriptExecuter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private ServoController m_ServoController;
        private bool m_Running;

        public MainWindow()
        {
            InitializeComponent();
            m_ServoController = new ServoController( "COM11" );
        }

        private async void ParseAndExecAsync()
        {
            m_Running = true;
            string[] commands = File.ReadAllLines( "script.txt" );
            for( int i = 0 ; i < commands.Length ; i++ )
            {
                string[] elements = commands[ i ].Split( null );
                int delay = int.Parse( elements[ 0 ] );
                int index = int.Parse( elements[ 1 ] );
                int value = int.Parse( elements[ 2 ] );
                await ExecuteCommandAsync( delay, index, value );
            }
            m_Running = false;
        }

        private async Task ExecuteCommandAsync( int delay, int index, int value )
        {
            await Task.Delay( delay );
            m_ServoController.SetServo( index, value );
        }

        private void Button_Click( object sender, RoutedEventArgs e )
        {
            if( !m_Running )
            {
                Task.Factory.StartNew( ParseAndExecAsync );
                System.Media.SystemSounds.Beep.Play();
            }
        }
    }
}
