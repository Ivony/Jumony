using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ivony.Html
{
  public sealed class HtmlDocument : IHtmlContainer
  {
    internal HtmlDocument()
    {
      Nodes = new HtmlNodeContainer( this );
    }

    public HtmlNodeContainer Nodes { get; }

    HtmlDocument IHtmlContainer.Document { get { return this; } }
  }
}
