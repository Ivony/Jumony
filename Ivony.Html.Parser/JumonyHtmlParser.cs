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
      get { return DomProvider.Instance; }
    }

    protected override IHtmlReader CreateReader( string html )
    {
      return new JumonyReader( html );
    }

    protected override void ProcessEndTagMissingBeginTag( HtmlEndTag endTag )
    {
      //忽略多出的结束标签
    }


  }
}
