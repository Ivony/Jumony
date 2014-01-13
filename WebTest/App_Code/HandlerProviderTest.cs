using Ivony.Web.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

using Ivony.Html.Web;

/// <summary>
/// RequestRouteTest 的摘要说明
/// </summary>
public class HandlerProviderTest : TestClass
{
  public void HandlerProviderTest1()
  {

    Assert.IsNotNull( HtmlHandlerProvider.GetHandler( "~/RouteTest/DefaultHandler/Test1.html" ), "不能获取默认的 HTML 处理程序" );
    Assert.IsNull( HtmlHandlerProvider.GetHandler( "~/RouteTest/DefaultHandler/Test2.html" ), "错误的获取了默认的 HTML 处理程序" );

    Assert.IsNotNull( HtmlHandlerProvider.GetHandler( "~/RouteTest/NonDefaultHandler/Test1.html" ), "未能获取 HTML 处理程序" );
    Assert.IsNull( HtmlHandlerProvider.GetHandler( "~/RouteTest/NonDefaultHandler/Test2.html" ), "错误的获取了 HTML 处理程序" );


  }


}

namespace Ivony.Html.Web
{

  public static class Extensions
  {
    public static void IsNotNull<T>( this TestAssert assert, T obj, string message = null ) where T : class
    {
      if ( obj == null )
        assert.Failure( message ?? "断言失败，期望对象不为空，但对象为空" );
    }
  }
}