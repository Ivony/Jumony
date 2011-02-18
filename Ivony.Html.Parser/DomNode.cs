using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Parser
{

  /// <summary>
  /// IHtmlNode 的实现
  /// </summary>
  public abstract class DomNode : DomObject, IHtmlNode
  {

    private IDomContainer _container;

    public virtual IHtmlContainer Container
    {
      get
      {
        CheckDisposed();
        return _container;
      }

      internal set
      {
        lock ( SyncRoot )
        {
          CheckDisposed();

          if ( _container != null )
            throw new InvalidOperationException();


          var domContainer = value as IDomContainer;

          if ( domContainer == null )
            throw new InvalidOperationException();

          _container = domContainer;

        }
      }
    }


    public virtual void Remove()
    {
      if ( removed )
        return;

      if ( Container == null )
        throw new InvalidOperationException();

      lock ( SyncRoot )
      {
        _container.NodeCollection.Remove( this );
        _container = null;
        removed = true;
      }
    }

    public override IHtmlDocument Document
    {
      get
      {
        CheckDisposed();

        return Container.Document;
      }
    }

    protected bool removed = false;

    protected void CheckDisposed()
    {
      if ( removed )
        throw new ObjectDisposedException( ObjectName );
    }

    protected abstract string ObjectName
    {
      get;
    }



    public override string ToString()
    {
      CheckDisposed();

      return this.OuterHtml();
    }
  }
}
