using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Ivony.Web
{
  public interface ITraceService
  {

    void Trace( TraceLevel level, string category, string message );

  }
}
