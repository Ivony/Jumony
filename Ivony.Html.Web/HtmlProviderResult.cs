using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web
{
  public class HtmlProviderResult
  {

    public HtmlProviderResult( string rewritePath, IHtmlParser parser )
    {
      RewritePath = rewritePath;
      Parser = parser;
    }


    public IHtmlParser Parser
    {
      get;
      private set;
    }

    public string RewritePath
    {
      get;
      private set;
    }

    public IHtmlHandler Handler
    {
      get;
      private set;
    }

  }
}
