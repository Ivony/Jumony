<%@ WebHandler Language="C#" Class="网易" %>

using System;
using System.Web;
using Ivony.Fluent;
using Ivony.Web.Html;
using System.Linq;

public class 网易 : Ivony.Web.Html.HtmlAgilityPackAdaptor.HtmlHandlerAdapter
{


  protected override void Process()
  {
    //Find( "script" ).BindAction( e => e.Remove() );
    Find( "object" ).BindAction( e => e.Remove() );
    Find( "embbed" ).BindAction( e => e.Remove() );
    Find( "iframe" ).BindAction( e => e.Remove() );

    HtmlBindingContext.Current.Commit();

    Enumerable.Range( 1, 100 )
      .BindTo( Find( "div.content div.rightMain a" ), ( dataItem, element ) =>
      {
        element.Bind( "@:text", dataItem, "新闻{0}" );
      } );

    //HtmlBindingContext.Current.Discard();
    //Find( "a" ).ForAll( ( e, i ) => e.Bind( "@:text", i, "链接{0}" ) );
  }
}