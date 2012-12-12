using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

using Ivony.Html.Web.Mvc;


namespace Test
{


  public class TestAreaRegistration : AreaRegistration
  {
    public override string AreaName
    {
      get { return "Test"; }
    }

    public override void RegisterArea( AreaRegistrationContext context )
    {
      context.SimpleAreaRouteTable().MapAction( "~/Test1", "Test", "Test" );
      //context.SimpleAreaRouteTable().MapAction( "~/Test2", "Test", "Test" );
    }
  }


  public class TestController : Controller
  {

    public ActionResult Test()
    {
      return View();
    }

  }
}
