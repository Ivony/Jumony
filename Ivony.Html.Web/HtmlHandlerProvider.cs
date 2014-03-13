using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Html;
using Ivony.Html.ExpandedAPI;
using System.Web.Hosting;
using System.Web.Compilation;
using System.Web;
using System.Web.Caching;
using Ivony.Web;


namespace Ivony.Html.Web
{

  /// <summary>
  /// 提供一组方法，用于查找对文档进行处理的 IHtmlHandler 实例。
  /// </summary>
  public static class HtmlHandlerProvider
  {


    /// <summary>
    /// 获取指定虚拟路径的 HTML 处理程序
    /// </summary>
    /// <param name="virtualPath">要获取 HTML 处理程序的虚拟路径</param>
    /// <returns>HTML 处理程序</returns>
    public static IHtmlHandler GetHandler( string virtualPath )
    {

      var services = WebServiceLocator.GetServices<IHtmlHandlerProvider>( virtualPath );
      foreach ( var provider in services )
      {
        var handler = provider.GetHandler( virtualPath );

        if ( handler != null )
          return handler;

      }


      if ( !HostingEnvironment.VirtualPathProvider.FileExists( virtualPath ) )//如果文件不存在，则直接返回 null　。
        return null;


      return
        GetHandlerInternal<IHtmlHandler>( virtualPath + ".ashx" ) ??
        GetHandlerInternal<IHtmlHandler>( GetHandlerPath( virtualPath ) ) ??
        GetHandlerInternal<IHtmlHandler>( VirtualPathHelper.FallbackSearch( virtualPath, "_handler.ashx" ) );
    }


    private static T GetHandlerInternal<T>( string handlerPath ) where T : class
    {

      if ( handlerPath == null )
        return null;

      if ( HostingEnvironment.VirtualPathProvider.FileExists( handlerPath ) )
      {
        try
        {

          var type = typeof( T );

          if ( type.IsAssignableFrom( BuildManager.GetCompiledType( handlerPath ) ) )
            return BuildManager.CreateInstanceFromVirtualPath( handlerPath, type ) as T;

        }
        catch
        {

        }
      }

      return null;
    }



    private static readonly string handlerPathCachePrefix = "JumonyHandlerCache_";

    private class HandlerPathCacheItem
    {
      public string HandlerPath { get; set; }
    }




    /// <summary>
    /// 在 HTML 文档中查找 ViewHandler 路径设置。
    /// </summary>
    /// <param name="virtualPath">要处理的 HTML 文档的虚拟路径</param>
    /// <returns>用于处理 HTML 的视图处理程序路径</returns>
    public static string GetHandlerPath( string virtualPath )
    {

      var cacheKey = handlerPathCachePrefix + virtualPath;

      var cacheItem = HttpRuntime.Cache.Get( cacheKey ) as HandlerPathCacheItem;
      if ( cacheItem == null )
      {
        string cacheDependencyKey;
        var document = HtmlServices.LoadDocument( virtualPath, out cacheDependencyKey );

        if ( document == null )
          return null;



        var head = document.FindFirstOrDefault( "head" );
        if ( head == null )
          return null;

        var handlerMeta = head.FindFirstOrDefault( "meta[name=handler]" );
        if ( handlerMeta == null )
          return null;

        var handlerPath = handlerMeta.Attribute( "value" ).Value();


        if ( cacheDependencyKey != null )
        {
          cacheItem = new HandlerPathCacheItem() { HandlerPath = handlerPath };
          HttpRuntime.Cache.Insert( cacheKey, cacheItem, new CacheDependency( new string[0], new[] { cacheDependencyKey } ) );
        }
      }


      return cacheItem.HandlerPath;

    }


  }
}
