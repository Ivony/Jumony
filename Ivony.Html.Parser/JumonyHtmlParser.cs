using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Parser
{
  public class JumonyHtmlParser : HtmlParserBase
  {

    protected override IHtmlDomProvider Provider
    {
      get { return new DomProvider(); }
    }

    protected override void ProcessEndTagMissingBeginTag( System.Text.RegularExpressions.Match match )
    {
      //base.ProcessEndTagMissingBeginTag( match );
    }
  }
}
