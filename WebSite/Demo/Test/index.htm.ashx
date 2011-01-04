<%@ WebHandler Language="C#" Class="index" %>

using System;
using System.Web;
using System.Linq;//没有LINQ将寸步难行
using Ivony.Fluent;//提供Fluent体验的一些扩展方法
using Ivony.Html;//定义了HTML DOM
using Ivony.Html.Binding;//提供数据绑定支持
using Ivony.Html.Parser;//提供HTML分析支持
using Ivony.Html.Web;

public class index : JumonyHandler//一个使用Jumony Parser作为分析器的标准Jumony体系的HttpHandler
{
  protected override void ProcessDocument()
  {
    var body = Document.Find( "body" ).Single();
    body.InnerText( "Hello Jumony!" );
  }
}