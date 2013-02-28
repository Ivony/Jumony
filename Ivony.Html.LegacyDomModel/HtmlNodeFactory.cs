using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{
  internal class HtmlNodeFactory : IHtmlNodeFactory
  {

    private IHtmlFragmentManager _fragmentManager;

    public HtmlNodeFactory( IHtmlFragmentManager fragmentManager )
    {
      _fragmentManager = fragmentManager;
    }

    public IHtmlDocument Document
    {
      get { return _fragmentManager.Document; }
    }

    public HtmlFragment ParseFragment( string html )
    {
      throw new NotSupportedException();
    }

    public HtmlFragment CreateFragment()
    {
      return new HtmlFragment( this );
    }

    public IFreeElement CreateElement( string name )
    {
      var fragment = _fragmentManager.CreateFragment();
      fragment.AddElement( name );
      return new FreeElement( fragment, this );
    }

    public IFreeTextNode CreateTextNode( string htmlText )
    {
      var fragment = _fragmentManager.CreateFragment();
      fragment.AddTextNode( htmlText );
      return new FreeTextNode( fragment, this );
      throw new NotImplementedException();
    }

    public IFreeComment CreateComment( string comment )
    {
      var fragment = _fragmentManager.CreateFragment();
      fragment.AddComment( comment );
      return new FreeComment( fragment, this );
      throw new NotImplementedException();
    }
  }
}
