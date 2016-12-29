using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ivony.Html
{
  public abstract class HtmlNode
  {

    internal HtmlNode()
    {

    }


    public HtmlDocument Document { get; private set; }

    internal void AddTo( IHtmlContainer container )
    {
      Document = container.Document;
    }


  }
}
