using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Html.Parser.ContentModels;

namespace Ivony.Html.Parser
{
  public class JumonyHtmlParser : HtmlParserBase
  {

    protected override IHtmlDomProvider Provider
    {
      get { return new DomProvider(); }
    }

    protected override IHtmlReader CreateReader( string html )
    {
      return new JumonyReader( html );
    }

    protected override void ProcessEndTagMissingBeginTag( HtmlEndTag endTag )
    {
      //base.ProcessEndTagMissingBeginTag( endTag );
    }


  }
}
