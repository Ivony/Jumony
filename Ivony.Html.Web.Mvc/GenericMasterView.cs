using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web.Mvc
{

  /// <summary>
  /// JumonyMasterView 的标准实现
  /// </summary>
  internal class GenericMasterView : JumonyMasterView
  {

    internal GenericMasterView( string virtualPath )
    {
      Initialize( virtualPath );
    }


    protected override void ProcessScope( IHtmlContainer container )
    {

    }
  }
}
