using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Web
{
  public interface IVirtualPathBasedProvider
  {

    object GetService( string virtualPath, Type serviceType );

  }
}
