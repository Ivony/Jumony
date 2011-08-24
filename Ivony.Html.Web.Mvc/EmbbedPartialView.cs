using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web.Mvc
{

#if DEBUG
  public abstract class EmbbedPartialView : PartialView
  {


    internal const string EmbbedPartialViewDataName = "Jumony_Embbed_Partial";


    protected override IHtmlContainer LoadContainer()
    {

      return ViewContext.ParentActionViewContext.TempData[EmbbedPartialViewDataName] as IHtmlContainer;

    }

  }
#endif
}
