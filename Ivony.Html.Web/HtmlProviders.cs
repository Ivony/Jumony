using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web
{
  public static class HtmlProviders
  {

    static HtmlProviders()
    {
      ParserProviders = new SynchronizedCollection<IHtmlParserProvider>();
    }

    
    public static ICollection<IHtmlParserProvider> ParserProviders
    {
      get;
      private set;
    }

  }

  public interface IHtmlContentProvider
  {

    string LoadContent( string virtualPath );

  }


  public interface IHtmlParserProvider
  {

    IHtmlDocument GetParser( string virtualPath, string htmlContent );

  }


}
