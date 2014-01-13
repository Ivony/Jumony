<%@ WebHandler Language="C#" Class="index" %>

using System;
using System.Web;
using System.Linq;//没有LINQ将寸步难行
using Ivony.Fluent;//提供Fluent体验的一些扩展方法
using Ivony.Html;//定义了HTML DOM
using Ivony.Html.Web;//ASP.NET支持
using Ivony.Html.Parser;//提供HTML分析支持

public class index : HtmlHandler//一个使用Jumony Parser作为分析器的标准Jumony体系的HttpHandler
{
  protected override void ProcessGet()
  {
    var body = Find( "body" ).Single();
    body.InnerText( "Hello Jumony!" );
  }
}