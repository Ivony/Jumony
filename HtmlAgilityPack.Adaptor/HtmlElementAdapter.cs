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
  internal class HtmlElementAdapter : HtmlContainerAdapter, IHtmlElement
  {
    private AP.HtmlNode _node;

    public HtmlElementAdapter( AP.HtmlNode node )
      : base( node )
    {
      if ( node.NodeType != AP.HtmlNodeType.Element )
        throw new ArgumentException( "只能从 NodeType 为 Element 的 HtmlNode 转换为 HtmlElement", "node" );

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


    #region IHtmlNode 成员

    public IHtmlContainer Container
    {
      get { return Node.ParentNode.AsContainer(); }
    }

    #endregion
  }
}
