using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;

namespace Ivony.Web
{


  /// <summary>
  /// 将追踪信息写入 ASP.NET 追踪上下文的追踪服务。
  /// </summary>
  public sealed class AspNetTraceService : ITraceService
  {
    public void Trace( TraceLevel level, string category, string message )
    {

      switch ( level )
      {
        case TraceLevel.Error:
        case TraceLevel.Warning:

          TraceWarning( category, message );
          break;
        case TraceLevel.Info:
        case TraceLevel.Verbose:
          TraceInfo( category, message );
          break;
        case TraceLevel.Off:
        default:
          break;
      }
    }

    private static void TraceInfo( string category, string message )
    {
      var context = HttpContext.Current;
      if ( context == null )
        return;

      context.Trace.Write( category, message );
    }

    private static void TraceWarning( string category, string message )
    {
      var context = HttpContext.Current;
      if ( context == null )
        return;

      context.Trace.Warn( category, message );
    }
  }
}
