using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Parser
{

  /// <summary>
  /// IHtmlComment 的实现
  /// </summary>
  public class DomComment : DomNode, IHtmlComment
  {

    private readonly string _comment;

    /// <summary>
    /// 创建 DomComment 实例
    /// </summary>
    /// <param name="comment">注释内容</param>
    public DomComment( string comment )
    {
      _comment = comment;
    }

    /// <summary>
    /// 获取一个名称，用于在抛出 ObjectDisposedException 异常时说明自己
    /// </summary>
    protected override string ObjectName
    {
      get { return "CommentNode"; }
    }

    /// <summary>
    /// 获取注释内容
    /// </summary>
    public string Comment
    {
      get { return _comment; }
    }
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
      _node = new DomComment( comment );
    }



    IHtmlDocument IHtmlDomObject.Document
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

      var domContainer = container as IDomContainer;
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
