using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using dmweis.ASC.ArmController;
using dmweis.ASC.Connector;
using dmweis.ASC.Connector.HardwareConnection;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace dmweis.ASC
{
   class MainWindowViewModel : ViewModelBase
   {

      private IArm m_Arm;

      public IArm Arm
      {
         get { return m_Arm; }
         private set { Set(() => Arm, ref m_Arm, value); }
      }


      public ICommand RefreshPortsCommand { get; }
      public RelayCommand ConnectCommand { get; }

      private ViewModelBase _currentViewModel;
      public ViewModelBase CurrentViewModel
      {
         get { return _currentViewModel; }
         set{Set(ref _currentViewModel, value );}
      }

      private SerialPortAddress[] m_AvailablePorts;
      public SerialPortAddress[] AvailablePorts
      {
         get { return m_AvailablePorts; }
         set { Set( ref m_AvailablePorts, value ); }
      }

      private SerialPortAddress m_SelectedPort;

      public SerialPortAddress SelectedPort
      {
         get { return m_SelectedPort; }
         set
         {
            Set(() => SelectedPort, ref m_SelectedPort, value);
            ConnectCommand.RaiseCanExecuteChanged();
         }
      }


      public MainWindowViewModel()
      {
         AvailablePorts = HardwareService.GetSerialPorts();
         RefreshPortsCommand = new RelayCommand( () =>
         {
            AvailablePorts = HardwareService.GetSerialPorts();
         } );
         ConnectCommand = new RelayCommand( Connect, () => Arm == null && SelectedPort != null );
         CurrentViewModel = new ArmControllerViewModel( this );
      }

      private void Connect()
      {
         Arm = new Arm( SelectedPort.Name, "direct.xml" );
         ConnectCommand.RaiseCanExecuteChanged();
      }

   }
}
