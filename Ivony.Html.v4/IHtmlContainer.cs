using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ivony.Html
{
  public interface IHtmlContainer
  {

    HtmlNodeContainer Nodes { get; }


    HtmlDocument Document { get; }

  }
}
