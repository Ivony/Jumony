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
  /// <summary>
  /// 提供视图处理程序
  /// </summary>
  public static class ViewHandlerProvider
  {
    /// <summary>
    /// 获取视图处理程序
    /// </summary>
    /// <param name="virtualPath">视图的虚拟路径</param>
    /// <returns>该虚拟路径的视图处理程序</returns>
    public static IViewHandler GetHandler( string virtualPath, bool isMasterView = false )
    {
      var handlerPath = virtualPath + ".ashx";

      var handler = GetHandlerInternal( handlerPath );
      if ( handler != null || isMasterView )
        return handler;

      return handler ?? GetHandlerInternal( VirtualPathUtility.Combine( VirtualPathUtility.GetDirectory( virtualPath ), "_handler.ashx" ) ) ?? new ViewHandler();
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
  }
}
