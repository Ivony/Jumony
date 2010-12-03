using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Parser.ContentModels
{
  public sealed class HtmlEndTag : HtmlContentFragment
  {
    public HtmlEndTag( HtmlContentFragment info, string tagName )
      : base( info )
    {
      TagName = tagName;
    }

    public string TagName
    {
      get;
      private set;
    }


  }
}
