using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Parser
{
  public class DomComment : DomNode, IHtmlComment
  {

    private readonly string _comment;

    public DomComment( DomContainer parent, string comment )
      : base( parent )
    {
      _comment = comment;
    }

    protected override string ObjectName
    {
      get { return "CommentNode"; }
    }


    #region IHtmlCommentNode 成员

    public string Comment
    {
      get { return _comment; }
    }

    #endregion
  }

  internal class DomFreeComment : HtmlNodeWrapper, IFreeComment
  {

    DomFactory _factory;
    DomComment _node;

    private bool disposed;
    private void CheckDisposed()
    {
      if ( disposed )
        throw new ObjectDisposedException( "FreeComment" );
    }




    public DomFreeComment( DomFactory factory, string comment )
    {
      _factory = factory;
      _node = new DomComment( null, comment );
    }



    IHtmlDocument IHtmlNode.Document
    {
      get
      {
        CheckDisposed();

        return _factory.Document;
      }
    }


    #region IHtmlComment 成员

    public string Comment
    {
      get
      {
        CheckDisposed();

        return _node.Comment;
      }
    }

    #endregion

    #region IFreeNode 成员

    public IHtmlNode Into( IHtmlContainer container, int index )
    {
      CheckDisposed();

      if ( container == null )
        throw new ArgumentNullException( "container" );

      var domContainer = container as DomContainer;
      if ( domContainer == null )
        throw new InvalidOperationException();

      domContainer.InsertNode( index, _node );

      disposed = true;
      return _node;
    }

    public IHtmlNodeFactory Factory
    {
      get
      {
        CheckDisposed();

        return _factory;
      }
    }

    #endregion

    protected override IHtmlNode Node
    {
      get
      {
        CheckDisposed();

        return _node;
      }
    }
  }

}
