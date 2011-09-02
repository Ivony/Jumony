using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Hosting;

namespace Ivony.Html.Web.Mvc
{
  public class WarmUpProvider : IProcessHostPreloadClient
  {
    public void Preload( string[] parameters )
    {
      MvcEnvironment.WarmUp();
    }
  }
}
