using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Parser
{
  public abstract class JumonyHandler : HtmlHandler
  {

    protected override IHtmlDocument ParseDocument( string documentContent )
    {
      return new JumonyParser().Parse( documentContent );
    }

    protected override void ProcessDocument()
    {

      Process();

    }

    protected abstract void Process();


  }
}
