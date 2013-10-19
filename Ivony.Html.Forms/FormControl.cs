using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Forms
{
  public abstract class FormControl
  {

    protected FormControl( HtmlForm form )
    {
      Form = form;
    }

    public HtmlForm Form
    {
      get;
      private set;
    }

    public abstract string Name { get; }

    public abstract string Value { get; }

    public abstract bool CanSetValue( string value );


  }
}
