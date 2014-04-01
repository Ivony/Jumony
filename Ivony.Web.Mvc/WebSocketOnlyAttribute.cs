using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Ivony.Web
{

  /// <summary>
  /// 标识 Action 只能处理 WebSocket 请求
  /// </summary>
  public class WebSocketOnlyAttribute : ActionMethodSelectorAttribute
  {

    /// <summary>
    /// 重写 IsValidForRequest 方法，判断当前请求是否为 WebSocket 请求
    /// </summary>
    /// <param name="controllerContext">当前控制器上下文</param>
    /// <param name="methodInfo">Action 方法信息</param>
    /// <returns>当前请求是否为 WebSocket 请求</returns>
    public override bool IsValidForRequest( ControllerContext controllerContext, MethodInfo methodInfo )
    {
      return controllerContext.HttpContext.IsWebSocketRequest;
    }
  }
}
