using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.WebSockets;

namespace Ivony.Web
{

  /// <summary>
  /// 扩展 Controller ，提供WebSocket和JSONP支持
  /// </summary>
  public class ExtController : Controller
  {


    /// <summary>
    /// 创建一个 JSONP 的 ActionResult
    /// </summary>
    /// <param name="data">数据对象</param>
    /// <param name="callback">回调方法（若没有或者为null，则此方法行为与　Json　方法相同）。</param>
    /// <returns></returns>
    protected ActionResult Jsonp( object data, string callback = null )
    {
      if ( callback == null )
        return Json( data, JsonRequestBehavior.AllowGet );

      var serializer = new JavaScriptSerializer();
      var json = serializer.Serialize( data );

      return JavaScript( string.Format( "{0} ( {1} )", callback, json ) );
    }


    /// <summary>
    /// 创建一个处理 WebSocket 请求的 ActionResult
    /// </summary>
    /// <param name="processer">处理 WebSocket 请求的方法</param>
    /// <returns></returns>
    protected WebSocketResult WebSocket( Func<AspNetWebSocketContext, Task> processer )
    {

      EnsureWebSocketRequest();

      return new WebSocketResult( processer );
    }


    private void EnsureWebSocketRequest()
    {
      if ( !HttpContext.IsWebSocketRequest )
        throw new HttpException( 406, "当前请求不是 WebSocket 请求" );
    }



    /// <summary>
    /// 创建一个处理 WebSocket 请求的 ActionResult
    /// </summary>
    /// <param name="processer">处理 WebSocket 请求的方法</param>
    /// <returns></returns>
    protected WebSocketResult WebSocket( Func<WebSocket, Task> processer )
    {
      return new WebSocketResult( context => processer( context.WebSocket ) );
    }

  }
}
