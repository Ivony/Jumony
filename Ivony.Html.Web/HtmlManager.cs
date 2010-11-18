using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Html.Parser;
using System.Web;
using System.Web.Hosting;
using System.IO;
using System.Web.Caching;

namespace Ivony.Html.Web
{
  public class HtmlManager
  {

    protected virtual VirtualPathProvider VirtualPathProvider
    {
      get { return HostingEnvironment.VirtualPathProvider; }
    }

    protected virtual Cache Cache
    {
      get { return HostingEnvironment.Cache; }
    }



    public virtual IHtmlDocument GetDocument( string virtualPath )
    {

      string key = CreateCacheKey( virtualPath );

      var document = Cache.Get( key ) as IHtmlDocument;

      if ( document == null )
      {

        string htmlContent = LoadFile( virtualPath );
        document = ParseDocument( virtualPath, htmlContent );

      }

      return document;

    }

    private string CreateCacheKey( string virtualPath )
    {
      return "Jumony_HtmlManager_DocumentCache_" + virtualPath;
    }

    protected virtual string LoadFile( string virtualPath )
    {
      var file = VirtualPathProvider.GetFile( virtualPath );

      using ( var stream = file.Open() )
      {
        return new StreamReader( stream ).ReadToEnd();
      }
    }

    protected virtual IHtmlDocument ParseDocument( string virtualPath, string htmlContent )
    {
      var parser = new JumonyParser();

      return parser.Parse( htmlContent );
    }


  }
}
