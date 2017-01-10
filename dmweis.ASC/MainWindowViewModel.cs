using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dmweis.ASC.ArmController;
using GalaSoft.MvvmLight;

namespace dmweis.ASC
{
   class MainWindowViewModel : ViewModelBase
   {
      private ViewModelBase _currentViewModel;

      public ViewModelBase CurrentViewModel
      {
         get { return _currentViewModel; }
         set
         {
            Set(ref _currentViewModel, value );
         }
      }

      public MainWindowViewModel()
      {
            CurrentViewModel = new ArmControllerViewModel();
      }

   }
}
