using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AP = HtmlAgilityPack;


namespace Ivony.Html.HtmlAgilityPackAdaptor
{
  public abstract class HtmlNodeAdapter : IHtmlNode
  {

    private AP.HtmlNode _node;

    protected HtmlNodeAdapter( AP.HtmlNode node )
    {
      _node = node;
    }

    public AP.HtmlNode Node
    {
      get { return _node; }
    }



    public IHtmlNodeContainer Container
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

      {
        var node = obj as IHtmlNode;
        if ( node != null )
          return NodeObject.Equals( node.NodeObject );
      }

      {
        var node = obj as AP.HtmlNode;
        if ( node != null )
          return NodeObject.Equals( node );
      }


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
      get { return Container.Document; }
    }

    public string RawHtml
    {
      get { return Node.OuterHtml; }
    }


    public object SyncRoot
    {
      get { return Node; }
    }


    public override string ToString()
    {
      return Node.OuterHtml;
    }

  }
}
