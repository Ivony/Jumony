using Ivony.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Ivony.Fluent;

namespace Ivony.Html.Web
{
  internal class DefaultProviders : IVirtualPathBasedProvider
  {


    public DefaultProviders()
    {
      StaticFileContentProvider = new StaticFileContentProvider();
      WebFormPageContentProvider = new WebFormPageContentProvider();
      JumonyParserProvider = new JumonyParserProvider();
    }

    public object GetService( string virtualPath, Type serviceType )
    {

      if ( serviceType == typeof( IHtmlContentProvider ) )
      {
        if ( VirtualPathUtility.GetExtension( virtualPath ).EqualsIgnoreCase( ".html" ) )
          return StaticFileContentProvider;

        if ( VirtualPathUtility.GetExtension( virtualPath ).EqualsIgnoreCase( ".html" ) )
          return WebFormPageContentProvider;

        return null;
      }

      else if ( serviceType == typeof( IHtmlParserProvider ) )
        return JumonyParserProvider;

      else
        return null;

    }


    public StaticFileContentProvider StaticFileContentProvider
    {
      get;
      private set;
    }


    public WebFormPageContentProvider WebFormPageContentProvider
    {
      get;
      private set;
    }


    public JumonyParserProvider JumonyParserProvider
    {
      get;
      private set;
    }


  }
}
