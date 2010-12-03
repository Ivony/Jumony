using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Html.Parser.ContentModels;

namespace Ivony.Html.Parser
{
  public interface IHtmlReader
  {

    string HtmlText
    {
      get;
    }


    IEnumerable<HtmlContentFragment> EnumerateContent();


  }
}
