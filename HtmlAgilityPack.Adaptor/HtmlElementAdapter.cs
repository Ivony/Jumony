using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AP = HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;

namespace Ivony.Web.Html.HtmlAgilityPackAdaptor
{
  internal class HtmlElementAdapter : HtmlContainerAdapter, IHtmlElement, IEquatable<IHtmlElement>
  {
    private AP.HtmlNode _node;

    public HtmlElementAdapter( AP.HtmlNode node )
      : base( node )
    {
      if ( node.NodeType != AP.HtmlNodeType.Element )
        throw new ArgumentException( "只能从NodeType为Element的HtmlNode转换为HtmlElement", "node" );

      _node = node;
    }

    public string Name
    {
      get { return _node.Name; }
    }

    public IEnumerable<IHtmlAttribute> Attributes()
    {
      return _node.Attributes.Select( attribute => attribute.AsAttribute() );
    }

    public IHtmlAttribute AddAttribute( string attributeName )
    {
      _node.Attributes.Add( attributeName, null );
      return this.Attribute( attributeName );
    }



    private static readonly Regex attributePathRegex = new Regex( @"@(?<name>\w+)" );

    void IHtmlElement.BindCore( HtmlBindingContext context, string path, string value, BindingNullBehavior nullBehavior )
    {

      if ( value == null )
      {
        switch ( nullBehavior )
        {
          case BindingNullBehavior.Ignore:
            break;
          case BindingNullBehavior.Hidden:
            context.Action( this, element => element.Node.SetAttributeValue( "style", element.Node.GetAttributeValue( "style", "" ) + " visibility: hidden;" ) );
            break;
          case BindingNullBehavior.Remove:
            context.Action( this, element => element.Node.Remove() );
            break;
          case BindingNullBehavior.DisplayNone:
            context.Action( this, element => element.Node.SetAttributeValue( "style", element.Node.GetAttributeValue( "style", "" ) + " display: none;" ) );
            break;
          default:
            throw new NotSupportedException();
        }

        return;
      }

      var attributeMatch = attributePathRegex.Match( path );
      if ( attributeMatch.Success )
      {
        string attributeName = attributeMatch.Groups["name"].Value;
        context.Action( this, element => element.Node.SetAttributeValue( attributeName, value ) );
      }

      if ( path == "@:text" )
        context.Action( this, element => element.Node.InnerHtml = HttpUtility.HtmlEncode( value ).Replace( "\r\n", "\n" ).Replace( "\n", "<br />" ) );

      if ( path == "@:html" )
        context.Action( this, element => element.Node.InnerHtml = value );

      return;
    }

    public override string ToString()
    {
      return Node.OuterHtml;
    }


    #region IEquatable<IHtmlElement> 成员

    bool IEquatable<IHtmlElement>.Equals( IHtmlElement other )
    {
      if ( other == null )
        return false;

      return NodeObject == other.NodeObject;
    }

    #endregion


  }
}
