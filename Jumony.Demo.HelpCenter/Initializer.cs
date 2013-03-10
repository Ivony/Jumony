using Jumony.Demo.HelpCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using Ivony.Web;
using Ivony.Html.Web;

[assembly: PreApplicationStartMethod( typeof( Initializer ), "Initialize" )]
namespace Jumony.Demo.HelpCenter
{
  public static class Initializer
  {

    public static void Initialize()
    {

      RouteTable.Routes.SimpleRouteTable()
        .MapRoute( "~/help", new { controller = "Help", action = "Entry" } )
        .MapRoute( "~/help/{action}", new { controller = "Help" } );
    }

  }
}
