using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Ivony.Html.Web
{
  public abstract class RawResponse
  {

    public abstract void Output( HttpResponseBase response );

  }
}
