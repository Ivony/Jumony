using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web.Mvc
{
  public class GenericMasterView : JumonyMasterView
  {

    internal GenericMasterView( string virtualPath )
    {
      Initialize( virtualPath );
    }

    protected override void Process( IHtmlContainer container )
    {

    }
  }
}
