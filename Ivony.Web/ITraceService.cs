using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Ivony.Web
{

  /// <summary>
  /// 定义追踪服务
  /// </summary>
  public interface ITraceService
  {

    /// <summary>
    /// 写入追踪消息
    /// </summary>
    /// <param name="level">消息严重级别</param>
    /// <param name="category">消息类型目录</param>
    /// <param name="message">消息内容</param>
    void Trace( TraceLevel level, string category, string message );

  }
}
