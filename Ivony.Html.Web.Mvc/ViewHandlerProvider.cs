using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Compilation;
using System.Web.Hosting;
using System.Web.Mvc;

namespace Ivony.Html.Web
{
  internal static class ViewHandlerProvider
  {

    public static IViewHandler GetHandler( string virtualPath )
    {
      var handlerPath = virtualPath + ".ashx";

      return GetHandlerInternal( handlerPath ) ?? GetHandlerInternal( VirtualPathUtility.Combine( VirtualPathUtility.GetDirectory( virtualPath ), "_handler.ashx" ) );
    }

    private static IViewHandler GetHandlerInternal( string handlerPath )
    {
      if ( HostingEnvironment.VirtualPathProvider.FileExists( handlerPath ) )
      {
        try
        {
          var instance = BuildManager.CreateInstanceFromVirtualPath( handlerPath, typeof( object ) );

          var handler = instance as IViewHandler;
          if ( handler != null )
            return handler;

          var htmlHandler = instance as IHtmlHandler;
          if ( htmlHandler != null )
            return new HtmlViewHandlerWrapper( htmlHandler );
        }
        catch
        {

        }
      }

      return null;
    }


    private class HtmlViewHandlerWrapper : IViewHandler
    {
      private IHtmlHandler _handler;

      public HtmlViewHandlerWrapper( IHtmlHandler handler )
      {
        _handler = handler;
      }


      public void ProcessScope( ViewContext context, IHtmlContainer scope, string virtualPath )
      {
        _handler.ProcessDocument( context.HttpContext, scope.Document );
      }

      public ViewDataDictionary ViewData
      {
        get;
        set;
      }

      public void Dispose()
      {
        _handler.Dispose();
      }
    }


  }
}
