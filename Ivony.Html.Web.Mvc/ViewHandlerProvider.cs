using System;
using System.Collections.Generic;
using System.Globalization;
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



    internal static Exception VirtualPathFormatError( string paramName )
    {
      return new ArgumentException( string.Format( CultureInfo.InvariantCulture, "{0} 只能使用应用程序根相对路径，即以 \"~/\" 开头的路径，调用 VirtualPathUtility.ToAppRelative 方法或使用 HttpRequest.AppRelativeCurrentExecutionFilePath 属性获取", paramName ), paramName );
    }


    /// <summary>
    /// 在指定虚拟路径上溯搜索指定文件名的文件
    /// </summary>
    /// <param name="virtualPath">要搜索的虚拟路径</param>
    /// <param name="fileNames">要搜索的文件名列表</param>
    /// <returns>返回找到的文件路径，若无法找到匹配的文件，则返回null</returns>
    internal static string FallbackSearch( string virtualPath, params string[] fileNames )
    {
      return FallbackSearch( HostingEnvironment.VirtualPathProvider, virtualPath, fileNames );
    }


    /// <summary>
    /// 在指定虚拟路径上溯搜索指定文件名的文件
    /// </summary>
    /// <param name="provider">自定义的虚拟路径提供程序</param>
    /// <param name="virtualPath">要搜索的虚拟路径</param>
    /// <param name="fileNames">要搜索的文件名列表</param>
    /// <returns>返回找到的文件路径，若无法找到匹配的文件，则返回null</returns>
    internal static string FallbackSearch( VirtualPathProvider provider, string virtualPath, params string[] fileNames )
    {
      if ( !VirtualPathUtility.IsAppRelative( virtualPath ) )
        throw VirtualPathFormatError( "virtualPath" );

      var directory = VirtualPathUtility.GetDirectory( virtualPath );

      while ( true )
      {

        foreach ( var name in fileNames )
        {
          var filePath = VirtualPathUtility.Combine( directory, name );
          if ( provider.FileExists( filePath ) )
            return filePath;
        }

        if ( directory == "~/" )
          break;

        directory = VirtualPathUtility.Combine( directory, "../" );

      }

      return null;
    }





    /// <summary>
    /// 获取视图处理程序
    /// </summary>
    /// <param name="virtualPath">视图的虚拟路径</param>
    /// <param name="excludeDefaultHandler">是否要查找默认视图处理程序</param>
    /// <returns>该虚拟路径的视图处理程序</returns>
    public static IViewHandler GetViewHandler( string virtualPath, bool includeDefaultHandler )
    {
      var handler = GetHandlerInternal( virtualPath );

      if ( handler == null && !includeDefaultHandler )
        handler = GetHandlerInternal( VirtualPathUtility.Combine( VirtualPathUtility.GetDirectory( virtualPath ), "_handler.ashx" ) );

      return handler ?? new ViewHandler();
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
