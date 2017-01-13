using System;
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
      private bool m_MagnetOn;

      public ArmBase Arm => ArmService.Default.Arm;

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
         MoveArmCommand = new RelayCommand<Position>( ArmCommandAsync );
         SwitchMagnetCommand = new RelayCommand( SwitchMagnetAsync );
      }

      private async void SwitchMagnetAsync()
      {
         if( Arm != null )
         {
            MagnetOn = !MagnetOn;
            await Arm.SetMagnetAsync( MagnetOn );
         }
      }

      private async void ArmCommandAsync(Position position)
      {
         if ( Arm != null )
         {
            try
            {
               await Arm.MoveToCartesianAsync( position.X, position.Y, position.Z );
            }
            catch (InvalidOperationException )
            {
            }
         }
      }
   }
}
