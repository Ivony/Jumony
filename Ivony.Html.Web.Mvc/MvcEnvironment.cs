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





    private static  MvcConfiguration _configuration = new MvcConfiguration();

    /// <summary>
    /// 获取 Jumony for MVC 配置对象，可以对 Jumony for MVC 行为进行调整。
    /// </summary>
    public static MvcConfiguration Configuration
    {
      get { return _configuration; }
    }


    /// <summary>
    /// 当任何一个 JumonyViewEngine 对象成功创建了视图时发生。
    /// </summary>
    public static event EventHandler<JumonyViewEventArgs> ViewCreated;

    internal static void OnViewCreated( object sender, JumonyViewEventArgs e )
    {
      if ( ViewCreated != null )
        ViewCreated( sender, e );
    }


    /// <summary>
    /// 获取当前请求的缓存策略
    /// </summary>
    /// <param name="context"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static CachePolicy GetCachePolicy( ControllerContext context, ActionDescriptor action )
    {

      return HtmlProviders.GetCachePolicy( context.HttpContext );
    }


    private static readonly IMvcCachePolicyProvider _defaultCachePolicyProvider = new MvcCachePolicyProvider( HtmlProviders.DefaultCachePolicyProvider );

    public static IMvcCachePolicyProvider DefaultCachePolicyProvider
    {
      get { return _defaultCachePolicyProvider; }
    }


  }

}
