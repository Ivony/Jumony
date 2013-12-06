using Ivony.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Ivony.Fluent;

namespace Ivony.Html.Web
{
  internal class DefaultProviders
  {


    public DefaultProviders()
    {
      StaticFileContentProvider = new StaticFileContentProvider();
      WebFormPageContentProvider = new WebFormPageContentProvider();
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


    public IEnumerable<IHtmlContentProvider> GetContentServices( string virtualPath )
    {
      if ( VirtualPathUtility.GetExtension( virtualPath ).EqualsIgnoreCase( ".html" ) )
        return new IHtmlContentProvider[] { StaticFileContentProvider };

      if ( VirtualPathUtility.GetExtension( virtualPath ).EqualsIgnoreCase( ".html" ) )
        return new IHtmlContentProvider[] { WebFormPageContentProvider };

      return null;
    }

    public IHtmlParser GetParser( string virtualPath )
    {
      return new WebParser();
    }


    public IRequestMapper HtmlFileRequestMapper = new DefaultRequestMapper();

    public IEnumerable<IRequestMapper> RequestMappers
    {
      get { return new[] { HtmlFileRequestMapper }; }
    }

    public IHtmlHandlerProvider GetHtmlHandlerProvider()
    {
      return new DefaultHtmlHandlerProvider();
    }
  }
}
