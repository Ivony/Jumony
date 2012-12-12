using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Parser.ContentModels
{
  public sealed class HtmlTextContent : HtmlContentFragment
  {
    public HtmlTextContent( HtmlContentFragment fragement ) : base( fragement ) { }
  }
}
