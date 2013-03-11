using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Compilation;
using System.Web.Hosting;

namespace Ivony.Html.Web.Mvc
{
  internal static class HtmlViewHandlerProvider
  {

    public IHtmlViewHandler GetHandler( string virtualPath )
    {
      var handlerPath = virtualPath + ".ashx";

      if ( HostingEnvironment.VirtualPathProvider.FileExists( handlerPath ) )
      {
        try
        {
          var handler = BuildManager.GetCompiledType( handlerPath ) as IHtmlViewHandler;
          if ( handler != null )
            return handler;
        }
        catch
        {

        }
      }

      return null;
    }

  }
}
