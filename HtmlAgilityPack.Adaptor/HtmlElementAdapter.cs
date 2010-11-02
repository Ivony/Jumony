using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AP = HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;

namespace Ivony.Html.HtmlAgilityPackAdaptor
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
