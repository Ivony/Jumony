using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Ivony.Html.Web;

namespace Jumony.Demo.HelpCenter
{
  public class HelpAreaRegistration : AreaRegistration
  {

    public override string AreaName
    {
      get { return "Help"; }
    }

    public override void RegisterArea( AreaRegistrationContext context )
    {
      context.SimpleRouteTable()
        .MapRoute( "~/help", new { controller = "Help", action = "Entry" } )
        .MapRoute( "~/help/{action}", new { controller = "Help" } );
    }
  }
}
