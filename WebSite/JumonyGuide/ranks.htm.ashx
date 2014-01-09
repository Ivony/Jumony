<%@ WebHandler Language="C#" Class="ranks" %>

using System;
using System.Web;
using System.Linq;
using Ivony.Fluent;
using Ivony.Html;
using Ivony.Html.Web;
using Ivony.Html.Parser;

public class ranks : HtmlHandler
{
  protected override void ProcessGet()
  {

    var data = new[]
      {
        new { Name = "Jumony", Votes = 365, Url = "http://jumony.codepldex.com/" },
        new { Name = "博客园", Votes = 128, Url = "http://www.cnblogs.com/" },
        new { Name = "Ivony", Votes = 40, Url = "http://Ivony.cnblogs.com/" },
      };

    var rankItems = Find( ".ranks > tr" );

    data.BindTo( rankItems, ( dataItem, element ) =>
      {

        var nameContainer = element.Find( ".name" ).Single();
        var votesContainer = element.Find( "span[style*=color: red;]" ).Single();
        var linkContainer = element.Find( "a" ).Single();

        nameContainer.InnerText( dataItem.Name );
        votesContainer.InnerText( dataItem.Votes.ToString() );
        linkContainer.InnerText( dataItem.Url );
        linkContainer.SetAttribute( "href", dataItem.Url );

      } );

  }
}