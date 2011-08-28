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

      CssSelector.WarmUp();
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






    public static string GetCacheKey( ControllerContext context, ActionDescriptor actionDescriptor )
    {

      return HtmlProviders.GetCacheKey( context.HttpContext );

    }


    public static HtmlCachePolicy GetCachePolicy( ControllerContext context, ActionDescriptor actionDescriptor, ActionResult cacheItem )
    {

      return HtmlProviders.GetCachePolicy( context.HttpContext, null );

    }



  }
}
