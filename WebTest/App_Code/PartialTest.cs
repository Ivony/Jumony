using Ivony.Fluent;
using Ivony.Html;
using Ivony.Html.Parser;
using Ivony.Html.Web;
using Ivony.Web;
using Ivony.Web.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// PartialTest 的摘要说明
/// </summary>
public class PartialTest : TestClass
{

  public static readonly string testContent = "Test Content";

  public void PartialTest1()
  {
    var document = ExecuteDocument( "~/PartialTest/Test1.html" );
    Assert.AreEqual( document.FindFirst( "body" ).InnerText().Trim(), testContent, "部分视图测试失败，没有正确渲染部分视图。" );
  }




  public void PartialTest2()
  {

    var document = ExecuteDocument( "~/PartialTest/Test2.html" );
    Assert.AreEqual( document.FindFirst( "body" ).InnerText().Trim(), testContent, "测试在 Handler 中增加部分视图失败，没能正确的渲染。" );

  }

  public void PartialTest3()
  {

    var document = ExecuteDocument( "~/PartialTest/Test3.html" );
    Assert.AreEqual( document.FindFirst( "#container" ).InnerText(), testContent, "测试在部分视图中处理失败，没能正确的渲染。" );

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