using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web.Mvc
{
  public interface IHtmlHandler
  {

    void ProcessDocument( IHtmlDocument document );

  }
}
