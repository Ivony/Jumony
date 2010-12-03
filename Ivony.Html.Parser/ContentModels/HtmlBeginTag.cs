using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Parser.ContentModels
{
  public sealed class HtmlBeginTag : HtmlContentFragment
  {

    public HtmlBeginTag( HtmlContentFragment fragment, string tagName, IEnumerable<HtmlAttributeSetting> attibutes )
      : base( fragment )
    {
      TagName = tagName;
      Attributes = attibutes;
    }


    public string TagName
    {
      get;
      private set;
    }


    public IEnumerable<HtmlAttributeSetting> Attributes
    {
      get;
      private set;
    }


  }


}
