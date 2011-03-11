using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Ivony.Html.Web.Mvc
{
  public interface ICachableResult
  {

    ActionResult CachedResult
    {
      get;
    }

  }
}
