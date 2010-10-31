using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{
  public interface IHtmlContainer : IHtmlNode
  {

    IEnumerable<IHtmlNode> Nodes();

  }
}
