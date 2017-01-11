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
      private IArm m_Arm;
      private bool m_SendingCommand;

      public ICommand RefreshPortsCommand { get; }
      public RelayCommand ConnectCommand { get; }
      public RelayCommand<Position> MoveArmCommand { get; }

      private SerialPortAddress[] m_AvailablePorts;
      public SerialPortAddress[] AvailablePorts
      {
         get { return m_AvailablePorts; }
         set { Set(ref m_AvailablePorts, value); }
      }

      public SerialPortAddress SelectedPort { get; set; }

      public ArmControllerViewModel()
      {
         AvailablePorts = HardwareService.GetSerialPorts();
         RefreshPortsCommand = new RelayCommand(() =>
           {
              AvailablePorts = HardwareService.GetSerialPorts();
           } );
         ConnectCommand = new RelayCommand( Connect, () => m_Arm == null );
         MoveArmCommand = new RelayCommand<Position>( ArmCommand, ( pos ) => m_Arm != null );
      }

      private void Connect()
      {
         m_Arm = new Arm(SelectedPort.Name, "direct.xml" );
         ConnectCommand.RaiseCanExecuteChanged();
         MoveArmCommand.RaiseCanExecuteChanged();
      }

      private async void ArmCommand(Position position)
      {
         if (!m_SendingCommand)
         {
            m_SendingCommand = true;
            await m_Arm.MoveToCartesianAsync( position.X, position.Y, position.Z );
            m_SendingCommand = false;
         }
      }
   }
}
