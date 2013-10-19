using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Forms
{
  public class FormControlException : Exception
  {

    public FormControlException( FormControl control, string message )
      : base( message )
    {
      Control = control;
    }


    public FormControl Control
    {
      get;
      private set;
    }

  }
}
