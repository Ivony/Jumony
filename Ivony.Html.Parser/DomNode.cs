using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Parser
{
  public abstract class DomNode : IHtmlNode
  {

    protected DomNode( DomContainer parent )
    {
      if ( parent != null )
        parent.AddNode( this );
    }


    public IHtmlNodeContainer Container
    {
      get { return DomContainer; }

    }


    private DomContainer _parent;
    internal DomContainer DomContainer
    {
      get
      {
        CheckDisposed();

        return _parent;
      }
      set
      {
        CheckDisposed();

        lock ( SyncRoot )
        {
          if ( _parent != null )
            throw new InvalidOperationException();

          _parent = value;
        }
      }
    }


    public object NodeObject
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

      if ( DomContainer == null )
        throw new InvalidOperationException();

      lock ( SyncRoot )
      {
        _parent.RemoveNode( this );
        _parent = null;

        removed = true;
      }
    }

    public virtual IHtmlDocument Document
    {
      get
      {
        CheckDisposed();

        return DomContainer.Document;
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
