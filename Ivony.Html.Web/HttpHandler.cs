using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Ivony.Html.Web
{

  /// <summary>
  /// 此类型用于包装 IHtmlHandler 处理 HTTP 请求
  /// </summary>
  internal sealed class HttpHandler : JumonyHandler
  {

    private IHtmlHandler _handler;

    public HttpHandler( IHtmlHandler handler )
    {
      if ( handler == null )
        throw new ArgumentNullException( "handler" );

      _handler = handler;
    }

    protected override void ProcessDocument()
    {
      _handler.ProcessDocument( new HttpContextWrapper( HttpContext.Current ), Document );
    }

    public override void Dispose()
    {
      _handler.Dispose();

    }

  }
}
