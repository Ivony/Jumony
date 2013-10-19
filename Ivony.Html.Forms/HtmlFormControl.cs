using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Forms
{
  public abstract class HtmlFormControl
  {


    public abstract HtmlForm Form { get; }

    public abstract string Name { get; }

    public abstract string Value { get; }

    public abstract bool CanSetValue( string value );


  }
}
