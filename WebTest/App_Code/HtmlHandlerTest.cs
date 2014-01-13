using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

using Ivony.Fluent;
using Ivony.Html;
using Ivony.Html.Parser;
using Ivony.Web.Test;
using Ivony.Web;
using Ivony.Html.Web;


/// <summary>
/// HtmlHandlerTest 的摘要说明
/// </summary>
public class HtmlHandlerTest : TestClass
{

  public static readonly string testContent = "Test Content";

  public void HandlerTest()
  {

    var document = ExecuteDocument( "~/HandlerTest/Test1.html" );
    Assert.AreEqual( document.FindFirst( "body" ).InnerText(), testContent );

    Assert.IsNull( HttpContext.Request.RequestContext.RouteData.DataTokens[JumonyRequestRoute.VirtualPathToken], "VirtualPath token 没有清理" );
    Assert.IsNull( HttpContext.Request.RequestContext.RouteData.DataTokens[JumonyRequestRoute.HtmlHandlerToken], "HtmlHandler token 没有清理" );

  }


  protected string ExecuteContent( string virtualPath )
  {
    var response = TestJumonyHandler.Render( new HttpContextWrapper( HttpContext ), virtualPath );
    return response.CastTo<RawResponse>().Content;
  }

  protected IHtmlDocument ExecuteDocument( string virtualPath )
  {

    return new JumonyParser().Parse( ExecuteContent( virtualPath ) );
  }



}