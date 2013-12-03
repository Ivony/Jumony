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


/// <summary>
/// HtmlHandlerTest 的摘要说明
/// </summary>
public class HtmlHandlerTest : TestClass
{

  public static readonly string testContent = "Test Content";

  public void HandlerTest()
  {

    var document = ExecuteDocument( "~/HandlerTest1.html" );
    Assert.AreEqual( document.FindFirst( "body" ).InnerText(), testContent );


  }


  public string ExecuteContent( string virtualPath )
  {
    var response = new TestJumonyHandler().ProcessRequest( new HttpContextWrapper( HttpContext ), virtualPath );
    return response.CastTo<RawResponse>().Content;
  }

  public IHtmlDocument ExecuteDocument( string virtualPath )
  {

    return new JumonyParser().Parse( ExecuteContent( virtualPath ) );
  }



}