using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Ivony.Html.Web;

namespace Jumony.Demo.SimpleTodo
{
  public class SimpleTodoAreaRegistration : AreaRegistration
  {

    public override string AreaName
    {
      get { return "SimpleTodo"; }
    }

    public override void RegisterArea( AreaRegistrationContext context )
    {
      context.SimpleRouteTable()
      .MapAction( "~/SimpleTodo", "Todo", "Index" )
      .MapAction( "~/SimpleTodo/add", "Todo", "Add" )
      .MapAction( "~/SimpleTodo/modify/{taskId}", "Todo", "Modify" )
      .MapAction( "~/SimpleTodo/complete/{taskId}", "Todo", "Complete" )
      .MapAction( "~/SimpleTodo/revert/{taskId}", "Todo", "Revert" )
      .MapAction( "~/SimpleTodo/remove/{taskId}", "Todo", "Remove" );
    }
  }
}
