using System.IO.Ports;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using dmweis.ASC.Connector;
using dmweis.ASC.Connector.HardwareConnection;

namespace dmweis.ASC.ArmController
{
   class ArmControllerViewModel : ViewModelBase
   {
      private MainWindowViewModel m_MainViewModel;
      private bool m_SendingCommand;

      private bool m_MagnetOn;

      public bool MagnetOn
      {
         get { return m_MagnetOn; }
         set { Set( () => MagnetOn, ref m_MagnetOn, value); }
      }


      public RelayCommand<Position> MoveArmCommand { get; }
      public RelayCommand SwitchMagnetCommand { get; }

      public SerialPortAddress SelectedPort { get; set; }

      public ArmControllerViewModel( MainWindowViewModel mainWindowViewModel )
      {
         m_MainViewModel = mainWindowViewModel;
         MoveArmCommand = new RelayCommand<Position>( ArmCommand );
         SwitchMagnetCommand = new RelayCommand( SwitchMagnet );
      }

      private async void SwitchMagnet()
      {
         if( m_MainViewModel.Arm != null && !m_SendingCommand )
         {
            m_SendingCommand = true;
            MagnetOn = !MagnetOn;
            await m_MainViewModel.Arm.SetMagnet( MagnetOn );
            m_SendingCommand = false;
         }
      }

      private async void ArmCommand(Position position)
      {
         if ( m_MainViewModel.Arm != null && !m_SendingCommand)
         {
            m_SendingCommand = true;
            await m_MainViewModel.Arm.MoveToCartesianAsync( position.X, position.Y, position.Z );
            m_SendingCommand = false;
         }
      }
   }
}
