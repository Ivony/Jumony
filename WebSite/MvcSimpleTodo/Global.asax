<%@ Application Language="C#" %>
<script RunAt="server">
  // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
  // 请访问 http://go.microsoft.com/?LinkId=9394801

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

    MvcEnvironment.Configuration.PartialRenderTimeout = TimeSpan.FromMilliseconds( 50 );

  }

</script>
