using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Parser
{
  public abstract class DomNode : IHtmlNode
  {

    protected DomNode()
    {
    }


    private IDomContainer _container;

    public IHtmlContainer Container
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


          var domContainer = _container as IDomContainer;

          if ( domContainer == null )
            throw new InvalidOperationException();

          _container = domContainer;

        }
      }

    }

    public object RawObject
    {
      get
      {
        CheckDisposed();

        return this;
      }
    }

    public void Remove()
    {
      //CheckDisposed();

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

    public virtual IHtmlDocument Document
    {
      get
      {
        CheckDisposed();

        return Container.Document;
      }
    }

    public virtual string RawHtml
    {
      get
      {
        CheckDisposed();

        return null;
      }
    }

    private readonly object _sync = new object();
    public object SyncRoot
    {
      get
      {
        CheckDisposed();

        return _sync;
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
