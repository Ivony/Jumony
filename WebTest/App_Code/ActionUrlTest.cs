using Ivony.Web.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using Ivony.Web;
using System.Web.Mvc;
using System.IO;
using System.Web.Mvc.Html;
using Ivony.Html;
using Ivony.Html.Parser;

/// <summary>
/// ActionUrlTest 的摘要说明
/// </summary>
public class ActionUrlTest : TestClass
{



  public override void MethodInitialize()
  {
    var routeData = HttpContext.Request.RequestContext.RouteData;
    routeData.Values.Add( "controller", "Test" );
    routeData.Values.Add( "action", "Test" );

    base.MethodInitialize();
  }

  public void Test1()
  {


    var context = new ControllerContext( HttpContext.Request.RequestContext, new TestController() );

    var result = ViewEngines.Engines.FindView( context, "~/ActionUrlTest/Test1.html", null );

    Assert.NotNull( result.View, "找不到视图" );


    IHtmlDocument document;

    using ( var writer = new StringWriter() )
    {
      result.View.Render( new ViewContext( context, result.View, new ViewDataDictionary(), new TempDataDictionary(), writer ), writer );

      document = new JumonyParser().Parse( writer.ToString() );
    }


    var link = document.FindFirst( "a" );

    Assert.NotNull( link );

    Assert.AreEqual( link.Attribute( "href" ).Value(), "/TestController/TestAction?arg=args" );


  }

  private class TestController : Controller
  {

  }
}