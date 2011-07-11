using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;

namespace Ivony.Html.Web.Mvc
{
  public static class RoutingExtensions
  {

    public static SimpleRoutingTable MapAction( this SimpleRoutingTable routingTable, string urlPattern, string controller, string action )
    {
      return MapAction( routingTable, string.Format( "{0}/{1}", controller, action ), urlPattern, controller, action );
    }


    public static SimpleRoutingTable MapAction( this SimpleRoutingTable routingTable, string name, string urlPattern, string controller, string action )
    {
      return MapAction( routingTable, name, urlPattern, controller, action, new string[0] );
    }


    public static SimpleRoutingTable MapAction( this SimpleRoutingTable routingTable, string name, string urlPattern, string controller, string action, string[] queryKeys )
    {
      routingTable.AddRule( name, urlPattern, new Dictionary<string, string>() { { "action", action }, { "controller", controller } }, queryKeys );

      return routingTable;
    }


  }
}
