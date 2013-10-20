using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Forms
{
  public class FormValueFormatException : FormatException
  {

    public FormControl Control
    {
      get;
      private set;
    }


    public FormValueFormatException( FormControl control, string message )
      : base( message )
    {
      Control = control;
    }


  }
}
