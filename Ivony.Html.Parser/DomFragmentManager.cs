using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Parser
{
  public class DomFragmentManager : IHtmlFragmentManager
  {


    private object _sync = new object();
    public object SyncRoot
    {
      get { return _sync; }
    }

    private DomDocument _document;
    private IList<DomFragment> _fragments;

    public DomFragmentManager( DomDocument document )
    {
      lock ( document.SyncRoot )
      {
        if ( document.FragmentManager != null )
          throw new InvalidOperationException();

        _document = document;

        _fragments = new SynchronizedCollection<DomFragment>( SyncRoot );
      }
    }

    public IHtmlDocument Document
    {
      get { return _document; }
    }

    public IEnumerable<IHtmlFragment> AllFragments
    {
      get
      {
        return _fragments.Cast<IHtmlFragment>();
      }
    }

    public IHtmlFragment CreateFragment()
    {
      return new DomFragment( this );
    }

    public IHtmlFragment ParseFragment( string html )
    {
      return new DomFragment( this, html );
    }

    public void Allocated( DomFragment fragment )
    {
      lock ( SyncRoot )
      {
        _fragments.Remove( fragment );
      }
    }
  }
}
