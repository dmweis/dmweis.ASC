using dmweis.ASC.Connector;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dmweis.ASC.ArmController
{
   class MainViewModel : ViewModelBase
   {
      private Arm m_Arm;

      private bool m_PortSelectionOpen;
      public bool PortSelectionOpen
      {
         get
         {
            return m_PortSelectionOpen;
         }
         set
         {
            if( m_PortSelectionOpen == value )
            {
               return;
            }

            m_PortSelectionOpen = value;
            RaisePropertyChanged();
         }
      }

      private ObservableCollection<string> m_AvailablePorts;
      public ObservableCollection<string> AvailablePorts
      {
         get
         {
            return m_AvailablePorts ?? new ObservableCollection<string>();
         }
         set
         {
            if( m_AvailablePorts == value )
            {
               return;
            }

            m_AvailablePorts = value;
            RaisePropertyChanged();
         }
      }

      private string m_SelectedPort;
      public string SelectedPort
      {
         get
         {
            return m_SelectedPort;
         }
         set
         {
            if( m_SelectedPort == value )
            {
               return;
            }

            m_SelectedPort = value;
            RaisePropertyChanged();
            Connect.RaiseCanExecuteChanged();
         }
      }

      private ArmScript m_Script;
      public ArmScript Script
      {
         get
         {
            return m_Script ?? new ArmScript();
         }
         set
         {
            if( m_Script == value )
            {
               return;
            }

            m_Script = value;
            RaisePropertyChanged();
            StartScript.RaiseCanExecuteChanged();
         }
      }

      private bool m_RepeatScript;
      public bool RepeatScript
      {
         get
         {
            return m_RepeatScript;
         }
         set
         {
            if( m_RepeatScript == value )
            {
               return;
            }

            m_RepeatScript = value;
            RaisePropertyChanged();
         }
      }

      private bool m_ScriptRunning;
      public bool ScriptRunning
      {
         get
         {
            return m_ScriptRunning;
         }
         set
         {
            if( m_ScriptRunning == value )
            {
               return;
            }

            m_ScriptRunning = value;
            RaisePropertyChanged();
         }
      }

      public RelayCommand Connect { get; }
      public RelayCommand StartScript { get; }
      public RelayCommand StopScript { get; }
      public RelayCommand<string> LoadScript { get; }

      public MainViewModel()
      {
         AvailablePorts = new ObservableCollection<string>( SerialPort.GetPortNames() );
         Connect = new RelayCommand( EstablisheConnection, () => !string.IsNullOrWhiteSpace( SelectedPort ) );
         StartScript = new RelayCommand( StartScriptExecutionAsync, () => m_Script != null );
         StopScript = new RelayCommand( () => { }, () => false );
         LoadScript = new RelayCommand<string>( LoadScriptFromFile );
         PortSelectionOpen = true;
         ScriptRunning = false;
      }

      private void EstablisheConnection()
      {
         m_Arm = new Arm( m_SelectedPort, "" );
         PortSelectionOpen = false;
      }

      private async void StartScriptExecutionAsync()
      {
         ScriptRunning = true;
         do
         {
            await m_Arm.ExecuteScriptAsync( m_Script );
         } while( RepeatScript );
         ScriptRunning = false;
      }

      private void LoadScriptFromFile( string path )
      {
         Script = ArmScript.ReadArmScript( path );
      }
   }
}
