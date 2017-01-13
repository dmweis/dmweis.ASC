﻿using System.Windows.Input;
using dmweis.ASC.ArmController;
using dmweis.ASC.Connector;
using dmweis.ASC.Connector.HardwareConnection;
using dmweis.ASC.ScriptPanel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace dmweis.ASC
{
   class MainWindowViewModel : ViewModelBase
   {

      public ArmBase Arm => ArmService.Default.Arm;

      public ICommand RefreshPortsCommand { get; }
      public RelayCommand ConnectCommand { get; }

      private ViewModelBase _SideViewModel;
      public ViewModelBase SideViewModel
      {
         get { return _SideViewModel; }
         set { Set(() => SideViewModel, ref _SideViewModel, value); }
      }

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
         AvailablePorts = ArmService.GetSerialPorts();
         RefreshPortsCommand = new RelayCommand( () =>
         {
            AvailablePorts = ArmService.GetSerialPorts();
         } );
         ConnectCommand = new RelayCommand( Connect, () => Arm == null && SelectedPort != null );
         CurrentViewModel = new ArmControllerViewModel();
         SideViewModel = new ScriptPanelViewModel();
      }

      private void Connect()
      {
         ArmService.Default.Connect( SelectedPort, "direct.xml" );
         ConnectCommand.RaiseCanExecuteChanged();
      }

   }
}
