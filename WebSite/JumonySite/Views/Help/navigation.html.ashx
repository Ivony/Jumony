<%@ WebHandler Language="C#" Class="Navigation" %>

using System;
using System.Web;
using System.Linq;
using Ivony.Html.Web;
using Ivony.Fluent;
using Ivony.Web;
using Ivony.Html;
using Jumony.Demo.HelpCenter;
using System.Collections.Generic;

public class Navigation : ViewHandler<HelpEntry[]>
{

  protected override void ProcessScope()
  {

    entryName = ViewContext.ParentActionViewContext.RouteData.Values["name"].CastTo<string>();
    var entries = Model;

    var categoryList = Scope.AddElement( "ul" );
    var categoryGroup = entries.GroupBy( e => e.Category );

    categoryGroup.ForAll( g => Category( g, categoryList ) );



  }

  private string entryName;


  private void Category( IGrouping<string, HelpEntry> grouping, IHtmlContainer categoryList )
  {
    var item = categoryList.AddElement( "li" );
    item.AddElement( "h3" ).InnerText( grouping.Key );
    var entryList = item.AddElement( "ul" );
    foreach ( var entry in grouping )
    {
      if ( entry.Name == entryName )
      {
        var listItem = entryList.AddElement( "li" ).InnerText( entry.Title );
        AddSubTitles( entry.SubTitles, listItem );
      }
      else
        entryList.AddElement( "li" ).AddElement( "a" ).SetAttribute( "action", "Entry" ).SetAttribute( "_name", entry.Name ).InnerText( entry.Title );
    }
  }

  private void AddSubTitles( IDictionary<string, string> subTitles, IHtmlElement listItem )
  {
    var list = listItem.AddElement( "ul" );
    foreach ( var pair in subTitles )
      list.AddElement( "li" ).AddElement( "a" ).SetAttribute( "href", "#" + pair.Key ).InnerText( pair.Value );

  }

}