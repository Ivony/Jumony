using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{

  public interface IHtmlParser
  {

    IHtmlDocument ParseDocument( string html );

    HtmlFragment ParseFragment( string html );

  }
}
