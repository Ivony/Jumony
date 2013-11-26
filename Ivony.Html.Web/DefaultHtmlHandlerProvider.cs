using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Compilation;
using System.Web.Hosting;

namespace Ivony.Html.Web
{
  public class DefaultHtmlHandlerProvider : IHtmlHandlerProvider
  {
    public IHtmlHandler GetHandler( string virtualPath )
    {

      var handlerPath = virtualPath + ".ashx";
      if ( !HostingEnvironment.VirtualPathProvider.FileExists( handlerPath ) )
        return null;

      return BuildManager.CreateInstanceFromVirtualPath( handlerPath, typeof( IHtmlHandler ) ) as IHtmlHandler;

    }
  }
}
