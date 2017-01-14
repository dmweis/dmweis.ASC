using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
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

      public RelayCommand AddMagnetOnCommand { get; }
      public RelayCommand AddMagnetOffCommand { get; }
      public RelayCommand<int> AddDelayCommand { get; }
      public RelayCommand RunScriptCommand { get; }
      public RelayCommand ClearScriptCommand { get; }
      public RelayCommand SaveScriptCommand { get; }
      public RelayCommand LoadScriptCommand { get; }

      public ScriptPanelViewModel()
      {
         AddMagnetOnCommand = new RelayCommand( () => OnNewMagnetCommand( true ) );
         AddMagnetOffCommand = new RelayCommand( () => OnNewMagnetCommand( false ) );
         AddDelayCommand = new RelayCommand<int>( OnNewDelayCommand );
         RunScriptCommand = new RelayCommand( OnRunScriptCommandAsync );
         ClearScriptCommand = new RelayCommand( () => Commands.Clear() );
         SaveScriptCommand = new RelayCommand( OnSaveScriptCommand );
         LoadScriptCommand = new RelayCommand( OnLoadScriptCommand );
         Messenger.Default.Register<ArmPosition>( this, OnNewArmPosition );
      }

      private async void OnRunScriptCommandAsync()
      {
         if( Arm == null )
         {
            return;
         }
         List<ArmCommand> commands = new List<ArmCommand>( Commands );
         foreach( var command in commands )
         {
            await Arm.ExecuteCommandAsync( command );
         }
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

      private void OnNewMagnetCommand( bool turnOn ) => Commands.Add( new MagnetCommand( turnOn ) );

      private void OnNewArmPosition( ArmPosition position ) => Commands.Add( new MoveCommand( position ) );
   }
}
