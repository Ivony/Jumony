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

public class Navigation : ViewHandler<HelpTopic>
{

  protected override void ProcessScope()
  {

    var parentsList = Scope.AddElement( "ul" );

    {
      var topic = Model;
      while ( true )
      {
        var parent = topic.Parent;
        if ( parent == null )
          break;

        parentsList.AddElement( 0, "li" ).AddElement( "a" ).SetAttribute( "action", "Entry" ).SetAttribute( "_path", parent.HelpPath ).InnerText( parent.Title );
        topic = parent;
      }
    }

    var siblingList = parentsList.AddElement( "li" ).AddElement( "ul" );

    {
      var parent = Model.Parent;
      if ( parent != null )
      {
        foreach ( var sibling in parent.Childs )
        {
          if ( sibling != Model )
            siblingList.AddElement( "li" ).AddElement( "a" ).SetAttribute( "action", "Entry" ).SetAttribute( "_path", sibling.HelpPath ).InnerText( sibling.Title );
        }
      }
    }

    var selfNode = siblingList.AddElement( "li" );
    selfNode.InnerText( Model.Title );
    var childsList = selfNode.AddElement( "ul" );
    foreach ( var child in Model.Childs )
    {
      childsList.AddElement( "li" ).AddElement( "a" ).SetAttribute( "action", "Entry" ).SetAttribute( "_path", child.HelpPath ).InnerText( child.Title );
    }


  }

  protected void ProcessBody( IHtmlElement body )
  {

  }

}