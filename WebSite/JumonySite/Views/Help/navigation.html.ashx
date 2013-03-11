<%@ WebHandler Language="C#" Class="Navigation" %>

using System;
using System.Web;
using System.Linq;
using Ivony.Html.Web;
using Ivony.Fluent;
using Ivony.Web;
using Ivony.Html;
using Jumony.Demo.HelpCenter;

public class Navigation : JumonyViewHandler
{

  protected override void ProcessScope( IHtmlContainer scope )
  {

    var entries = ViewModel as HelpEntry[];

    var categoryList = Scope.AddElement( "ul" );
    var categoryGroup = entries.GroupBy( e => e.Category );

    categoryGroup.ForAll( g => Category( g, categoryList ) );

  }

  private void Category( IGrouping<string, HelpEntry> grouping, IHtmlContainer categoryList )
  {
    var item = categoryList.AddElement( "li" );
    item.AddElement( "h3" ).InnerText( grouping.Key );
    var entryList = item.AddElement( "ul" );
    foreach ( var entry in grouping )
      entryList.AddElement( "li" ).AddElement( "a" ).SetAttribute( "action", "Entry" ).SetAttribute( "_name", entry.Name ).InnerText( entry.Title );
  }

}