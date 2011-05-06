using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;

namespace Ivony.Html.Web
{

  /// <summary>
  /// 重写 HTTP 请求映射到 Jumony 处理程序的模块
  /// </summary>
  public sealed class HtmlRewriteModule : IHttpModule
  {


    void IHttpModule.Dispose()
    {
    }

    void IHttpModule.Init( HttpApplication context )
    {
      context.PostResolveRequestCache += new EventHandler( OnPreMapRequestHandler );
    }

    private void OnPreMapRequestHandler( object sender, EventArgs e )
    {
      var context = HttpContext.Current;


      var request = context.Request;

      var result = HtmlProviders.MapRequest( new HttpRequestWrapper( request ) );

      if ( result == null )
        return;


      var handler = result.Handler;

      var httpHandler = handler as JumonyHandler;

      if ( httpHandler == null )
        httpHandler = new HttpHandler( handler );


      context.SetMapping( result );

      context.RemapHandler( httpHandler );

    }

  }
}
