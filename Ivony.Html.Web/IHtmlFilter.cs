using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web
{
  public interface IHtmlFilter
  {

    void OnProcessing( HtmlRequestContext context );
    void OnProcessed( HtmlRequestContext context );

    void OnRendering( HtmlRequestContext context );
    void OnRendered( HtmlRequestContext context );

  }
}
