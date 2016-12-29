using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ivony.Html
{
  public sealed class HtmlElement : HtmlNode, IHtmlContainer
  {


    internal HtmlElement()
    {
      Nodes = new HtmlNodeContainer( this );
    }



    public string Name { get; }

    public HtmlNodeContainer Nodes { get; }
  }
}
