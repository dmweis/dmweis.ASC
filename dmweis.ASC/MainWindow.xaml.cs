using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using dmweis.ASC.Connector;

namespace dmweis.ASC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ServoController m_ServoController;
        private ObservableCollection<ServoControllerViewModel> m_ServoControllerViewModels;

        public MainWindow()
        {
            InitializeComponent();
            m_ServoController = new ServoController( "COM11" );
            m_ServoControllerViewModels = new ObservableCollection<ServoControllerViewModel>();
            m_ServoControllerViewModels.Add( new ServoControllerViewModel( m_ServoController, 0 ) );
            m_ServoControllerViewModels.Add( new ServoControllerViewModel( m_ServoController, 1 ) );
            m_ServoControllerViewModels.Add( new ServoControllerViewModel( m_ServoController, 2 ) );
            ServoCollectionView.ItemsSource = m_ServoControllerViewModels;
        }

        private void AddServoButton( object sender, RoutedEventArgs e )
        {
         if( !int.TryParse( ( sender as Button )?.Tag as string, out int index ) )
            {
                return;
            }
            m_ServoControllerViewModels.Add( new ServoControllerViewModel( m_ServoController, index ) );

        }

        private void RemoveServoButton( object sender, RoutedEventArgs e )
        {
            Button senderButton = sender as Button;
            string servoName = senderButton.Tag as string;
            ServoControllerViewModel servoToRemove = null;
            foreach( var servoViewModel in m_ServoControllerViewModels )
            {
                if( servoViewModel.Name == servoName )
                {
                    servoToRemove = servoViewModel;
                    break;
                }
            }
            m_ServoControllerViewModels.Remove( servoToRemove );
        }

        private void Button_Click( object sender, RoutedEventArgs e )
        {
            m_ServoController?.SetServo( 20, 300 );
        }

        private void Button_Click_1( object sender, RoutedEventArgs e )
        {
            m_ServoController?.SetServo( 21, 300 );
        }
    }

    class ServoControllerViewModel : INotifyPropertyChanged
    {
        private ServoController m_ServoController;

        public event PropertyChangedEventHandler PropertyChanged;

        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                NotifyPropertyChanged();
            }
        }

        private int value;
        public int Value
        {
            get { return value; }
            set
            {
                if( value >= Minimum && value <= Maximum )
                {
                    this.value = value;
                    NotifyPropertyChanged();
                    UpdateServpValue( value );
                }
            }
        }

        private int servoIndex;
        public int ServoIndex
        {
            get { return servoIndex; }
            set
            {
                servoIndex = value;
                NotifyPropertyChanged();
            }
        }

        private int minimum;
        public int Minimum
        {
            get { return minimum; }
            set
            {
                minimum = value;
                NotifyPropertyChanged();
            }
        }

        private int maximum;
        public int Maximum
        {
            get { return maximum; }
            set
            {
                maximum = value;
                NotifyPropertyChanged();
            }
        }

        public ServoControllerViewModel( ServoController servoController, int servoIndex , string servoName = null, int value = 300, int minimum = 100, int maximum = 600 )
        {
            ServoIndex = servoIndex;
            Name = servoName ?? $"Servo {servoIndex}";
            Minimum = minimum;
            Maximum = maximum;
            Value = value;
            m_ServoController = servoController;
        }

        private void UpdateServpValue( int value )
        {
            m_ServoController?.SetServo( ServoIndex, value );
        }

        private void NotifyPropertyChanged( [CallerMemberName] string propertyName = "" )
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
        }

        private void Button_Click( object sender, RoutedEventArgs e )
        {
            m_ServoController?.SetServo( 20, 300 );
        }

        private void Button_Click_1( object sender, RoutedEventArgs e )
        {
            m_ServoController?.SetServo( 21, 300 );
        }
    }
}
