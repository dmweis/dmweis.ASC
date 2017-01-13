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
         if( m_MainViewModel.Arm != null )
         {
            MagnetOn = !MagnetOn;
            await m_MainViewModel.Arm.SetMagnetAsync( MagnetOn );
         }
      }

      private async void ArmCommandAsync(Position position)
      {
         if ( m_MainViewModel.Arm != null )
         {
            await m_MainViewModel.Arm.MoveToCartesianAsync( position.X, position.Y, position.Z );
         }
      }
   }
}
