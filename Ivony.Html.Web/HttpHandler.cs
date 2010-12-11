using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web
{

  /// <summary>
  /// 此类型用于包装 HtmlHandler 处理 HTTP 请求
  /// </summary>
  internal sealed class HttpHandler : JumonyHandler
  {

    private IHtmlHandler _handler;

    public HttpHandler( IHtmlHandler handler )
    {
      if ( handler == null )
        throw new ArgumentNullException( "handler" );
    }

    protected override void ProcessDocument()
    {
      _handler.ProcessDocument( Document );
    }

    public override void Dispose()
    {

      _handler.Dispose();

    }

  }
}
