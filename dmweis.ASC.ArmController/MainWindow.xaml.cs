using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace dmweis.ASC.ArmController
{
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window
   {
      private MainViewModel m_MainViewModel;
      private string m_LastUsedPath;

      public MainWindow()
      {
         InitializeComponent();
         m_MainViewModel = new MainViewModel();
         DataContext = m_MainViewModel;
      }

      private void LoadScriptButton( object sender, RoutedEventArgs e )
      {
         OpenFileDialog openFIleDialog = new OpenFileDialog()
         {
            Filter = "XML (*.xml)|*.xml|All files (*.*)|*.*"
         };
         if( Directory.Exists( @"D:\Programming\C#\Visual_studio_projects_2017\dmweis.ASC\Scripts" ) )
         {
            openFIleDialog.InitialDirectory = @"D:\Programming\C#\Visual_studio_projects_2017\dmweis.ASC\Scripts";
         }
         if( openFIleDialog.ShowDialog() == true )
         {
            m_LastUsedPath = openFIleDialog.FileName;
            m_MainViewModel.LoadScript.Execute( openFIleDialog.FileName );
         }
      }

      private void ReloadScriptButton( object sender, RoutedEventArgs e )
      {
         if( !string.IsNullOrWhiteSpace( m_LastUsedPath ) )
         {
            m_MainViewModel.LoadScript.Execute( m_LastUsedPath );
         }
      }
   }
}
