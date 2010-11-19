using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AP = HtmlAgilityPack;

namespace Ivony.Html.HtmlAgilityPackAdaptor
{
  public class HtmlNodeFactory : IHtmlNodeFactory
  {
    private AP.HtmlDocument _document;

    internal HtmlNodeFactory( AP.HtmlDocument document )
    {
      _document = document;
    }



    #region IHtmlNodeFactory 成员

    public IFreeElement CreateElement( string name )
    {
      return new FreeElementAdaptor( this, _document.CreateElement( name ) );
    }

    public IFreeTextNode CreateTextNode( string htmlText )
    {
      return new FreeTextNodeAdaptor( this, _document.CreateTextNode( htmlText ) );
    }

    public IFreeComment CreateComment( string comment )
    {
      return new FreeCommentAdaptor( this, _document.CreateComment( comment ) );
    }



    public HtmlFragment ParseHtml( string html )
    {
      var document = new AP.HtmlDocument();

      document.LoadHtml( html );

      var fragment =  new HtmlFragment( this );
      fragment.AddNodesCopy( document.AsDocument().Nodes(), this );

      return fragment;
    }

    #endregion


    private class FreeElementAdaptor : HtmlElementAdapter, IFreeElement
    {

      private HtmlNodeFactory _factory;

      public FreeElementAdaptor( HtmlNodeFactory factory, AP.HtmlNode node )
        : base( node )
      {
        if ( node.ParentNode != null )
          throw new InvalidOperationException();

        _factory = factory;
      }


      public IHtmlNode Into( IHtmlNodeContainer container, int index )
      {
        if ( container == null )
          throw new ArgumentNullException( "container" );

        var containerNode = container as IHtmlContainerNode;
        if ( containerNode == null )
          throw new InvalidOperationException();


        containerNode.ChildNodes.Insert( index, Node );

        return Node.AsNode();
      }


      public IHtmlNodeFactory Factory
      {
        get { return _factory; }
      }


      IHtmlNodeContainer IHtmlNode.Container
      {
        get { return null; }
      }

      IHtmlDocument IHtmlObject.Document
      {
        get { return _factory._document.AsDocument(); }
      }
    }


    private class FreeTextNodeAdaptor : HtmlTextNodeAdapter, IFreeTextNode
    {
      private HtmlNodeFactory _factory;

      public FreeTextNodeAdaptor( HtmlNodeFactory factory, AP.HtmlTextNode node )
        : base( node )
      {
        if ( node.ParentNode != null )
          throw new InvalidOperationException();

        _factory = factory;
      }



      public IHtmlNode Into( IHtmlNodeContainer container, int index )
      {
        if ( container == null )
          throw new ArgumentNullException( "container" );

        var containerNode = container as IHtmlContainerNode;
        if ( containerNode == null )
          throw new InvalidOperationException();


        containerNode.ChildNodes.Insert( index, Node );

        return Node.AsNode();
      }


      public IHtmlNodeFactory Factory
      {
        get { return _factory; }
      }


      IHtmlNodeContainer IHtmlNode.Container
      {
        get { return null; }
      }

      IHtmlDocument IHtmlObject.Document
      {
        get { return _factory._document.AsDocument(); }
      }
    }


    private class FreeCommentAdaptor : HtmlCommentNodeAdapter, IFreeComment
    {
      private HtmlNodeFactory _factory;

      public FreeCommentAdaptor( HtmlNodeFactory factory, AP.HtmlCommentNode node )
        : base( node )
      {
        if ( node.ParentNode != null )
          throw new InvalidOperationException();

        _factory = factory;
      }



      public IHtmlNode Into( IHtmlNodeContainer container, int index )
      {
        if ( container == null )
          throw new ArgumentNullException( "container" );

        var containerNode = container as IHtmlContainerNode;
        if ( containerNode == null )
          throw new InvalidOperationException();


        containerNode.ChildNodes.Insert( index, Node );

        return Node.AsNode();
      }


      public IHtmlNodeFactory Factory
      {
        get { return _factory; }
      }


      IHtmlNodeContainer IHtmlNode.Container
      {
        get { return null; }
      }

      IHtmlDocument IHtmlObject.Document
      {
        get { return _factory._document.AsDocument(); }
      }
    }

  }
}
