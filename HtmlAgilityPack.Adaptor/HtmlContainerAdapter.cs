using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AP = HtmlAgilityPack;


namespace Ivony.Web.Html.HtmlAgilityPackAdaptor
{
  internal abstract class HtmlContainerAdapter : HtmlNodeAdapter, IHtmlContainer
  {
    private AP.HtmlNode _node;

    protected HtmlContainerAdapter( AP.HtmlNode node )
      : base( node )
    {
      if ( node.NodeType != AP.HtmlNodeType.Document && node.NodeType != AP.HtmlNodeType.Element )
        throw new ArgumentException( "只能从NodeType为Document或Element的HtmlNode转换为HtmlContainerAdapter", "node" );

      _node = node;
    }

    public IEnumerable<IHtmlNode> Nodes()
    {
      return _node.ChildNodes.Select( node => node.AsNode() );
    }

  }
}
