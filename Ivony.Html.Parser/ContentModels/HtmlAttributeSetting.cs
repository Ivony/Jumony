using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Parser.ContentModels
{
  public sealed class HtmlAttributeSetting : HtmlContentFragment
  {

    public HtmlAttributeSetting( HtmlContentFragment info, string name, string value )
      : base( info )
    {
      if ( name == null )
        throw new ArgumentNullException( "name" );

      Name = name;
      Value = value;
    }

    public string Name
    {
      get;
      private set;
    }

    public string Value
    {
      get;
      private set;
    }

  }
}
