using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Parser.ContentModels
{
  public sealed class HtmlSpecialTag : HtmlContentFragment
  {

    public HtmlSpecialTag( HtmlContentFragment fragment, string content, string speciaSymbol )
      : base( fragment )
    {
      if ( content == null )
        throw new ArgumentNullException( "content" );

      if ( speciaSymbol == null )
        throw new ArgumentNullException( "speciaSymbol" );

      Content = content;
      SpecialSymbol = speciaSymbol;
    }

    public string Content
    {
      get;
      protected set;
    }

    public string SpecialSymbol
    {
      get;
      protected set;
    }

  }
}
