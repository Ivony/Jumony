using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{
  public class CssCasecadingSelector
  {

    private readonly string _relative;
    public string Relative
    {
      get { return _relative; }
    }

    private readonly CssElementSelector _selector;
    public CssElementSelector ElementSelector
    {
      get { return _selector; }
    }

    private readonly CssCasecadingSelector _parent;
    public CssCasecadingSelector ParentSelector
    {
      get { return _parent; }
    }


    public CssCasecadingSelector( CssElementSelector selector ) : this( selector, null, null ) { }

    public CssCasecadingSelector( CssElementSelector selector, string relative, CssCasecadingSelector parent )
    {
      _selector = selector;
      _relative = relative;
      _parent = parent;
    }


    public bool Allows( IHtmlElement element, IHtmlContainer scope )
    {

      if ( !ElementSelector.IsEligible( element ) )
        return false;

      if ( Relative == null )
        return true;

      else if ( Relative == ">" )
        return element.Parent().Equals( scope ) ? false : ParentSelector.Allows( element.Parent(), scope );

      else if ( Relative == "" )
        return element.Ancestors().TakeWhile( e => !e.Equals( scope ) ).Any( e => ParentSelector.Allows( e, scope ) );

      else if ( Relative == "+" )
        return ParentSelector.Allows( element.PreviousElement(), scope );

      else if ( Relative == "~" )
        return element.SiblingsBeforeSelf().Any( e => ParentSelector.Allows( e, scope ) );

      else
        throw new FormatException();
    }

    public override string ToString()
    {
      if ( Relative == null )
        return ElementSelector.ToString();

      else if ( Relative == "" )
        return string.Format( "{0} {1}", ParentSelector, ElementSelector );

      else
        return string.Format( "{0} {1} {2}", ParentSelector, Relative, ElementSelector );
    }

  }
}
