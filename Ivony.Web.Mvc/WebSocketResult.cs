using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.WebSockets;

namespace Ivony.Web
{

  /// <summary>
  /// 定义一个处理 WebSocket 请求的 Result
  /// </summary>
  public class WebSocketResult : ActionResult
  {

    /// <summary>
    /// 创建 WebSocketResult 对象
    /// </summary>
    /// <param name="processer">处理函数</param>
    public WebSocketResult( Func<AspNetWebSocketContext, Task> processer )
    {
      Processer = processer;
    }


    /// <summary>
    /// 用于处理 WebSocket 请求的处理函数
    /// </summary>
    public Func<AspNetWebSocketContext, Task> Processer
    {
      get;
      private set;
    }


    /// <summary>
    /// 在当前上下文执行结果
    /// </summary>
    /// <param name="context">当前控制器上下文</param>
    public override void ExecuteResult( ControllerContext context )
    {

      if ( !context.HttpContext.IsWebSocketRequest )
        throw new HttpException( 406, "当前请求不是 WebSocket 请求" );

      context.HttpContext.AcceptWebSocketRequest( Processer );
    }
  }
}
