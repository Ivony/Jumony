using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Ivony.Web
{
  public class AspNetTraceService : ITraceService
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

    private void TraceInfo( string category, string message )
    {
      throw new NotImplementedException();
    }

    private void TraceWarning( string category, string message )
    {
      throw new NotImplementedException();
    }
  }
}
