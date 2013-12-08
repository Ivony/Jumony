using Ivony.Html.Web;
using Ivony.Web.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// DefaultProviderTest 的摘要说明
/// </summary>
public class DefaultProviderTest : TestClass
{

  public void DefaultContentService()
  {
    Assert.HasAny( HtmlServices.Default.GetContentServices( "~/1.htm" ).OfType<StaticFileContentProvider>(), ".htm 扩展名未能获得 StaticFileContentProvider" );
    Assert.HasAny( HtmlServices.Default.GetContentServices( "~/1.html" ).OfType<StaticFileContentProvider>(), ".html 扩展名未能获得 StaticFileContentProvider" );
    Assert.HasAny( HtmlServices.Default.GetContentServices( "~/1.aspx" ).OfType<WebFormPageContentProvider>(), ".aspx 扩展名未能获得 WebFormPageContentProvider" );
  }

}