<%@ WebHandler Language="C#" Class="links_aspx" %>

using System;
using System.Web;
using Ivony.Fluent;
using System.Linq;
using Ivony.Html;
using Ivony.Html.Web;
using Ivony.Html.Parser;
using Ivony.Html.Styles;

public class links_aspx : HtmlHandler
{

  protected override void ProcessGet()
  {

    var dataList = Find( "#DataList" ).Single();
    dataList.Style().SetValue( "font-size", "12px" );
    dataList.SetAttribute( "cellpadding", "3" );

    var header = dataList.Elements( "tr" ).First();
    header.Style().SetValue( "text-align", "center" );
    header.Style().SetValue( "font-weight", "bold" );

    dataList.Elements( "tr:nth-child(even)" ).ForAll( e => e.Style().SetValue( "background", "#EEEEEE" ) );

    dataList.Descendants( "td:nth-child( 2 )" ).ForAll( e =>//nth-child( 2 )就是选择在父元素中排行第2的元素，TD的父元素总是TR，所以这里会选出第二列的TD元素出来
      {
        if ( e.InnerText() == "True" )
          e.PreviousElement().Style().SetValue( "color", "red" );//PreviousElement就是前一个元素，也就是第一列。

        e.Style().SetValue( "display", "none" );//在枚举循环过程中不能删除元素，因为这会改变枚举集合而导致异常。
        //在后面的文章中，我再详细介绍怎么用Jumony Binding安全的在枚举中删除元素（因为Binding可以将删除操作延迟）。
      } );
  }
}