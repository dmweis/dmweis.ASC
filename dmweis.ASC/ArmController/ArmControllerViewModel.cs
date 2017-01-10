using System.IO.Ports;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using dmweis.ASC.Connector;

namespace dmweis.ASC.ArmController
{
   class ArmControllerViewModel : ViewModelBase
   {
      private IArm m_Arm;

      public ICommand RefreshPortsCommand { get; }
      public RelayCommand ConnectCommand { get; }
      public RelayCommand<Position> MoveArmCommand { get; }

      private string[] m_AvailablePorts;
      public string[] AvailablePorts
      {
         get { return m_AvailablePorts; }
         set { Set(ref m_AvailablePorts, value); }
      }

      public string SelectedPort { get; set; }

      public ArmControllerViewModel()
      {
         AvailablePorts = SerialPort.GetPortNames();
         RefreshPortsCommand = new RelayCommand(() =>
           {
              AvailablePorts = SerialPort.GetPortNames();
           });
         ConnectCommand = new RelayCommand( Connect, () => m_Arm == null );
         MoveArmCommand = new RelayCommand<Position>( ArmCommand, ( pos ) => m_Arm != null );
      }

      private void Connect()
      {
         m_Arm = new Arm(SelectedPort, "direct.xml" );
         ConnectCommand.RaiseCanExecuteChanged();
         MoveArmCommand.RaiseCanExecuteChanged();
      }

      private async void ArmCommand(Position position)
      {
         await m_Arm.MoveToCartesianAsync( position.X, position.Y, position.Z );
      }
   }
}
