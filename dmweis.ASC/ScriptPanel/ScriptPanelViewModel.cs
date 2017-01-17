using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using dmweis.ASC.Connector;
using dmweis.ASC.Connector.Scriping;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;

namespace dmweis.ASC.ScriptPanel
{
   class ScriptPanelViewModel : ViewModelBase
   {
      private ArmBase Arm => ArmService.Default.Arm;

      public ObservableCollection<ArmCommand> Commands { get; }
         = new ObservableCollection<ArmCommand>();

      private bool m_RepeatScript;
      public bool RepeatScript
      {
         get { return m_RepeatScript; }
         set { Set( () => RepeatScript, ref m_RepeatScript, value); }
      }

      public RelayCommand AddMagnetOnCommand { get; }
      public RelayCommand AddMagnetOffCommand { get; }
      public RelayCommand<int> AddDelayCommand { get; }
      public RelayCommand RunScriptCommand { get; }
      public RelayCommand ClearScriptCommand { get; }
      public RelayCommand SaveScriptCommand { get; }
      public RelayCommand LoadScriptCommand { get; }
      public RelayCommand<ArmCommand> DeleteCommand { get; }

      private bool m_ScriptRunning;

      public ScriptPanelViewModel()
      {
         AddMagnetOnCommand = new RelayCommand( () => OnNewMagnetCommand( true ) );
         AddMagnetOffCommand = new RelayCommand( () => OnNewMagnetCommand( false ) );
         AddDelayCommand = new RelayCommand<int>( OnNewDelayCommand );
         RunScriptCommand = new RelayCommand( OnRunScriptCommandAsync );
         ClearScriptCommand = new RelayCommand( OnClearScript );
         SaveScriptCommand = new RelayCommand( OnSaveScriptCommand );
         LoadScriptCommand = new RelayCommand( OnLoadScriptCommand );
         DeleteCommand = new RelayCommand<ArmCommand>(OnDeleteCommand);
         Messenger.Default.Register<ArmPosition>( this, OnNewArmPosition );
      }

      private async void OnRunScriptCommandAsync()
      {
         if( Arm == null )
         {
            return;
         }
         if (m_ScriptRunning)
         {
            m_ScriptRunning = false;
            return;
         }
         m_ScriptRunning = true;
         List<ArmCommand> commands = new List<ArmCommand>( Commands );
         do
         {
            foreach( var command in commands )
            {
               if (!m_ScriptRunning)
               {
                  return;
               }
               await Arm.ExecuteCommandAsync( command );
            }
         } while (RepeatScript);
         m_ScriptRunning = false;
      }

      private void OnNewDelayCommand( int seconds )
      {
         if( Commands.Count > 0 && Commands[ Commands.Count - 1 ] is DelayCommand )
         {
            ((DelayCommand) Commands[ Commands.Count - 1 ]).DelayTime += TimeSpan.FromMilliseconds( seconds );
         }
         else
         {
            Commands.Add( new DelayCommand( TimeSpan.FromMilliseconds( seconds ) ) );
         }
      }

      private void OnLoadScriptCommand()
      {
         if (Commands.Count > 0)
         {
            MessageBoxResult messageBoxResult = MessageBox.Show(
            "You will lose current script.\nAre you sure you want to continue?",
            "Arm controller",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning );
            if( messageBoxResult != MessageBoxResult.Yes )
            {
               return;
            }
         }
         OpenFileDialog dialogue = new OpenFileDialog();
         dialogue.DefaultExt = ".xml";
         dialogue.Filter = "XML Files (*.xml)|*.xml";
         bool? result = dialogue.ShowDialog();
         if( result == true )
         {
            ArmScript script = ArmScript.ReadArmScript( dialogue.FileName );
            Commands.Clear();
            script.Movements.ForEach( ( command ) => Commands.Add( command ) );
         }
      }

      private void OnSaveScriptCommand()
      {
         SaveFileDialog dialogue = new SaveFileDialog();
         dialogue.Filter = "XML Files (*.xml)|*.xml";
         dialogue.Title = "save script";
         bool? result = dialogue.ShowDialog();
         if( result == true )
         {
            ArmScript.SaveArmScript( dialogue.FileName, new ArmScript( Commands ) );
         }
      }

      private void OnClearScript()
      {
         MessageBoxResult result = MessageBox.Show(
            "Are you sure you want to clear the script?",
            "Arm controller",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);
         if (result == MessageBoxResult.Yes)
         {
            Commands.Clear();
         }
      }

      private void OnNewMagnetCommand( bool turnOn ) => Commands.Add( new MagnetCommand( turnOn ) );

      private void OnNewArmPosition( ArmPosition position ) => Commands.Add( new MoveCommand( position ) );

      private void OnDeleteCommand(ArmCommand command) => Commands.Remove(command);
   }
}
