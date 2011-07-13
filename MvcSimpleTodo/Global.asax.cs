using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Ivony.Html.Web.Mvc;


namespace MvcSimpleTodo
{
  // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
  // 请访问 http://go.microsoft.com/?LinkId=9394801

  public class MvcApplication : System.Web.HttpApplication
  {
    public static void RegisterRoutes( RouteCollection routes )
    {
      routes.IgnoreRoute( "{resource}.axd/{*pathInfo}" );


      MvcEnvironment.SimpleRoutingTable
        .MapAction( "~/", "Todo", "Index" )
        .MapAction( "~/add", "Todo", "Add" )
        .MapAction( "~/modify/{taskId}", "Todo", "Modify" )
        .MapAction( "~/complete/{taskId}", "Todo", "Complete" )
        .MapAction( "~/revert/{taskId}", "Todo", "Revert" )
        .MapAction( "~/remove/{taskId}", "Todo", "Remove" );

    }

    protected void Application_Start()
    {
      AreaRegistration.RegisterAllAreas();

      RegisterRoutes( RouteTable.Routes );


      MvcEnvironment.JumonyViewEngine.ViewLocationFormats = new string[] { "~/Views/{0}.html" };

    }
  }
}