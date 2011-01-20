using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Ivony.Html.Web
{
  public interface ICachedResponse
  {

    void Apply( HttpResponseBase response );

  }
}
