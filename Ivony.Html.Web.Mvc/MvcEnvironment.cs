using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;



[assembly: PreApplicationStartMethod( typeof( Ivony.Html.Web.Mvc.MvcEnvironment ), "Initialize" )]

namespace Ivony.Html.Web.Mvc
{

  public static class MvcEnvironment
  {
    /// <summary>
    /// 此方法由 ASP.NET 4 系统调用，不应从用户代码中直接调用。
    /// </summary>
    public static void Initialize()
    {
      ViewEngines.Engines.Add( _viewEngine );
      RouteTable.Routes.Add( _simpleRoutingTable );
    }



    private static bool _isWarmedUp = false;

    /// <summary>
    /// 调用此方法通知预热 Jumony for MVC 运行环境。
    /// </summary>
    public static void WarmUp()
    {
      if ( !_isWarmedUp )
      {
        CssSelector.WarmUp();
        _isWarmedUp = true;
      }
    }




    private static JumonyViewEngine _viewEngine = new JumonyViewEngine();

    /// <summary>
    /// 获取 Jumony 视图引擎的默认实例
    /// </summary>
    public static JumonyViewEngine JumonyViewEngine
    {
      get { return _viewEngine; }
    }


    private static SimpleRoutingTable _simpleRoutingTable = new SimpleRoutingTable( new MvcRouteHandler(), true );

    /// <summary>
    /// 获取简单路由表的默认实例
    /// </summary>
    public static SimpleRoutingTable SimpleRoutingTable
    {
      get { return _simpleRoutingTable; }
    }





    /// <summary>
    /// 获取缓存键
    /// </summary>
    /// <param name="context">控制器上下文</param>
    /// <param name="actionDescriptor">调用的 Action</param>
    /// <returns>需要使用的缓存键</returns>
    public static string GetCacheKey( ControllerContext context, ActionDescriptor actionDescriptor )
    {

      return HtmlProviders.GetCacheKey( context.HttpContext );

    }



    /// <summary>
    /// 获取缓存策略
    /// </summary>
    /// <param name="context">控制器上下文</param>
    /// <param name="actionDescriptor">调用的 Action</param>
    /// <param name="cacheItem">需要被缓存的结果</param>
    /// <returns>缓存策略</returns>
    public static HtmlCachePolicy GetCachePolicy( ControllerContext context, string cacheKey, ICachedResponse cacheItem )
    {

      return HtmlProviders.GetCachePolicy( context.HttpContext, null );

    }


    private static  MvcConfiguration _configuration = new MvcConfiguration();

    public static MvcConfiguration Configuration
    {
      get { return _configuration; }
    }


  }

}
