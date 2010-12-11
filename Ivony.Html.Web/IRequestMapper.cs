using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using Ivony.Html.Parser;
using System.Web.Compilation;
using System.Web.Hosting;

namespace Ivony.Html.Web
{
  public interface IRequestMapper
  {

    MapInfo MapRequest( HttpRequest request );

  }

}
