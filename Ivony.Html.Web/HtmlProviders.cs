using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using Ivony.Html.Parser;
using System.Collections.ObjectModel;

namespace Ivony.Html.Web
{
  public static class HtmlProviders
  {

    private static SynchronizedCollection<IHtmlProvider> _providers = new SynchronizedCollection<IHtmlProvider>();


    static HtmlProviders()
    {
      _providers.Add( new RewriteToAshxProvider() );
    }


    public static RequestData GetRequestData( HttpRequest request )
    {
      foreach ( var provider in _providers )
      {
        var result = provider.GetRequestData( request );
        if ( result != null )
          return result;
      }

      return null;
    }





  }
}
