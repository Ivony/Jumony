using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AP = HtmlAgilityPack;


namespace Ivony.Web.Html.HtmlAgilityPackAdaptor
{
  internal class HtmlNodeAdapter : IHtmlNode
  {

    private AP.HtmlNode _node;

    public HtmlNodeAdapter( AP.HtmlNode node )
    {
      _node = node;
    }

    public AP.HtmlNode Node
    {
      get { return _node; }
    }



    public IHtmlContainer Parent
    {
      get
      {
        if ( _node.ParentNode == null )
          return null;

        return _node.ParentNode.AsContainer();
      }
    }


    public object NodeObject
    {
      get { return Node; }
    }


    public override bool Equals( object obj )
    {

      if ( obj == null )
        return false;

      var element = obj as IHtmlElement;
      if ( element != null )
        return NodeObject.Equals( element.NodeObject );


      var node = obj as AP.HtmlNode;
      if ( node != null )
        return NodeObject.Equals( node );


      return base.Equals( obj );
    }

    public override int GetHashCode()
    {
      return NodeObject.GetHashCode();
    }


    public void Remove()
    {
      Node.Remove();
    }

    public IHtmlDocument Document
    {
      get { return Node.OwnerDocument.AsDocument(); }
    }
  }
}
