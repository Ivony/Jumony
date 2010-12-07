using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;

namespace Ivony.Html.Web.Mvc
{
  public interface IHtmlHandlerFactory
  {

    IHtmlHandler CreateHandler( RequestContext context );


  }
}
