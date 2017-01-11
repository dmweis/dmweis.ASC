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

      
      public RelayCommand<Position> MoveArmCommand { get; }

      public SerialPortAddress SelectedPort { get; set; }

      public ArmControllerViewModel( MainWindowViewModel mainWindowViewModel )
      {
         m_MainViewModel = mainWindowViewModel;
         MoveArmCommand = new RelayCommand<Position>( ArmCommand );
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
