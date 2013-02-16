using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;

namespace Ivony.Html.Web.Mvc
{
  public static class ResourceManager
  {

    public static void Initialize()
    {

      HostingEnvironment.VirtualPathProvider.GetDirectory( VirtualPathUtility.ToAbsolute( "~/" ) );
 
    }

  }
}
