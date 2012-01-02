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
using System.Windows.Shapes;
using Ivony.Html;
using Ivony.Html.MSHTMLAdapter;
using System.Collections.ObjectModel;
using Ivony.Fluent;

namespace WebBrowser
{
  /// <summary>
  /// MainWindow.xaml 的交互逻辑
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();


      DataView.ItemsSource = new ObservableCollection<Term>();
    }

    private void Go( object sender, RoutedEventArgs e )
    {
      var url = new Uri( Address.Text );
      WebBrowser.Navigate( url );
    }

    private void OnLoadCompleted( object sender, NavigationEventArgs e )
    {
      var document = Ivony.Html.MSHTMLAdapter.ConvertExtensions.AsDocument( WebBrowser.Document );

      var textNodes = document.DescendantNodes().OfType<IHtmlTextNode>().Where( t => !t.IsWhiteSpace() && t.InnerText() != null );

      var data = DataView.ItemsSource.CastTo<ObservableCollection<Term>>();

      textNodes.ForAll( text => data.Add( new Term() { SourceTerm = text.HtmlText, TranslatedTerm = text.HtmlText } ) );


    }

  }
}
