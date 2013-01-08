<%@ WebHandler Language="C#" Class="DomTree" %>

using System;
using System.Web;
using System.Linq;
using Ivony.Html;
using Ivony.Html.Web;
using Ivony.Html.Web.Mvc;

public class DomTree : ViewHandler<IHtmlDocument>
{

  private ICssSelector selector;

  protected override void ProcessDocument()
  {

    string title;
    var titleElement =  ViewModel.Find( "title" ).FirstOrDefault();
    if ( titleElement != null )
      title = titleElement.InnerHtml();
    else
      title = "无标题文档";

    Document.FindSingle( "title" ).InnerHtml( "Jumony Core Demo - " + title );
    ViewData["title"] = title;


    selector = ViewData["Selector"] as ICssSelector;

    foreach ( var node in ViewModel.Nodes() )
    {
      ProcessNode( Document.FindSingle( ".body" ), node );
    }

    ViewData["SelectedElements"] = selectedElements;

  }

  private int selectedElements = 0;


  private void ProcessNode( IHtmlElement container, IHtmlNode node, bool encodeWhiteSpace = false )
  {

    var special = node as IHtmlSpecial;
    if ( special != null )
    {
      container.AddElement( "div" ).SetAttribute( "class", "special" ).InnerText( special.RawHtml );
      return;
    }

    var comment = node as IHtmlComment;
    if ( special != null )
    {
      container.AddElement( "div" ).SetAttribute( "class", "comment" ).InnerText( comment.RawHtml );
      return;
    }

    var textNode = node as IHtmlTextNode;
    if ( textNode != null )
    {
      container.AddElement( "div" ).SetAttribute( "class", "text" ).InnerText( textNode.HtmlText, encodeWhiteSpace );
      return;
    }

    var element = node as IHtmlElement;
    if ( element != null )
    {
      bool selfClosed = HtmlSpecification.selfCloseTags.Contains( element.Name );

      if ( selector != null && selector.IsEligible( element ) )
      {
        container = container.AddElement( "div" ).SetAttribute( "class", "selected" );
        selectedElements++;
      }

      var beginTag = container.AddElement( "div" ).SetAttribute( "class", "beginTag tag" );

      beginTag.AddElement( "span" ).SetAttribute( "class", "brackets" ).InnerText( "<" );
      beginTag.AddElement( "span" ).SetAttribute( "class", "elementName" ).InnerText( element.Name );

      foreach ( var attribute in element.Attributes() )
      {
        beginTag.AddTextNode( " " );
        beginTag.AddElement( "span" ).SetAttribute( "class", "attributeName" ).InnerText( attribute.Name );
        if ( attribute.AttributeValue != null )
        {
          beginTag.AddTextNode( "=" );
          var attributeValue = beginTag.AddElement( "span" ).SetAttribute( "class", "attributeValue" );
          attributeValue.AddElement( "span" ).SetAttribute( "class", "quote" ).InnerText( "\"" );
          attributeValue.AddTextNode( HtmlEncoding.HtmlAttributeEncode( attribute.AttributeValue ) );
          attributeValue.AddElement( "span" ).SetAttribute( "class", "quote" ).InnerText( "\"" );
        }
      }

      beginTag.AddElement( "span" ).SetAttribute( "class", "brackets" ).InnerText( ">" );


      var _encodeWhiteSpace = false;
      if ( HtmlSpecification.cdataTags.Contains( element.Name ) )
        _encodeWhiteSpace = true;


      if ( !selfClosed && element.Nodes().Any() )
      {
        var childsContainer = container.AddElement( "div" ).SetAttribute( "class", "childs" );
        foreach ( var childNode in element.Nodes() )
          ProcessNode( childsContainer, childNode, _encodeWhiteSpace );
      }

      if ( !selfClosed )
      {
        var endTag = container.AddElement( "div" ).SetAttribute( "class", "endTag tag" );
        endTag
          .AddElement( "span" ).SetAttribute( "class", "brackets" ).InnerText( "<" )
          .AddElement( "span" ).SetAttribute( "class", "slash" ).InnerText( "/" )
          .AddElement( "span" ).SetAttribute( "class", "elementName" ).InnerText( element.Name )
          .AddElement( "span" ).SetAttribute( "class", "brackets" ).InnerText( ">" );
      }


      return;
    }


  }

}