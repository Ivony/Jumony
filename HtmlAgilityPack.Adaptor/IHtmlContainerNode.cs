using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace Ivony.Html.HtmlAgilityPackAdaptor
{
  internal interface IHtmlContainerNode
  {

    HtmlNode Node
    {
      get;
    }

  }
}
