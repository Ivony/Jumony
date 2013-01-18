using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Ivony.Html.Web.Mvc
{
  public interface IMasterView
  {

    void Initialize( ViewContext context );

    string Render( IMasterContentView view );

  }
}
