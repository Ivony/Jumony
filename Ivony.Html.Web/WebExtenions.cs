using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using System.Web;
using System.Web.Caching;

namespace Ivony.Html.Web
{

  /// <summary>
  /// 提供一些 Web 相关的扩展方法。
  /// </summary>
  public static class WebExtenions
  {


    private const string requestDataToken = "Jumony_HttpContext_RequestMapping";


    /// <summary>
    /// 获取请求的映射信息
    /// </summary>
    /// <param name="context">请求上下文</param>
    /// <returns>映射信息</returns>
    public static RequestMapping GetMapping( this HttpContextBase context )
    {
      return (RequestMapping) context.Items[requestDataToken];
    }


    internal static void SetMapping( this HttpContextBase context, RequestMapping data )
    {
      context.Items[requestDataToken] = data;
    }

    internal static void ApplyMapping( this HttpContextBase context, RequestMapping data )
    {
      if ( data == null )
        throw new ArgumentNullException( "data" );

      var httpHandler = GetHttpHandler( data.Handler );

      SetMapping( context, data );
      context.RemapHandler( httpHandler );
    }


    internal static JumonyHandler GetHttpHandler( IHtmlHandler handler )
    {
      var httpHandler = handler as JumonyHandler;

      if ( httpHandler == null )
        httpHandler = new HtmlHandlerWrapper( handler );
      return httpHandler;
    }


  }
}
