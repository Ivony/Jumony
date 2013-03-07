using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Ivony.Html;
using Ivony.Html.MSHTMLAdapter;
using System.Collections.ObjectModel;
using Ivony.Fluent;
using Microsoft.Win32;
using System.ComponentModel;
using Ivony.Html.Parser;

namespace HtmlTranslator
{
  /// <summary>
  /// MainWindow.xaml 的交互逻辑
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();

      DataView.ItemsSource = Data;
    }


    private readonly ObservableCollection<TranslationTerm> Data = new ObservableCollection<TranslationTerm>();


    private OpenFileDialog openFile = InitializeOpenFileDialog();

    private static OpenFileDialog InitializeOpenFileDialog()
    {
      var dialog = new OpenFileDialog();

      dialog.InitialDirectory = Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments );
      dialog.Filter = "HTML 文件|*.htm;*.html";
      dialog.Multiselect = false;

      return dialog;
    }

    private void OnChooseFile( object sender, RoutedEventArgs e )
    {

      if ( openFile.ShowDialog() ?? false )
      {

        BrowseFileButton.IsEnabled = false;


        Open( openFile.FileName );


        BrowseFileButton.IsEnabled = true;

      }

    }


    private TranslateTask Task
    {
      get;
      set;
    }

    private void Open( string filePath )
    {

      Task = TranslateTask.LoadTranslateTask( filePath );
      WebBrowser.Navigate( Task.Translate() );
      DataView.ItemsSource = Task.Terms;

    }

    private void OnSave( object sender, RoutedEventArgs e )
    {

      WebBrowser.Navigate( Task.Translate() );
    }
  }
}
