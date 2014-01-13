<%@ Application Language="C#" %>
<script RunAt="server">

  public static void RegisterRoutes( RouteCollection routes )
  {
    routes.IgnoreRoute( "{resource}.axd/{*pathInfo}" );


    MvcEnvironment.SimpleRouteTable
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
    MvcEnvironment.JumonyViewEngine.MasterLocationFormats = new string[] { "~/Views/{0}.html" };
    MvcEnvironment.JumonyViewEngine.PartialViewLocationFormats = new string[] { "~/Views/{0}.html" };

  }



  public class MyCachePolicyProvider : ICachePolicyProvider
  {

    public CachePolicy CreateCachePolicy( HttpContextBase context )
    {
      return new StandardCachePolicy( context, CacheToken.FromVirtualPath( context ), this, TimeSpan.FromMinutes( 0.5 ), true, "~/StaticCaches", true );
    }
  }

  

</script>
