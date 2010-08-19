using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AP = HtmlAgilityPack;

namespace Ivony.Web.Html.HtmlAgilityPackAdaptor
{
  internal class HtmlDocumentAdapter : HtmlContainerAdapter
  {

    private AP.HtmlNode _node;

    public HtmlDocumentAdapter( AP.HtmlNode node )
      : base( node )
    {
      if ( node.NodeType != AP.HtmlNodeType.Document )
        throw new ArgumentException( "只能从NodeType为Document的HtmlNode转换为HtmlDocumentAdapter", "node" );

      _node = node;
    }
  }
}
