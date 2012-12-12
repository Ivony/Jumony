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



    public IHtmlContainer Container
    {
      get
      {
        if ( _node.ParentNode == null )
          return null;

        return _node.ParentNode.AsContainer();
      }
    }


    public object RawObject
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
          return RawObject.Equals( node.RawObject );
      }

      {
        var node = obj as AP.HtmlNode;
        if ( node != null )
          return RawObject.Equals( node );
      }


      return base.Equals( obj );
    }

    public override int GetHashCode()
    {
      return RawObject.GetHashCode();
    }


    public void Remove()
    {
      Node.Remove();
    }

    public IHtmlDocument Document
    {
      get { return Node.OwnerDocument.AsDocument(); }
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
